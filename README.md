# moviereviews

Movie reviews is a webapp that indexes and searches a collection of user reviews from the popular site [IMdb](http://www.imdb.com/).

The dataset was pulled from [http://ai.stanford.edu/~amaas/data/sentiment/](http://ai.stanford.edu/~amaas/data/sentiment/). The original dataset contains 25,000 user reviews. For the indexes included in this repro, 10000 of them were processed and included in the index.

The data is indexed and searched using [Lucene.net](https://lucenenet.apache.org/). This library is also used to implement autocompletion and spell checking.

## Running the App

To run, download the repo and open the app in Visual Studio. The app should run after restoring nuget packages. The app is also hosted at [http://hamachermoviereviews.azurewebsites.net/](http://hamachermoviereviews.azurewebsites.net/) for convenience, as I some free Azure hosting available to me.


## Building the index

This repo already includes a pre built Lucene, so this step can be skipped.

To build the index, the dataset from [http://ai.stanford.edu/~amaas/data/sentiment/](http://ai.stanford.edu/~amaas/data/sentiment/) must be placed in the MovieReviews/Resources folder. You can then start the app and navigate to ~/Admin/Admin. Select the checkbox to pull addtional metadata from IMDb for the index. When this option is selected, the index takes about 10 minutes to build. Then proceed to build the autocomplete and spell checker indexes (the review index should be built first)

### Features

The webapp is single page application for searching user reviews pulled from IMDb. The reviews are returned with additional metadata, such as the release date of the movie/show, the review score and a link to imdb.

When viewing results note that only the matched sentences are shown. To view the original review in full, the [Read Full Review] link must be pressed.

### Advanced Features

+ Stemming
    + Steming was implemented by building and searching the index with a Lucene SnowballAnalyzer. Some examples
        + "dumping", results that match on "dumped" are returned
        + "assessment", results that match on "assessed" and "assessing" are returned
+ Spellchecking
    + Spelling suggestions are shown, if and only if you search for a term that doesn't return any results
        + ex: search for "soldeir"
+ Related search terms
    + NOT IMPLEMENTED
+ Autocomplete
    + As you type, a dropdown will appear with autocomplete suggestions
+ Filtering
    + There is only one filter included for filtering reviews by score. It is accessed on the left hand side of the screen

Other features not mentioned in the original task

+ Pagination of results
++ Results are split into pages of 10 results per page (not configurable)
+ Integration with [OMDb API](https://www.omdbapi.com/)
++ Addtional movie/show metadata was pulled when the index was built to provide movie posters and release dates
+ Responsive design
++ I spent a little bit of time testing with my iPhone to ensure everything looks okay on mobile

## Third Party Libraries Used

+ jQuery
+ jQuery UI
+ Bootstrap 3
+ Lucene.net
+ Font awesome
+ Newtonsoft.Json
+ Knockout.js
+ Moment.js
+ Autofac

## Brief Infrastructure Description

The app uses knockout.js to interactively search and populate the results. There are only 2 pages in the app, the main saerch page and a small admin panel. All results and autocomplete suggestions are pulled using AJAX. jQuery UI was used for the filter dropdowns and autocomplete widget.

The server code is split into two projects, MovieReviews.Web that contains the MVC application and viuew models, controllers and views, and MovieReviews.Back, which handles all interaction with the Lucene indexes. Autofac was used to provide easy dependancy injection for my service classes.

The notable classes in the Backend are the LuceneSearchService and LuceneAutoCompleteService classes. LuceneSearchService provides a method to search and filter the index. Movie and episode titles are given more weight in search results. The search service also bolds the terms (using html \<b> tags) that matched the query.

The LuceneAutoCompleteService provides autocomplete suggestions and spell checking methods. Most of the code in this class was taken from [http://stackoverflow.com/questions/120180/how-to-do-query-auto-completion-suggestions-in-lucene](http://stackoverflow.com/questions/120180/how-to-do-query-auto-completion-suggestions-in-lucene), but I made some adjustments to provide for spell checking, and to make the class injectable.

## Known Issues/Improvments

+ Writing custom knockout bindings for the jQuery UI autocomplete and selectmenu widgets would be an improvement. For now, when the autocomplete suggestions are returned from the server, I refresh the widget's datasource manually and reopen the dropdown. For the filter dropdowns, I used the widgets change event to manually update the knockout vm, which is not a good pattern
+ You will often see 403 errors in the console relating to images. This is because some of the image urls I pulled from OMDb API are not permitted to be accessed from another domain
+ There is no user authentication at all. So the admin panel at ~/Admin/Admin is publically accessable. This would definitely need to be added in a real application. We have a user/identity framework at my current job, so I chose to take it out as I wasn't familiar with the default ASP.NET authentication framework. Luckily, there isn't any real damage that can be done, as attempting to rebuild any of the indexes will just result in an error on the live site (the raw data has not been uploaded to azure)
+ As I was working alone, I chose to just commit everything to master. I know that when working with a team a proper branching, pull request and code review workflow should be followed
