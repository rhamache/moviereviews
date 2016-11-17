using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using MovieReviews.Back;
using System.Configuration;

[assembly: OwinStartup(typeof(MovieReviews.Web.Startup))]

namespace MovieReviews.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            ConfigureAuth(app);
            MovieReviewDatabaseStructure.CreateDatabase(ConfigurationManager.AppSettings["dbPath"]);
            MovieReviewDatabaseStructure.UpdateSchema();
        }
    }
}
