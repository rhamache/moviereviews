using System;

namespace MovieReviews.Back.ServiceObjects
{
    public class OmdbMovie
    {
        public string Title { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string seriesID { get; set; }
        public string Poster { get; set; }
        public string EpisodeName { get; set; }
        public decimal imdbRating { get; set; }
        public string Country { get; set; }
    }
}
