using MovieReviews.Back.ServiceObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Services
{
    public interface IMovieApiService
    {
        OmdbMovie GetMovie(string imdbId);
    }
}
