using Lucene.Net.Store;

namespace MovieReviews.Back.Services
{
    public interface IIndexDirectoryProvider
    {
        Directory GetIndexDirectory();
        Directory GetAutocompleteIndexDirectory();
    }
}
