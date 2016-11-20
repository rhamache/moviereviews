using MovieReviews.Back.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieReviews.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        protected IIndexBuilder IndexBuilder { get; private set; }
        protected IAutoCompleteService AutoCompleteService { get; private set; }

        public AdminController(IIndexBuilder indexBuilder, IAutoCompleteService autoCompleteService)
        {
            if (indexBuilder == null)
                throw new ArgumentNullException("indexBuilder");
            if (autoCompleteService == null)
                throw new ArgumentNullException("autoCompleteService");

            IndexBuilder = indexBuilder;
            AutoCompleteService = autoCompleteService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult BuildLuceneIndex(bool fetchMetaData = false)
        {
            IndexBuilder.BuildIndex(fetchMetaData);
            return Json(new { status = "success" });
        }

        [HttpPost]
        public JsonResult BuildLuceneAutoCompleteIndex()
        {
            AutoCompleteService.BuildAutoCompleteIndex(new[] {"text", "title"});
            return Json(new { status = "success" });
        }
    }
}