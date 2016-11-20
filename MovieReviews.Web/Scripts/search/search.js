var ResultsViewModel = function (searchUrl) {
    var self = this;
    self.searchTerm = ko.observable("");
    self.results = ko.observableArray([]);

    self.search = function () {
        $.ajax({
            url: searchUrl,
            type: "POST",
            data: {
                searchTerm: self.searchTerm()
            },
            success: function (resp) {
                self.results.removeAll();
                for (var i = 0; i < resp.results.length; i++) {
                    self.results.push(new searchResult(resp.results[i]));
                }
            }
        });
    }

    self.hasResults = ko.computed(function () {
        return self.results().length > 0;
    });
}

var searchResult = function (reviewObj) {
    var self = this;

    self.text = ko.observable(reviewObj.Text);
    self.url = ko.observable(reviewObj.Url);
    self.title = ko.observable(reviewObj.Movie.Title);
    self.episode = ko.observable(reviewObj.Movie.EpisodeName);
    self.genre = ko.observable(reviewObj.Movie.Genre);
    self.releaseDate = ko.observable(reviewObj.Movie.ReleaseDate);
    self.runningTime = ko.observable(reviewObj.Movie.RunningTime);
    self.imdbId = ko.observable(reviewObj.Movie.ImdbId);
    self.matchText = ko.observableArray(reviewObj.MatchedFragments);
    self.imageUrl = ko.observable(reviewObj.Movie.ImageUrl);

    self.getTitle = ko.computed(function () {
        var title = self.title();
        if (self.episode() !== null && self.episode() !== '') {
            title += " <small>(Episode: " + self.episode() + ")</small>";
        }
        return title;
    });

    self.getFragments = ko.computed(function () {
        var html = "";
        for (var i = 0; i < self.matchText().length; i++) {
            html += self.matchText()[i] + "... ";
        }
        return html;
    });
}
