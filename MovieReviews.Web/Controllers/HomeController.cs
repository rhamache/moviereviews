using MovieReviews.Back.Model;
using MovieReviews.Back.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieReviews.Web.Controllers
{
    public class HomeController : Controller
    {
        protected IIndexDirectoryProvider IndexDirectoryProvider { get; private set; }
        protected IAutoCompleteService AutoCompleteService { get; private set; }

        public HomeController(IIndexDirectoryProvider indexDirectoryProvider, IAutoCompleteService autoCompleteService)
        {
            if (indexDirectoryProvider == null)
                throw new ArgumentNullException("indexDirectoryProvider");
            if (autoCompleteService == null)
                throw new ArgumentNullException("autoCompleteService");

            IndexDirectoryProvider = indexDirectoryProvider;
            AutoCompleteService = autoCompleteService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(string searchTerm, int page, int? scoreMin, int? scoreMax)
        {
            using (var searchService = new LuceneSearchService(IndexDirectoryProvider))
            {
                if (String.IsNullOrWhiteSpace(searchTerm))
                    return Json(new { results = new Review[] { }, spelling = new string[] { } });

                int hits;
                try
                {
                    var spellingSuggestions = AutoCompleteService.SpellCheck(searchTerm);

                    var results = searchService.Search(searchTerm, page * 10, 10, scoreMin.GetValueOrDefault(0), scoreMax.GetValueOrDefault(10), out hits).ToList();
                    return Json(new { results, hits, status = "success", spelling = spellingSuggestions });
                }
                catch (Exception)
                {
                    return Json(new { status = "error" });
                }
            }
        }

        [HttpPost]
        public JsonResult GetAutoCompleteTerms(string searchTerm)
        {
            var results = AutoCompleteService.SuggestTermsFor(searchTerm);
            return Json(new { results });
        }
    }
}
