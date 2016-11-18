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
        protected IReviewRepository ReviewRepository { get; private set; }

        public HomeController(IReviewRepository reviewRepository)
        {
            if (reviewRepository == null)
                throw new ArgumentNullException("reviewRepository");

            ReviewRepository = reviewRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string searchTerm)
        {
            var res = String.IsNullOrWhiteSpace(searchTerm) ? "N/A" : searchTerm;
            return Content(res, "text/html");
        }
    }
}
