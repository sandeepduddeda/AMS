using AMS.Web.Models;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using System.Web;
using AMS.Web.Extensions;
using Sitecore;
using Sitecore.Data.Items;
using System.Linq;
using AMS.Web.Core;
using Sitecore.Web;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace AMS.Web.Services
{
    public class ContentService
    {

        public BannerModel GetBanner()
        {
            var item = Context.Item;
            var renderingParams = string.Empty;

            // @todo Sitecore.Context.Item.TemplateID == Sitecore.Context.Item.Parent.TemplateID
            if (AreAllFieldsEmpty(new[] { FieldNames.FIELD_NAME_BANNER_TITLE, FieldNames.FIELD_NAME_BANNER_SUBTITLE, FieldNames.FIELD_NAME_BANNER_IMAGE }))
            {
                item = Context.Item.Parent;
                renderingParams = "disable-web-editing=true";
            }

            var model = new BannerModel
            {
                Title = new HtmlString(FieldRenderer.Render(item, FieldNames.FIELD_NAME_BANNER_TITLE, renderingParams)),
                Subtitle = new HtmlString(FieldRenderer.Render(item, FieldNames.FIELD_NAME_BANNER_SUBTITLE, renderingParams)),
                RequestAQuoteUrl = (IsPPC() ? "/quote.aspx" : "/ams_quote.aspx") + GetPPCQueryString()
            };

            var imgField = ((ImageField)item.Fields[FieldNames.FIELD_NAME_BANNER_IMAGE]);
            if (imgField != null && imgField.MediaItem != null)
            {
                model.ImageUrl = imgField.MediaItem.GetMediaUrl(maxWidth: 1280);
                model.ImageAlt = imgField.Alt;
            }
            if (HttpContext.Current.Request.Browser.IsMobileDevice)
            {

            }

            return model;
        }

        public LocationsModel GetRelatedLocations()
        {
            var item = Context.Item;
            //Checking current page is CITY Page or Not
            if (item.TemplateID.ToString() == TemplateIds.TEMPLATE_ID_CITY_PAGE)
            {
                //Retrieving Branch of Current City page
                ReferenceField field = item.Fields["Branch Of"];
                if (field != null || field.TargetItem != null)
                {
                    //if City page has Branch then we need to show Branch relation locations
                    item = field.TargetItem;
                }
            }
            MultilistField relatedLocationsField = item.Fields[FieldNames.FIELD_NAME_RELATED_LOCATIONS];

            if (relatedLocationsField != null && relatedLocationsField.Count == 0)
            {
                relatedLocationsField = item.Parent.Fields[FieldNames.FIELD_NAME_RELATED_LOCATIONS];
            }

            return new LocationsModel
            {
                Items = relatedLocationsField.GetItems().OrderBy(i => i.DisplayName).ToList()
            };
        }

        public PPCModel GetPPCContent()
        {
            var branchName = WebUtil.GetQueryString(PPC.BranchReferenceQueryStringParameter);
            var ppcItem = ContentService.GetPPCItemForBranch(branchName);
            var currentItem = ppcItem != null ? ppcItem : Context.Item;

            var model = new PPCModel();

            string phoneFieldId = string.Equals(WebUtil.GetQueryString(PPC.BranchSourceQueryStringParameter), PPC.BingBranchSourceQueryStringValue, StringComparison.InvariantCultureIgnoreCase)
                ? FieldIds.FIELD_ID_PPC_BING_PHONE
                : FieldIds.FIELD_ID_PPC_PHONE;
            model.PhoneRaw = currentItem[phoneFieldId];
            model.Phone = new HtmlString(currentItem[phoneFieldId]);
            model.Footer = new HtmlString(currentItem[FieldIds.FIELD_ID_PPC_FOOTER]);

            var copyrightItem = Context.Database.GetItem(AMS.Web.Core.ItemPaths.PATH_FOOTER_COPYRIGHT);
            string copyright = copyrightItem != null ? copyrightItem[FieldNames.FIELD_NAME_CONTENT] : null;
            model.Copyright = new HtmlString(copyright);

            return model;
        }

        /// <summary>
        /// returns corresponding PPC item for a branch
        /// </summary>
        /// <param name="branchName"></param>
        /// <returns></returns>
        public static Item GetPPCItemForBranch(string branchName)
        {
            var homeItem = Context.Database.GetItem(ItemPaths.PATH_HOME);
            var branchItem = homeItem.Children.FirstOrDefault(c => string.Equals(c.Name, branchName, StringComparison.InvariantCultureIgnoreCase)
                    && (c.TemplateName == TemplateNames.BRANCH_TEMPLATE_NAME || c.TemplateName == TemplateNames.CITY_TEMPLATE_NAME));
            var ppcItem = branchItem == null ? null : branchItem.Children.FirstOrDefault(c => c.TemplateName == TemplateNames.BRANCH_PPC_TEMPLATE_NAME);

            return ppcItem;
        }

        public static string GetPPCQueryString(string branchName = null, string branchSrc = null)
        {
            var branch = branchName ?? WebUtil.GetQueryString(PPC.BranchReferenceQueryStringParameter);
            var src = branchSrc ?? WebUtil.GetQueryString(PPC.BranchSourceQueryStringParameter);

            var query = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(branch))
                query.Add(PPC.BranchReferenceQueryStringParameter, branch);

            if (!string.IsNullOrEmpty(src))
                query.Add(PPC.BranchSourceQueryStringParameter, src);

            var queryString = string.Join("&", query.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)));
            
            return string.IsNullOrEmpty(queryString) ? string.Empty : "?" + queryString;
        }

        public static bool IsPPC()
        {
            var uri = WebUtil.GetRequestUri();
            return uri.Authority.IndexOf("landingtab") > -1;
        }

        private bool AreAllFieldsEmpty(string[] fields)
        {
            foreach (var fieldName in fields)
            {
                if (Context.Item.Fields[fieldName] != null && Context.Item.Fields[fieldName].Value != "")
                    return false;
            }

            return true;
        }
    }
}