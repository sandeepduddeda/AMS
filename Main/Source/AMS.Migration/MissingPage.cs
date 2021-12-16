using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMS.Migration
{
    public class MissingPage
    {
        public string ItemGuid { get; set; }
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }

        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }

        public string Body { get; set; }
    }
}
