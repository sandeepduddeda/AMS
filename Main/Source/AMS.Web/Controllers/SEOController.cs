using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Web.Controllers
{
    public class SEOController : BaseController
    {
        public ActionResult PageTags()
        {
            return View("~/Views/AMS/allmysons/SEO/PageTags.cshtml");
        }

        public ActionResult VisitorTracking()
        {
            var querySrc = Request.QueryString["src"];

            if (!string.IsNullOrWhiteSpace(querySrc))
            {
                var cookie = Request.Cookies["ams_src"] ?? new HttpCookie("ams_src", querySrc);
                cookie.Value = querySrc;
                cookie.HttpOnly = true;
                Response.Cookies.Add(cookie);
            }

            return new EmptyResult();
        }
    }
}