using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Pipelines
{
    public class ExecuteNotFoundRequest : ExecuteRequest
    {
        protected override void RedirectOnItemNotFound(string url)
        {
            HttpContext context = HttpContext.Current;

            try
            {
                string content = null;

                // Check if it is a recursive request
                if (!IsRecursiveRequest(url))
                {
                    // Request the NotFound page
                    string domain = context.Request.Url.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.Unescaped);
                    string notFoundUrl = string.Concat(domain, url);

                    content = WebUtil.ExecuteWebPage(notFoundUrl);
                }
                else
                {
                    content = "<html><body><center><h1>404 Page Not Found</h1></center></body></html>";
                }

                // Send the NotFound page content to the client with a 404 status code
                context.Response.TrySkipIisCustomErrors = true;
                context.Response.Status = "404 Not Found";
                context.Response.StatusCode = 404;
                context.Response.Write(content);

            }
            catch (Exception)
            {
                // If the sending has failed for any reason, fall back to the base method
                base.RedirectOnItemNotFound(url);
            }

            // Must be outside the try/catch because Response.End() throws an exception
            context.Response.End();
        }

        private bool IsRecursiveRequest(string url)
        {
            // Check the target the url
            int queryPos = url.IndexOf('?');

            if (queryPos >= 0)
            {
                string itemPath = url.Substring(0, queryPos);
                var parameters = HttpUtility.ParseQueryString(url.Substring(queryPos));

                // Check the item parameter in the query string
                string paramValue = parameters["item"];

                // If the not-found item equals the item parameter, this is a recursive request
                return (paramValue != null && paramValue == itemPath);
            }

            return false;
        }
    }
}