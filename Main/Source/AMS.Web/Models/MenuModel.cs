using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class MenuModel
    {
        public IList<Item> Items { get; set; }

        public AddressModel Address { get; set; }
    }
}