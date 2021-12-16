using AMS.Web.Models;
using AMS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Web.Controllers
{
    public class BlogController : BaseController
    {
        private readonly BlogService _blogService;

        public BlogController()
        {
            _blogService = new BlogService();
        }

        public ActionResult BlogPostsListFull(int? page)
        {
            return View("~/Views/AMS/allmysons/Renderings/Blog/BlogPostsListFull.cshtml", new BlogList { ItemsPerPage = 10, Page = page, Blogs = _blogService.List() });
        }

        /// <param name="t">category/tag</param>   
        /// <param name="y">year</param>
        /// <param name="m">month</param>  
        public ActionResult BlogPostsListShort(string t, int? y, int? page)
        {
            var blogs = _blogService.List(t, y);
            if (blogs == null)
                return HttpNotFound();

            return View("~/Views/AMS/allmysons/Renderings/Blog/BlogPostsListShort.cshtml", new BlogList { ItemsPerPage = 10, Page = page, Blogs = blogs, Category = t, Year = y });
        }

        public ActionResult BlogArchives()
        {
            return View("~/Views/AMS/allmysons/Renderings/Blog/BlogArchives.cshtml", _blogService.Archives());
        }
    }
}