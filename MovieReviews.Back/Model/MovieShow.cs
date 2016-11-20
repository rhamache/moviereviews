namespace MovieReviews.Back.Model
{
    public class MovieShow
    {
        public string Title { get; set; }
        public string EpisodeName { get; set; }
        public string ImdbId { get; set; }
        public string Genre { get; set; }
        public string RunningTime { get; set; }
        public string ReleaseDate { get; set; }
        public string ImageUrl { get; set; }
        public string Country { get; set; }
        public decimal OverallScore { get; set; }
    }
}
