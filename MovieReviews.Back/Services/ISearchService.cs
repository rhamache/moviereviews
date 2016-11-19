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
        IQueryable<Review> Search(string searchText);
    }
}
