using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Core
{
    public static class Config
    {
        public static string CustomerServiceAppUrl
        {
            get { return Settings.GetSetting("AMS.CustomerServiceAppUrl"); }
        }

        public static string ReviewsServiceAppUrl
        {
            get { return Settings.GetSetting("AMS.ReviewsServiceAppUrl"); }
        }

        public static string CustomerServiceAppCountUrl
        {
            get { return Settings.GetSetting("AMS.CustomerServiceAppCountUrl"); }
        }
    }
}