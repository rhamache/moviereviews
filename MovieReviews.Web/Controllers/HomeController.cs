using MovieReviews.Back.Model;
using MovieReviews.Back.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MovieReviews.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(string searchTerm)
        {
            using (var searchService = new LuceneSearchService())
            {
                if (String.IsNullOrWhiteSpace(searchTerm))
                    return Json(new { results = new Review[] { } });

                var results = searchService.Search(searchTerm).ToList();
                return Json(new { results });
            }
        }
    }
}
