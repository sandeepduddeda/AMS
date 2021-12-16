using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Security;
using System.Web.SessionState;
using AMS.Web.Extensions;

namespace AMS.Web
{
    public class MvcApplication : Sitecore.Web.Application
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExecutionTimeFilterAttribute());
            // Any other filters here...
        }
    }
}