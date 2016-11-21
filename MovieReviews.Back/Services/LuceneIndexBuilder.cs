using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Services
{
    public class LuceneIndexBuilder : IIndexBuilder
    {
        private const int REVIEW_TO_PROCESS_COUNT = 5000;

        protected IMovieApiService MovieApi { get; private set; }
        protected IIndexDirectoryProvider IndexDirectoryProvider { get; private set; }

        public LuceneIndexBuilder(IMovieApiService movieApi, IIndexDirectoryProvider indexDirectoryProvider)
        {
            if (movieApi == null)
                throw new ArgumentNullException("movieApi");
            if (indexDirectoryProvider == null)
                throw new ArgumentNullException("indexDirectoryProvider");

            MovieApi = movieApi;
            IndexDirectoryProvider = indexDirectoryProvider;
        }

        public void BuildIndex(bool fetchMetaData)
        {
            var currentPath = AppDomain.CurrentDomain.BaseDirectory;
            var resourcePath = Path.Combine(currentPath, "Resources");
            var indexDir = IndexDirectoryProvider.GetIndexDirectory();

            var positiveReviewPaths = Directory.GetFiles(Path.Combine(resourcePath, "aclImdb\\train\\pos")).OrderBy(n => n).Take(REVIEW_TO_PROCESS_COUNT).ToArray();
            var negativeReviewPaths = Directory.GetFiles(Path.Combine(resourcePath, "aclImdb\\train\\neg")).OrderBy(n => n).Take(REVIEW_TO_PROCESS_COUNT).ToArray();

            using (var snow = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_29, "English"))
            using (var idxw = new IndexWriter(indexDir, snow, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                for (var i = 0; i < REVIEW_TO_PROCESS_COUNT; i++)
                {
                    var docP = CreateReviewDocument(positiveReviewPaths[i], Path.Combine(resourcePath, "aclImdb\\train\\urls_pos.txt"), fetchMetaData);
                    idxw.AddDocument(docP);

                    var docN = CreateReviewDocument(negativeReviewPaths[i], Path.Combine(resourcePath, "aclImdb\\train\\urls_neg.txt"), fetchMetaData);
                    idxw.AddDocument(docN);
                }

                idxw.Optimize();
            }
        }

        private Document CreateReviewDocument(string reviewPath, string urlsPath, bool fetchMetaData)
        {
            var doc = new Document();
            var filename = Path.GetFileNameWithoutExtension(reviewPath);
            var scoreAndRating = filename.Split('_');
            var urlLineNumber = int.Parse(scoreAndRating.ElementAt(0));
            var score = scoreAndRating.ElementAt(1);
            var url = File.ReadLines(urlsPath).Skip(urlLineNumber).Take(1).First();
            var id = ParseImdbIdFromUrl(url);

            int intScore;
            var intScoreParsed = int.TryParse(score, out intScore) ? intScore : -1;

            var fldText = new Field("text", File.ReadAllText(reviewPath), Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES);
            var fldUrl = new Field("url", url, Field.Store.YES, Field.Index.NO, Field.TermVector.NO);
            var fldScr = new NumericField("score", 1, Field.Store.YES, true).SetIntValue(intScoreParsed);
            var fldImdbId = new Field("imdbId", id, Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO);

            if (fetchMetaData)
            {
                var movie = MovieApi.GetMovie(id);
                DateTime release;

                var fldImdbId2 = new Field("imdbId2", movie.imdbID ?? "N/A", Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO);
                var fldTitle = new Field("title", movie.Title ?? "N/A", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES);
                var fldRuntime = new Field("runtime", movie.Runtime ?? "N/A", Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO);
                var fldReleased = new Field("releaseDate", DateTime.TryParse(movie.Released, out release) ? DateTools.DateToString(release, DateTools.Resolution.DAY) : "N/A", Field.Store.YES, Field.Index.NOT_ANALYZED);
                var fldGenre = new Field("genre", movie.Genre ?? "N/A", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES);
                var fldEpisodeName = new Field("episodeTitle", movie.EpisodeName ?? "", Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.YES);
                var fldImageUrl = new Field("image", movie.Poster ?? "N/A", Field.Store.YES, Field.Index.NO, Field.TermVector.NO);
                var fldCountry = new Field("country", movie.Country ?? "N/A", Field.Store.YES, Field.Index.NOT_ANALYZED, Field.TermVector.NO);
                var fldTotalRating = new Field("overallScore", movie.imdbRating.ToString("G"), Field.Store.YES, Field.Index.NO, Field.TermVector.NO);

                doc.Add(fldImdbId2);
                doc.Add(fldTitle);
                doc.Add(fldRuntime);
                doc.Add(fldReleased);
                doc.Add(fldGenre);
                doc.Add(fldEpisodeName);
                doc.Add(fldImageUrl);
                doc.Add(fldTotalRating);
                doc.Add(fldCountry);
            }

            doc.Add(fldImdbId);
            doc.Add(fldText);
            doc.Add(fldUrl);
            doc.Add(fldScr);
            return doc;
        }

        private string ParseImdbIdFromUrl(string url)
        {
            return url.Replace("http://www.imdb.com/title/", String.Empty).Replace("/usercomments", String.Empty);
        }
    }
}
