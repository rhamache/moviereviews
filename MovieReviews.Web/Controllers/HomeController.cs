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
            var test = new Review
            {
                Title = "Test rev",
                Movie = null,
                Text = "A cool review"
            };
            ReviewRepository.Save(test);

            return View();
        }
    }
}
