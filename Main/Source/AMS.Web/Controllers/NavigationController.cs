using AMS.Web.Extensions;
using AMS.Web.Services;
using System.Web.Mvc;

namespace AMS.Web.Controllers
{
    [ExecutionTimeFilter]
    public class NavigationController : BaseController
    {
        public ActionResult Menu()
        {
            return View("~/Views/AMS/allmysons/Navigation/Menu.cshtml", new NavigationService().GetMenu());
        }

        public ActionResult Header()
        {
            return View("~/Views/AMS/allmysons/Navigation/Header.cshtml", new NavigationService().GetHeader());
        }

        public ActionResult Footer()
        {
            return View("~/Views/AMS/allmysons/Navigation/Footer.cshtml", new NavigationService().GetFooter());
        }
    }
}