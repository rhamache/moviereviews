using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieReviews.Back.ServiceObjects;
using System.Net;
using Newtonsoft.Json;

namespace MovieReviews.Back.Services
{
    public class OmdbApiService : IMovieApiService
    {
        public OmdbMovie GetMovie(string imdbId)
        {
            using (var client = new WebClient())
            {
                var jsonResp = client.DownloadString(String.Format("http://www.omdbapi.com/?i={0}", imdbId));
                var res = JsonConvert.DeserializeObject<OmdbMovie>(jsonResp);
                if (res.Title == "#DUPE#")
                {
                    return GetMovie(res.imdbID);
                }
                return res;
            }
        }
    }
}
