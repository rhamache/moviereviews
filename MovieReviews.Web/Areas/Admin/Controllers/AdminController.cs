﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieReviews.Web.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}