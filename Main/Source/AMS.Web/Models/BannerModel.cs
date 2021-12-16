using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class BannerModel
    {
        public HtmlString Title { get; set; }
        public HtmlString Subtitle { get; set; }
        public string ImageUrl { get; set; }
        public string ImageAlt { get; set; }
        public string RequestAQuoteUrl { get; set; }
    }
}