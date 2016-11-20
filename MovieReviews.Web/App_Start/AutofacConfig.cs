using Autofac;
using Autofac.Integration.Mvc;
using MovieReviews.Back.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovieReviews.Web.App_Start
{
    public class AutofacConfig
    {
        public static void AutofacStart()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<LuceneIndexBuilder>().As<IIndexBuilder>();
            builder.RegisterType<OmdbApiService>().As<IMovieApiService>();
            builder.RegisterType<LuceneAutoCompleteService>().As<IAutoCompleteService>();
            builder.Register(s => new IndexDirectoryProvider(ConfigurationManager.AppSettings["LuceneIndexDirectory"])).As<IIndexDirectoryProvider>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}