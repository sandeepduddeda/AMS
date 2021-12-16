using AMS.Web.Core;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Links;
using System;
using System.Web.Mvc;

namespace AMS.Web.Controllers
{
    public class BaseController : Controller
    {
        private const string ExceptionDataKey = "AMS Exception";
        protected const string ErrorMessage = "<div class=\"row\">Error</div>";
        protected const string ErrorFormat = "<div class=\"row\">{0}</div>";

        protected override void ExecuteCore()
        {
            try
            {
                base.ExecuteCore();
            }
            catch (Exception ex)
            {
                var data = new SitecoreErrorData(ex);

                TempData[ExceptionDataKey] = data;

                LogError(data);

                ActionInvoker.InvokeAction(ControllerContext, "RenderErrorResult");
            }
        }

        public ActionResult RenderErrorResult()
        {
            if (Context.PageMode.IsPreview || Context.PageMode.IsNormal)

                return new EmptyResult();

            var data = TempData[ExceptionDataKey] as SitecoreErrorData;

            if (data != null)
                return View("~/Views/AMS/allmysons/Shared/_Error.cshtml", data);

            return new EmptyResult();
        }

        private void LogError(SitecoreErrorData errorData)
        {
            if (errorData == null) throw new ArgumentNullException("errorData");
            if (errorData.ID == null) throw new NullReferenceException("Exception ID is not set");
            if (errorData.Message == null) throw new NullReferenceException("Exception Message is not set");

            if (errorData.SourceException != null)
                Log.Error(errorData.Message, errorData.SourceException, this);
            else
                Log.Error(errorData.Message, this);

            if (Context.Item != null)
                Log.Error(string.Format("page url {0}", LinkManager.GetItemUrl(Context.Item)), this);
        }
    }
}