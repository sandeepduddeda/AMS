using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AMS.Web.Extensions
{
    public class ExecutionTimeFilterAttribute : ActionFilterAttribute
    {
        Stopwatch stopWatch;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            stopWatch.Stop();
            Log(filterContext.RouteData, stopWatch.ElapsedMilliseconds);
        }
        private void Log(RouteData routeData, long time)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            Sitecore.Diagnostics.Log.Info("Controller: "+controllerName + ",Action: "+actionName+", Executed Time :"+time+ " in Milliseconds", this);
        }
    }
}