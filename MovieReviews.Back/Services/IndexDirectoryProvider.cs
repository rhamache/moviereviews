using Lucene.Net.Store;
using System;

namespace MovieReviews.Back.Services
{
    public class IndexDirectoryProvider : IIndexDirectoryProvider
    {
        private string _indexPath;

        public IndexDirectoryProvider(string indexPath)
        {
            if (string.IsNullOrWhiteSpace(indexPath))
                throw new ArgumentNullException("indexPath");

            _indexPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, indexPath);
        }

        public Directory GetIndexDirectory()
        {
            var dir = System.IO.Path.Combine(_indexPath, "ReviewIndex");
            return FSDirectory.Open(new System.IO.DirectoryInfo(dir));
        }

        public Directory GetAutocompleteIndexDirectory()
        {
            var dir = System.IO.Path.Combine(_indexPath, "AutoCompleteIndex");
            return FSDirectory.Open(new System.IO.DirectoryInfo(dir));
        }
    }
}
