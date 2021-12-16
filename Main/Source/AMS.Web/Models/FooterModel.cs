using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class FooterModel
    {
        public HtmlString Top { get; set; }
        public HtmlString Copyright { get; set; }
        public IList<Item> LinkItems {get; set;}
        public AddressModel Address { get; set; }
    }
}