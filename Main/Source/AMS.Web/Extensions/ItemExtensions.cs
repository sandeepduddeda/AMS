using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Extensions
{
    public static class ItemExtensions
    {
        public static string GetItemUrl(this Item item)
        {
            if (item == null) throw new NullReferenceException();

            return LinkManager.GetItemUrl(item);
        }

        public static string GetMediaUrl(this Item item, int? maxHeight = null, int? maxWidth = null)
        {
            if (item == null) throw new NullReferenceException();

            var mediaUrlOptions = new MediaUrlOptions { UseItemPath = true, AbsolutePath = false };

            if (maxHeight != null) mediaUrlOptions.MaxHeight = maxHeight.Value;
            if (maxWidth != null) mediaUrlOptions.MaxWidth = maxWidth.Value;

            return MediaManager.GetMediaUrl(item, mediaUrlOptions);
        }

        public static Item GetHomePage()
        {
            if (Sitecore.Context.PageMode.IsPageEditor || Sitecore.Context.PageMode.IsPreview)
            {
                // item ID for page editor and front-end preview mode
                string id = Sitecore.Web.WebUtil.GetQueryString("sc_itemid");

                // by default, get the item assuming Presentation Preview tool (embedded preview in shell)
                var item = Sitecore.Context.Item;

                // if a query string ID was found, get the item for page editor and front-end preview mode
                if (!string.IsNullOrEmpty(id))
                {
                    item = Sitecore.Context.Database.GetItem(id);
                }

                // loop through all configured sites
                foreach (var site in Sitecore.Configuration.Factory.GetSiteInfoList())
                {
                    // get this site's home page item
                    var homePage = Sitecore.Context.Database.GetItem(site.RootPath + site.StartItem);

                    // if the item lives within this site, this is our context site
                    if (homePage != null && homePage.Axes.IsAncestorOf(item))
                    {
                        return homePage;
                    }
                }

                // fallback and assume context site
                return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath + Sitecore.Context.Site.StartItem);
            }
            else
            {
                // standard context site resolution via hostname, virtual/physical path, and port number
                return Sitecore.Context.Database.GetItem(Sitecore.Context.Site.RootPath + Sitecore.Context.Site.StartItem);
            }
        }
    }
}