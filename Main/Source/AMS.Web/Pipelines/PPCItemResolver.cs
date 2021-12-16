using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Pipelines.HttpRequest;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web;
using AMS.Web.Core;
using AMS.Web.Services;

namespace AMS.Web.Pipelines
{
    public class PPCItemResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var branchName = WebUtil.GetQueryString(PPC.BranchReferenceQueryStringParameter);

            /*In Case Sitecore has mapped the item, do not do anything and simply return*/
            if (string.IsNullOrEmpty(branchName) || Context.Item != null || Context.Database == null || args.Url.ItemPath.Length == 0)
                return;

            var ppcItem = ContentService.GetPPCItemForBranch(branchName);

            if (ppcItem == null)
                return;

            var pageName = args.LocalPath.Replace("/", "");
            while (pageName.IndexOf("_") != -1)
                pageName = pageName.Replace("_", " ");

            var pageItem = ppcItem.Children.FirstOrDefault(c => string.Equals(c.Name, pageName, StringComparison.InvariantCultureIgnoreCase) && c.TemplateName == TemplateNames.PPC_TEMPLATE_NAME);

            if (pageItem != null)
                Context.Item = pageItem;
        }
    }
}