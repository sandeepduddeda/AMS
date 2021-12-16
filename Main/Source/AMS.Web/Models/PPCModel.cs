using System.Web;
namespace AMS.Web.Models
{
    public class PPCModel
    {
        public string PhoneRaw { get; set; }
        public HtmlString Phone { get; set; }
        public HtmlString Footer { get; set; }
        public HtmlString Copyright { get; set; }
    }
}