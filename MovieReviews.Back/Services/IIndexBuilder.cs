using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieReviews.Back.Services
{
    public interface IIndexBuilder
    {
        /// <summary>
        /// Builds the Lucene index using the data contained in MovieReviews.Back/Resources/aclImdb
        /// </summary>
        /// <param name="fetchMetaData">If set, additional meta data will be fetched from OMDB</param>
        void BuildIndex(bool fetchMetaData);
    }
}
