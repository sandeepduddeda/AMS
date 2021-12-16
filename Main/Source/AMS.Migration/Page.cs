using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Migration
{
    public class Page
    {
        public Page()
        {
            Children = new List<Page>();
            ItemType = ItemType.Undefined;
        }

        public int PageId { get; set; }
        public int BranchId { get; set; }
        public string[] Paths { get; set; }

        public string ItemName { get; set; }
        public string ItemDisplayName { get; set; }
        public ItemType ItemType { get; set; } //
        public string ItemGuid { get; set; }
        public int SortOrder { get; set; }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Content { get; set; }

        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public string CanonicalUrl { get; set; } //
        public string GooglePublisherTag { get; set; }

        public string CorporateName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string AdditionalInfo { get; set; }

        public string LogoAlt { get; set; }

        public List<Page> Children { get; set; }
    }
}
