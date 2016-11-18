using System;

namespace MovieReviews.Back.ServiceObjects
{
    public class OmdbMovie
    {
        public string Title { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
    }
}
