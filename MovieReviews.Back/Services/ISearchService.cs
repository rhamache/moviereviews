using MovieReviews.Back.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Searches the index for the given text
        /// </summary>
        /// <param name="searchText">The search text</param>
        /// <param name="skip">The number of results to skip</param>
        /// <param name="take">The number of results to return</param>
        /// <param name="hitCount">The number of results will be returned in this variable</param>
        IQueryable<Review> Search(string searchText, int skip, int take, out int hitCount);
    }
}
