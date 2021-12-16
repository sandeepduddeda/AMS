using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Models
{
    public class QuoteRequest
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string MoveDate { get; set; }
        public string MoveSize1 { get; set; }
        public string MoveSize2 { get; set; }
        public string OriginZip { get; set; }
        public string DestinationZip { get; set; }
        public string MoveType { get; set; }
        public bool CanWeText { get; set; }

        public string PPCBranchRef { get; set; }
        public string PPCBranchSrc { get; set; }
        public string PromoCode { get; set; }
        public string GetMoveSize()
        {
            if ("Office".Equals(MoveSize1))
                return string.Format("{1} {0}", MoveSize1, MoveSize2);

            if ("Other".Equals(MoveSize1))
                return MoveSize2;

            return string.Format("{0} {1}", MoveSize1, MoveSize2);
        }

        public string GetFirstName()
        {
            return string.IsNullOrWhiteSpace(FullName)
                ? "N/A"
                : FullName.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        public string GetLastName()
        {
            return string.IsNullOrWhiteSpace(FullName)
                ? "N/A"
                : FullName.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }
    }
}