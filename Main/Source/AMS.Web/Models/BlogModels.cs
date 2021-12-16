using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Links;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class BlogList
    {
        public int ItemsPerPage { get; set; }
        public int? Page { get; set; }
        public IEnumerable<BlogPost> Blogs { get; set; }

        public string Category { get; set; }
        public int? Year { get; set; }        

        public int GetPage()
        {
            return Page.HasValue ? Page.Value : 0;
        }        
    }

    public class BlogPost
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostDate { get; set; }
        public IEnumerable<BlogCategory> Categories { get; set; }
    }

    public class BlogCategory
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }

    public class BlogArchive
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
}