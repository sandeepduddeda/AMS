using AMS.Web.Extensions;
using AMS.Web.Services;
using System.Web.Mvc;

namespace AMS.Web.Controllers
{
    [ExecutionTimeFilter]
    public class HomeController : BaseController
    {
        public ActionResult Locations()
        {
            return View("~/Views/AMS/allmysons/Renderings/Home/Locations.cshtml", new HomeService().GetLocations());
        }

        public ActionResult Reviews(string branch = null, int pageNum = 1, int pageSize = 15)
        {
            return View("~/Views/AMS/allmysons/Renderings/Home/Reviews.cshtml", new HomeService().GetReviews(branch, pageNum, pageSize));
        }

    }
}