using MovieReviews.Back.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Services
{
    public interface IReviewRepository
    {
        /// <summary>
        /// Get review entity by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>review</returns>
        Review Get(Guid id);

        /// <summary>
        /// Save review entity
        /// </summary>
        /// <param name="review">review</param>
        void Save(Review review);

        /// <summary>
        /// Update review entity
        /// </summary>
        /// <param name="review">review</param>
        void Update(Review review);

        /// <summary>
        /// Delete review entity
        /// </summary>
        /// <param name="review">person</param>
        void Delete(Review review);

        /// <summary>
        /// Row count review in db
        /// </summary>
        /// <returns>number of rows</returns>
        long RowCount();
    }
}
