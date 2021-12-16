using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class ReviewsListModel
    {
        public List<ReviewModel> Reviews { get; set; }
        public string BranchName { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }
    }

    public class ReviewModel
    {
        public string CustomerName { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewTitle { get; set; }
        public float Rating { get; set; }
    }

}