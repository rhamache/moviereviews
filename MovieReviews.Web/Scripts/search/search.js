var ResultsViewModel = function (searchUrl, autoCompleteUrl, inputId) {
    var self = this;
    self.skipAutoComplete = false;
    self.searchTerm = ko.observable("").extend({ rateLimit: 500 });;
    self.results = ko.observableArray([]);
    self.page = ko.observable(0);
    self.hits = ko.observable(0);
    self.filterMax = ko.observable(null);
    self.filterMin = ko.observable(null);

    self.totalPages = ko.computed(function () {
        return Math.ceil(self.hits() / 10);
    });

    self.searchTerm.subscribe(function (newVal) {
        self.setPage(0, false);
    });

    self.filterMax.subscribe(function (newVal) {
        if (newVal !== null && self.filterMin() !== null && newVal < self.filterMin()) {
            alert("Can not set the minimum less than the maximum.")
            return;
        }

        if (newVal !== null)
            self.setPage(0, true);
    });

    self.filterMin.subscribe(function (newVal) {
        if (newVal !== null && self.filterMax() !== null && newVal > self.filterMax()) {
            alert("Can not set the maximum greater than the minimum.")
            return;
        }

        if (newVal !== null)
            self.setPage(0, true);
    });

    self.search = function () {
        // forces autocomplete to hide
        var auto = $('#' + inputId);
        auto.autocomplete("option", { source: self.autoCompleteTerms() });
        $.ajax({
            url: searchUrl,
            type: "POST",
            data: {
                searchTerm: self.searchTerm(),
                page: self.page(),
                scoreMin: self.filterMin(),
                scoreMax: self.filterMax()
            },
            success: function (resp) {
                if (!self.skipAutoComplete) {
                    self.getAutoCompleteTerms(self.searchTerm());
                } else {
                    auto.blur();
                }
                self.skipAutoComplete = false;
                if (resp.status === "error")
                    return;

                $("#resultTerm").text(self.searchTerm());
                self.hits(resp.hits);
                self.results.removeAll();
                for (var i = 0; i < resp.results.length; i++) {
                    self.results.push(new searchResult(resp.results[i]));
                }
                if (resp.results.length === 0) {
                    self.spellingSuggestions.removeAll();
                    for (var i = 0; i < resp.spelling.length; i++) {
                        self.spellingSuggestions.push(resp.spelling[i]);
                    }
                }
                if (resp.results.length === 0) {
                    self.filterMin(null);
                    self.filterMax(null);

                    $('#dropdownMin').val('0');
                    $("#dropdownMin").selectmenu("refresh");

                    $('#dropdownMax').val('10');
                    $("#dropdownMax").selectmenu("refresh");
                }
                $('[data-toggle="tooltip"]').tooltip();
            }
        });
    }

    self.pagerHtml = ko.computed(function () {
        var html = "";
        var start = Math.max(0, self.page() - 5);
        var end = Math.min(self.totalPages(), start + 10);

        if (self.page() != 0) {
            html += "<button class='btn btn-default' onclick='vm.setPage(" + 0 + ", true)'><span class='fa fa-angle-double-left'></span></button>";
            html += "<button class='btn btn-default' onclick='vm.decrementPage()'><span class='fa fa-angle-left'></span></button>";
        }

        for (var i = start; i < end; i++) {
            if (i == self.page()) {
                html += "<button class='btn btn-default active'>" + (i + 1) + "</button>";
            } else {
                html += "<button class='btn btn-default' onclick='vm.setPage(" + i + ", true)'>" + (i + 1) + "</button>";
            }
        }

        if ((self.page() + 1) < self.totalPages()) {
            html += "<button class='btn btn-default' onclick='vm.incrementPage()'><span class='fa fa-angle-right'></span></button>";
            html += "<button class='btn btn-default' onclick='vm.setPage(" + (self.totalPages() - 1) + ", true)'><span class='fa fa-angle-double-right'></span></button>";
        }

        return html;
    })

    self.hasResults = ko.computed(function () {
        return self.results().length > 0;
    });

    self.showFilter = ko.computed(function () {
        return self.hasResults() || self.filterMin() !== null || self.filterMax() !== null;
    });

    self.setPage = function (i, skipauto) {
        self.skipAutoComplete = skipauto;
        $("html, body").animate({ scrollTop: 0 }, 600);
        self.page(i);
        self.search();
    }

    self.decrementPage = function (i) {
        $("html, body").animate({ scrollTop: 0 }, 600);
        self.setPage(self.page() - 1, true);
    }

    self.incrementPage = function (i) {
        $("html, body").animate({ scrollTop: 0 }, 600);
        self.setPage(self.page() + 1, true);
    }

    self.autoCompleteTerms = ko.observableArray([]);
    $('#' + inputId).autocomplete({
        source: self.autoCompleteTerms(),
        select: function (e, ui) {
            self.skipAutoComplete = true;
            self.searchTerm(ui.item.value);
        },
        appendTo: "#autocompleteContainer"
    });

    self.getAutoCompleteTerms = function (term) {
        $.ajax({
            method: "POST",
            url: autoCompleteUrl,
            data: {
                searchTerm: self.searchTerm()
            },
            success: function (resp) {
                self.autoCompleteTerms.removeAll();
                for (var i = 0; i < resp.results.length; i++) {
                    self.autoCompleteTerms.push(resp.results[i]);
                }
                var auto = $('#' + inputId);
                auto.autocomplete("option", { source: self.autoCompleteTerms() });
                auto.autocomplete("search");
            }
        });
    };

    self.spellingSuggestions = ko.observableArray([]);

    self.hasSpellingSuggestions = function () {
        return self.spellingSuggestions().length > 0;
    }

    self.clearFilters = function () {
        self.filterMin(null);
        self.filterMax(null);
    }
}

var searchResult = function (reviewObj) {
    var self = this;

    self.text = ko.observable(reviewObj.Text);
    self.url = ko.observable(reviewObj.Url);
    self.title = ko.observable(reviewObj.Movie.Title);
    self.episode = ko.observable(reviewObj.Movie.EpisodeName);
    self.genre = ko.observable(reviewObj.Movie.Genre);
    self.releaseDate = reviewObj.Movie.ReleaseDate === null ?
        ko.observable("N/A") :
        ko.observable(moment(new Date(parseInt(reviewObj.Movie.ReleaseDate.substr(6)))).format('MMMM D, YYYY'));
    self.runningTime = ko.observable(reviewObj.Movie.RunningTime);
    self.imdbId = ko.observable(reviewObj.Movie.ImdbId);
    self.matchText = ko.observableArray(reviewObj.MatchedFragments);
    self.imageUrl = reviewObj.Movie.ImageUrl === "N/A" ? ko.observable("/Content/notfound.jpg") : ko.observable(reviewObj.Movie.ImageUrl);
    self.id = ko.observable(reviewObj.Id);
    self.isOpen = ko.observable(false);
    self.score = ko.observable(reviewObj.Score);

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

    self.imageError = function (a,b,c) {
        self.imageUrl("/Content/notfound.jpg");
    }

    self.toggleReview = function () {
        self.isOpen(!self.isOpen());
        $("#" + self.id()).slideToggle();
    }

    self.moreLinkText = ko.computed(function () {
        return self.isOpen() ? "[Collapse]" : "[Read Full Review]";
    })
}
