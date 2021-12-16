using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class AddressModel
    {
        public HtmlString CorporateName { get; set; }
        public HtmlString Street { get; set; }
        public HtmlString Street2 { get; set; }
        public HtmlString City { get; set; }
        public HtmlString State { get; set; }
        public HtmlString Zip { get; set; }
        public HtmlString Tel { get; set; }
        public HtmlString Fax { get; set; }
        public HtmlString AdditionalInfo { get; set; }
        public HtmlString DBA { get; set; }
        public HtmlString TollFreeNumber { get; set; }
        public bool IsTxDMV { get; set; }
    }
}