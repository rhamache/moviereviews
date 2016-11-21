using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;
using SpellChecker.Net.Search.Spell;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.NGram;
using Lucene.Net.Documents;

namespace MovieReviews.Back.Services
{
    /// <summary>
    /// Search term auto-completer, works for single terms (so use on the last term of the query).
    /// Returns more popular terms first.
    /// <br/>
    /// Author: Mat Mannion, M.Mannion@warwick.ac.uk
    /// <seealso cref="http://stackoverflow.com/questions/120180/how-to-do-query-auto-completion-suggestions-in-lucene"/>
    /// </summary>
    public class LuceneAutoCompleteService : IAutoCompleteService
    {

        public int MaxResults { get; set; }

        private class AutoCompleteAnalyzer : Analyzer
        {
            public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
            {
                TokenStream result = new StandardTokenizer(kLuceneVersion, reader);

                result = new StandardFilter(result);
                result = new LowerCaseFilter(result);
                result = new ASCIIFoldingFilter(result);
                result = new StopFilter(false, result, StopFilter.MakeStopSet(kEnglishStopWords));
                result = new EdgeNGramTokenFilter(
                    result, Lucene.Net.Analysis.NGram.EdgeNGramTokenFilter.DEFAULT_SIDE, 1, 20);

                return result;
            }
        }

        private const int MAX_RESULTS = 8;

        private static readonly Lucene.Net.Util.Version kLuceneVersion = Lucene.Net.Util.Version.LUCENE_29;

        private static readonly string kGrammedWordsField = "words";

        private static readonly string kSourceWordField = "sourceWord";

        private static readonly string kCountField = "count";

        private static readonly string[] kEnglishStopWords = {
            "a", "an", "and", "are", "as", "at", "be", "but", "by",
            "for", "i", "if", "in", "into", "is",
            "no", "not", "of", "on", "or", "s", "such",
            "t", "that", "the", "their", "then", "there", "these",
            "they", "this", "to", "was", "will", "with"
        };

        private readonly Directory _sourceDirectory;
        private readonly Directory _directory;
        private readonly Directory _spellCheckDirectory;
        private readonly System.IO.FileInfo _dictFile;

        private IndexReader _reader;

        private IndexSearcher _searcher;

        public LuceneAutoCompleteService(IIndexDirectoryProvider indexDirectoryProvider)
        {
            if (indexDirectoryProvider == null)
                throw new ArgumentNullException("indexDirectoryProvider");

            _directory = indexDirectoryProvider.GetAutocompleteIndexDirectory();
            _sourceDirectory = indexDirectoryProvider.GetIndexDirectory();
            _spellCheckDirectory = indexDirectoryProvider.GetSpellCheckIndexDirectory();
            _dictFile = indexDirectoryProvider.GetSpellCheckDictionaryFileInfo();
            MaxResults = MAX_RESULTS;

            ReplaceSearcher();
        }

        public IEnumerable<string> SuggestTermsFor(string term)
        {
            if (_searcher == null)
                return new string[] { };

            // get the top terms for query
            Query query = new TermQuery(new Term(kGrammedWordsField, term.ToLower()));
            Sort sort = new Sort(new SortField(kCountField, SortField.INT, true));

            TopDocs docs = _searcher.Search(query, null, MaxResults, sort);
            string[] suggestions = docs.ScoreDocs.Select(doc =>
                _reader.Document(doc.Doc).Get(kSourceWordField)).ToArray();

            return suggestions;
        }

        public IEnumerable<string> SpellCheck(string term)
        {
            if (_searcher == null)
                return new string[] { };

            var spellChecker = new SpellChecker.Net.Search.Spell.SpellChecker(_spellCheckDirectory);
            string[] suggestions = spellChecker.SuggestSimilar(term, MaxResults);

            return suggestions;
        }

        public void BuildAutoCompleteIndex(string[] fieldsToAutocomplete)
        {
            // build a dictionary (from the spell package)
            using (IndexReader sourceReader = IndexReader.Open(_sourceDirectory, true))
            {
                // code from
                // org.apache.lucene.search.spell.SpellChecker.indexDictionary(
                // Dictionary)
                //IndexWriter.Unlock(m_directory);

                // use a custom analyzer so we can do EdgeNGramFiltering
                var analyzer = new AutoCompleteAnalyzer();
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED))
                {
                    writer.MergeFactor = 300;
                    writer.SetMaxBufferedDocs(150);

                    foreach (var fieldToAutocomplete in fieldsToAutocomplete)
                    {

                        LuceneDictionary dict = new LuceneDictionary(sourceReader, fieldToAutocomplete);// go through every word, storing the original word (incl. n-grams) 
                        // and the number of times it occurs
                        foreach (string word in dict)
                        {
                            if (word.Length < 3)
                                continue; // too short we bail but "too long" is fine...

                            // ok index the word
                            // use the number of documents this word appears in
                            int freq = sourceReader.DocFreq(new Term(fieldToAutocomplete, word));
                            var doc = MakeDocument(word, freq);

                            writer.AddDocument(doc);
                        }
                    }

                    writer.Optimize();
                }

            }
        }

        public void BuildSpellCheckIndex()
        {
            using (IndexReader sourceReader = IndexReader.Open(_sourceDirectory, true))
            {
                var spellChecker = new SpellChecker.Net.Search.Spell.SpellChecker(_spellCheckDirectory);
                var dict = new LuceneDictionary(sourceReader, "text");
                spellChecker.IndexDictionary(dict);
            }
        }

        private static Document MakeDocument(string word, int frequency)
        {
            var doc = new Document();
            doc.Add(new Field(kSourceWordField, word, Field.Store.YES,
                    Field.Index.NOT_ANALYZED)); // orig term
            doc.Add(new Field(kGrammedWordsField, word, Field.Store.YES,
                    Field.Index.ANALYZED)); // grammed
            doc.Add(new Field(kCountField,
                    frequency.ToString(), Field.Store.YES,
                    Field.Index.NOT_ANALYZED)); // count
            return doc;
        }

        private void ReplaceSearcher()
        {
            if (IndexReader.IndexExists(_directory))
            {
                if (_reader == null)
                    _reader = IndexReader.Open(_directory, true);
                else
                    _reader.Reopen();

                _searcher = new IndexSearcher(_reader);
            }
            else
            {
                _searcher = null;
            }
        }
    }
}