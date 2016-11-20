var ResultsViewModel = function (searchUrl) {
    var self = this;
    self.searchTerm = ko.observable("");
    self.results = ko.observableArray([]);
    self.page = ko.observable(0);
    self.hits = ko.observable(0);

    self.totalPages = ko.computed(function () {
        return Math.ceil(self.hits() / 10);
    });

    self.search = function () {
        $.ajax({
            url: searchUrl,
            type: "POST",
            data: {
                searchTerm: self.searchTerm(),
                page: self.page()
            },
            success: function (resp) {
                $("#resultTerm").text(self.searchTerm());
                self.hits(resp.hits);
                self.results.removeAll();
                for (var i = 0; i < resp.results.length; i++) {
                    self.results.push(new searchResult(resp.results[i]));
                }
            }
        });
    }

    self.pagerHtml = ko.computed(function () {
        var html = "";
        var start = Math.max(0, self.page() - 5);
        var end = Math.min(self.totalPages(), start + 10);

        if (self.page() != 0) {
            html += "<button class='btn btn-default' onclick='vm.setPage(" + 0 + ")'><span class='fa fa-angle-double-left'></span></button>";
            html += "<button class='btn btn-default' onclick='vm.decrementPage()'><span class='fa fa-angle-left'></span></button>";
        }

        for (var i = start; i < end; i++) {
            if (i == self.page()) {
                html += "<button class='btn btn-default active'>" + (i + 1) + "</button>";
            } else {
                html += "<button class='btn btn-default' onclick='vm.setPage(" + i + ")'>" + (i + 1) + "</button>";
            }
        }

        if ((self.page() + 1) < self.totalPages()) {
            html += "<button class='btn btn-default' onclick='vm.incrementPage()'><span class='fa fa-angle-right'></span></button>";
            html += "<button class='btn btn-default' onclick='vm.setPage(" + (self.totalPages() - 1) + ")'><span class='fa fa-angle-double-right'></span></button>";
        }

        return html;
    })

    self.hasResults = ko.computed(function () {
        return self.results().length > 0;
    });

    self.setPage = function (i) {
        $("html, body").animate({ scrollTop: 0 }, 600);
        self.page(i);
        self.search();
    }

    self.decrementPage = function (i) {
        $("html, body").animate({ scrollTop: 0 }, 600);
        self.setPage(self.page() - 1);
    }

    self.incrementPage = function (i) {
        $("html, body").animate({ scrollTop: 0 }, 600);
        self.setPage(self.page() + 1);
    }
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
