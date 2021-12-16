using AMS.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Providers
{
    public class AmsLinkProvider : Sitecore.Links.LinkProvider
    {
        public override string GetItemUrl(Sitecore.Data.Items.Item item, Sitecore.Links.UrlOptions urlOptions)
        {
            if (Sitecore.Context.Site.Name.StartsWith("allmysons"))
            {
                // @todo if page has children then replace URL with /index.aspx?
                urlOptions.SiteResolving = Sitecore.Configuration.Settings.Rendering.SiteResolving;
                var url = base.GetItemUrl(item, urlOptions).ToLower();

                return item.TemplateName == TemplateNames.BRANCH_TEMPLATE_NAME || item.TemplateName == TemplateNames.CITY_TEMPLATE_NAME /* item.HasChildren*/
                    ? url.Replace(".aspx", "/index.aspx")
                    : url;
            }

            return base.GetItemUrl(item, urlOptions);
        }
    }
}