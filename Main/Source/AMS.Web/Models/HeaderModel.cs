using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class HeaderModel
    {
        public HtmlString Phone { get; set; }
        public HtmlString Title { get; set; }
        public string RatingText { get; set; }
        public float RatingValue { get; set; }
        public int RatingCount { get; set; }
        public float NationalRatingValue { get; set; }
        public int NationalRatinCount { get; set; }
    }
}