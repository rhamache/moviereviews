using Lucene.Net.Store;
using System.Collections.Generic;

namespace MovieReviews.Back.Services
{
    public interface IAutoCompleteService
    {
        /// <summary>
        /// Find terms matching the given partial word that appear in the highest number of documents.</summary>
        /// <param name="term">A word or part of a word</param>
        /// <returns>A list of suggested completions</returns>
        IEnumerable<string> SuggestTermsFor(string term);

        /// <summary>
        /// Open the index in the given directory and create a new index of word frequency for the 
        /// given index.</summary>
        /// <param name="sourceDirectory">Directory containing the index to count words in.</param>
        /// <param name="fieldToAutocomplete">The fields in the index that should be analyzed.</param>
        void BuildAutoCompleteIndex(string[] fieldsToAutocomplete);
    }
}
