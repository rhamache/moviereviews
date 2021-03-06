﻿using System;
using System.Collections.Generic;

namespace MovieReviews.Back.Model
{
    public class Review
    {
        public Guid Id { get; set; }
        public MovieShow Movie { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public IEnumerable<string> MatchedFragments { get; set; }
        public decimal Score { get; set; }
    }
}
