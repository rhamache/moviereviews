using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using MovieReviews.Back.Model;
using System;
using System.Linq;

namespace MovieReviews.Back.Services
{
    public class LuceneSearchService : ISearchService, IDisposable
    {
        protected SnowballAnalyzer SnowballAnalyzer { get; private set; }
        protected Directory IndexDirectory { get; private set; }

        public LuceneSearchService(IIndexDirectoryProvider indexDirectoryProvider)
        {
            if (indexDirectoryProvider == null)
                throw new ArgumentNullException("indexDirectoryProvider");

            SnowballAnalyzer = new SnowballAnalyzer(Lucene.Net.Util.Version.LUCENE_29, "English");
            IndexDirectory = indexDirectoryProvider.GetIndexDirectory();
        }

        public IQueryable<Review> Search(string searchText, int skip, int take, int scoreMin, int scoreMax, out int totalHits)
        {
            var multiParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29,new[] { "text", "genre", "score" }, SnowballAnalyzer);
            var titleParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, new[] { "title", "episodeTitle" }, SnowballAnalyzer);

            var multiQuery = multiParser.Parse(searchText);
            var titleQuery = titleParser.Parse(searchText);

            var filter = NumericRangeFilter.NewIntRange("score", scoreMin, scoreMax, true, true);

            titleQuery.Boost = 0.8f;
            multiQuery.Boost = 0.2f;

            var query = new BooleanQuery();
            query.Add(multiQuery, Occur.SHOULD);
            query.Add(titleQuery, Occur.SHOULD);

            var reviewSearcher = new IndexSearcher(IndexDirectory);

            var hits = reviewSearcher.Search(query, filter, 10000);
            var reviewDocs = hits.ScoreDocs.Skip(skip).Take(take).Select(sc => reviewSearcher.Doc(sc.Doc)).ToList();
            totalHits = hits.TotalHits;

            var scorer = new QueryScorer(query);
            var highlighter = new Highlighter(scorer);

            // null highlighter will return the ENTIRE field with relevant terms highlighted
            var nullHighlighter = new Highlighter(scorer);


            nullHighlighter.TextFragmenter = new NullFragmenter();
            decimal score, overall;
            DateTime release; 
            return reviewDocs.AsQueryable().Select(doc => new Review
            {
                Id = Guid.NewGuid(),
                Url = doc.GetField("url").StringValue,
                Text = nullHighlighter.GetBestFragment(SnowballAnalyzer, "text", doc.GetField("text").StringValue) ?? doc.GetField("text").StringValue,
                Movie = new MovieShow
                {
                    Title = nullHighlighter.GetBestFragment(SnowballAnalyzer, "title", doc.GetField("title").StringValue) ?? doc.GetField("title").StringValue,
                    ImdbId = nullHighlighter.GetBestFragment(SnowballAnalyzer, "imdbId", doc.GetField("imdbId").StringValue) ?? doc.GetField("imdbId").StringValue,
                    Genre = nullHighlighter.GetBestFragment(SnowballAnalyzer, "genre", doc.GetField("genre").StringValue) ?? doc.GetField("genre").StringValue,
                    RunningTime = doc.GetField("runtime").StringValue,
                    ReleaseDate = doc.GetField("releaseDate").StringValue != "N/A" ? DateTools.StringToDate(doc.GetField("releaseDate").StringValue) : (DateTime?) null,
                    EpisodeName = nullHighlighter.GetBestFragment(SnowballAnalyzer, "episodeTitle", doc.GetField("episodeTitle").StringValue) ?? doc.GetField("episodeTitle").StringValue,
                    OverallScore = decimal.TryParse(doc.GetField("overallScore").StringValue, out overall) ? overall : -1m,
                    Country = doc.GetField("country").StringValue,
                    ImageUrl = doc.GetField("image").StringValue
                },
                MatchedFragments = highlighter.GetBestFragments(SnowballAnalyzer, "text", doc.GetField("text").StringValue, 5)
                    .Concat(highlighter.GetBestFragments(SnowballAnalyzer, "title", doc.GetField("title").StringValue, 5))
                    .Concat(highlighter.GetBestFragments(SnowballAnalyzer, "episodeTitle", doc.GetField("episodeTitle").StringValue, 5)),
                Score = decimal.TryParse(doc.GetField("score").StringValue, out score) ? score : -1m
            });
        }

        public void Dispose()
        {
            SnowballAnalyzer.Dispose();
        }
    }
}
