using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using MovieReviews.Back.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Services
{
    public class LuceneSearchService : ISearchService, IDisposable
    {
        protected SnowballAnalyzer SnowballAnalyzer { get; private set; }
        protected FSDirectory IndexDirectory { get; private set; }

        public LuceneSearchService()
        {
            SnowballAnalyzer = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_29, "English");

            var currentPath = AppDomain.CurrentDomain.BaseDirectory;
            var resourcePath = Path.Combine(currentPath, "..\\MovieReviews.Back\\Resources");
            IndexDirectory = FSDirectory.Open(new DirectoryInfo(Path.Combine(resourcePath, "ReviewIndex")));
        }

        public IQueryable<Review> Search(string searchText)
        {
            var multiParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29,new[] { "text", "imdbId", "title", "genre" }, SnowballAnalyzer);

            var query = multiParser.Parse(searchText);
            var reviewSearcher = new IndexSearcher(IndexDirectory);

            var hits = reviewSearcher.Search(query, 10);
            var reviewDocs = hits.ScoreDocs.Select(sc => reviewSearcher.Doc(sc.Doc));

            var scorer = new QueryScorer(query);
            var highlighter = new Highlighter(scorer);

            // null highlighter will return the ENTIRE field with relevant terms highlighted
            var nullHighlighter = new Highlighter(scorer);
            nullHighlighter.TextFragmenter = new NullFragmenter();

            return reviewDocs.AsQueryable().Select(doc => new Review
            {
                Url = doc.GetField("url").StringValue,
                Text = nullHighlighter.GetBestFragment(SnowballAnalyzer, "text", doc.GetField("text").StringValue) ?? doc.GetField("text").StringValue,
                Movie = new MovieShow
                {
                    Title = nullHighlighter.GetBestFragment(SnowballAnalyzer, "title", doc.GetField("title").StringValue) ?? doc.GetField("title").StringValue,
                    ImdbId = nullHighlighter.GetBestFragment(SnowballAnalyzer, "imdbId", doc.GetField("imdbId").StringValue) ?? doc.GetField("imdbId").StringValue,
                    Genre = nullHighlighter.GetBestFragment(SnowballAnalyzer, "genre", doc.GetField("genre").StringValue) ?? doc.GetField("genre").StringValue,
                    RunningTime = doc.GetField("runtime").StringValue,
                    ReleaseDate = doc.GetField("releaseDate").StringValue
                },
                MatchedFragments = new[] 
                {
                    highlighter.GetBestFragment(SnowballAnalyzer, "title", doc.GetField("title").StringValue),
                    highlighter.GetBestFragment(SnowballAnalyzer, "text", doc.GetField("text").StringValue),
                    highlighter.GetBestFragment(SnowballAnalyzer, "genre", doc.GetField("genre").StringValue),
                    highlighter.GetBestFragment(SnowballAnalyzer, "imdbId", doc.GetField("imdbId").StringValue),
                }
            });
        }

        public void Dispose()
        {
            SnowballAnalyzer.Dispose();
        }
    }
}
