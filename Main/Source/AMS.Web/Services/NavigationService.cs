using AMS.Web.Core;
using AMS.Web.Extensions;
using AMS.Web.Models;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Diagnostics;
using System.Net;
using System.Collections.Specialized;
using System.Text;

namespace AMS.Web.Services
{
    public class NavigationService
    {
        public MenuModel GetMenu()
        {
            var currentItem = Sitecore.Context.Item;
            Item itemWithMenu = null;
            Item itemWithAddress = null;

            var model = new MenuModel();

            // check explicit menu items in current item and in parent
            if (currentItem.Fields[FieldNames.FIELD_NAME_MENU_ITEMS] != null)
            {
                itemWithMenu = GetItemOrAncestorWithValueInField(currentItem, FieldNames.FIELD_NAME_MENU_ITEMS);
            }

            if (itemWithMenu != null)
            {
                MultilistField items = itemWithMenu.Fields[FieldNames.FIELD_NAME_MENU_ITEMS];
                model.Items = items.GetItems().ToList();
            }
            // if no explicit menu items given then show children or siblings
            else
            {
                if (IsNavItemTemplate(currentItem.TemplateName))
                {
                    model.Items = currentItem.Children.Where(i => IsNavItemTemplate(i.TemplateName)).ToList();
                    if(model.Items.Count == 0)
                    {
                        if (IsNavItemTemplate(currentItem.Parent.TemplateName))
                        {
                            model.Items = currentItem.Parent.Children.Where(i => IsNavItemTemplate(i.TemplateName)).ToList();
                        }
                    }
                    model.Items.Insert(0, currentItem);
                }
                else if(IsNavItemTemplate(currentItem.Parent.TemplateName))
                {
                    model.Items = currentItem.Parent.Children.Where(i => IsNavItemTemplate(i.TemplateName)).ToList();
                    model.Items.Insert(0, currentItem.Parent);
                }

                 
            }

            // if item has a field
            itemWithAddress = GetItemOrAncestorWithValueInField(currentItem, FieldNames.FIELD_NAME_CORPORATE_NAME);

            if (itemWithAddress != null)
            {
                // if values are taken from a parent item then render a field in read only
                var renderingParams = currentItem.Equals(itemWithAddress) ? string.Empty : "disable-web-editing=true";

                model.Address = new AddressModel
                {
                    CorporateName = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_CORPORATE_NAME, renderingParams)),
                    Street = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_STREET, renderingParams)),
                    Street2 = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_STREET2, renderingParams)),
                    City = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_CITY, renderingParams)),
                    State = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_STATE, renderingParams)),
                    Zip = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_ZIP, renderingParams)),
                    Tel = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_TEL, renderingParams)),
                    Fax = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_FAX, renderingParams)),
                    AdditionalInfo = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_ADDITIONAL_INFO, renderingParams)),
                    DBA = new HtmlString(FieldRenderer.Render(itemWithAddress, FieldNames.FIELD_NAME_DBA, renderingParams)),
                };

            }

            return model;
        }

        public static bool IsNavItemTemplate(string templateName)
        {
            return templateName == TemplateNames.BRANCH_TEMPLATE_NAME || templateName == TemplateNames.MOVING_ARTICLES_TEMPLATE_NAME || templateName == TemplateNames.CITY_TEMPLATE_NAME ;
        }

        public HeaderModel GetHeader()
        {
            var dataItem = !string.IsNullOrWhiteSpace(Sitecore.Context.Item[FieldNames.FIELD_NAME_PHONE])
               ? Sitecore.Context.Item
               : Sitecore.Context.Item.Parent;
            string branch = this.GetCurrentBranchName() ?? "National";
            Log.Info("Rating and count info : Branch Name: " + branch , this);
            float rating = this.GetNationalRating(branch);
            Log.Info("Rating : " + rating , this);           
            int count = (double)rating < 4.0 ? this.GetNationalRatingCount("National") : this.GetNationalRatingCount(branch);
            rating = rating < 4 ? GetNationalRating("National") : rating;
            string branchPhoneNumber = this.GetBranchPhoneNumber();
            var model = new HeaderModel
            {
                Phone = branchPhoneNumber != string.Empty ? new HtmlString(branchPhoneNumber.ToString()) : this.GetPhoneNumber(),
                Title = new HtmlString(FieldRenderer.Render(Context.Item, FieldNames.FIELD_NAME_TITLE, "disable-web-editing=true")),
                RatingText = "Rating",
                RatingValue = rating,
                RatingCount = count
            };
            return model;

        }

        protected class RatingRequest
        {
            public string branchName { get; set; }
            public int Year { get; set; }
        }

        private float GetNationalRating(string branch)
        {
            try
            {
                var rating = CacheService.Instance.Get("NationalRating_" + branch, () =>
                {
                    using (var client = new WebClient())
                    {
                        var data = new NameValueCollection();
                        data.Add("branchName", branch);
                        data.Add("Year", DateTime.Now.Year.ToString());

                        var result = client.UploadValues(Config.CustomerServiceAppUrl, data);

                        return float.Parse(Encoding.UTF8.GetString(result));
                    }
                });

                return rating;
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching National Rating", ex, this);
            }

            return 0f;
        }

        public FooterModel GetFooter()
        {
            var model = new FooterModel
            {
                Top = new HtmlString(FieldRenderer.Render(Sitecore.Context.Database.GetItem(ItemPaths.PATH_FOOTER_TOP), FieldNames.FIELD_NAME_CONTENT, "disable-web-editing=true")),
                Copyright = new HtmlString(FieldRenderer.Render(Sitecore.Context.Database.GetItem(ItemPaths.PATH_FOOTER_COPYRIGHT), FieldNames.FIELD_NAME_CONTENT, "disable-web-editing=true")),
                LinkItems = Sitecore.Context.Database.GetItem(ItemPaths.PATH_FOOTER_LINKS).Children.ToList()
            };

            return model;
        }

        public static IList<Item> GetBreadcrumbs()
        {
            var item = Sitecore.Context.Item;

            // by default, get the item assuming Presentation Preview tool (embedded preview in shell)
            if (Context.PageMode.IsPageEditor || Context.PageMode.IsPreview)
            {
                // item ID for page editor and front-end preview mode
                string id = Sitecore.Web.WebUtil.GetQueryString("sc_itemid");

                // if a query string ID was found, get the item for page editor and front-end preview mode
                if (!string.IsNullOrEmpty(id))
                {
                    item = Sitecore.Context.Database.GetItem(id);
                }
            }

            var home = ItemExtensions.GetHomePage();

            var list = new List<Item>();

            if (item == null || item.ID == home.ID) return list.AsReadOnly();

            var ancestors = item.Axes.GetAncestors();

            foreach (var i in ancestors)
            {
                if (i.ID == home.ID || i.Paths.IsDescendantOf(home))
                    list.Add(i);
            }

            list.Add(item);

            return list.AsReadOnly();
        }

        public PPCModel GetPPCHeader()
        {
            return null;
        }

        public string GetCurrentBranchName()
        {
            var item = Sitecore.Context.Item;

            ///Checking template details
            bool isBranchTemplate = item.TemplateName == TemplateNames.BRANCH_TEMPLATE_NAME;
            bool isCityTemplate = item.TemplateName == TemplateNames.CITY_TEMPLATE_NAME;
            bool isParentIsCity = item.Parent.TemplateName == TemplateNames.CITY_TEMPLATE_NAME;
            bool isHomeTemplate = item.TemplateID == TemplateIds.TEMPLATE_ID_HOMEPAGE;

            if (isHomeTemplate)
            {
                return null;
            }
            if (!isCityTemplate)
            {
                if (!isParentIsCity)
                {
                    return item.Parent.TemplateID == TemplateIds.TEMPLATE_ID_HOMEPAGE ? item.Name : item.Parent.Name;
                }
                else
                {
                    return GetBranchNameFromRelatedLocations(item.Parent);
                }
            }
            else if (isCityTemplate)
            {
                return GetBranchNameFromRelatedLocations(item);
            }

            // try to get the value from ancestors
            var ancestors = item.Axes.GetAncestors();
            Array.Reverse(ancestors);

            foreach (var i in ancestors)
            {
                if (i.TemplateName == TemplateNames.BRANCH_TEMPLATE_NAME)
                    return item.Name;

                if (i.TemplateName == TemplateNames.CITY_TEMPLATE_NAME)
                    return GetBranchNameFromRelatedLocations(i);
            }

            return null;
        }

        private string GetBranchNameFromRelatedLocations(Item item)
        {
            MultilistField relatedLocations = item.Fields[FieldNames.FIELD_NAME_RELATED_LOCATIONS];

            if (relatedLocations != null && relatedLocations.Count == 0)
            {
                relatedLocations = item.Parent.Fields[FieldNames.FIELD_NAME_RELATED_LOCATIONS];
            }

            var branchItem = relatedLocations.GetItems().FirstOrDefault(l => l.TemplateName == TemplateNames.BRANCH_TEMPLATE_NAME);

            if (branchItem != null)
                return branchItem.Name;

            return null;
        }

        private Item GetItemOrAncestorWithValueInField(Item item, string fieldName)
        {
            // try to get the value from the item
            if (item.Fields[fieldName] != null && !string.IsNullOrWhiteSpace(item[fieldName]))
                return item;

            // try to get the value from ancestors
            var ancestors = item.Axes.GetAncestors();
            Array.Reverse(ancestors);

            foreach (var i in ancestors)
            {
                if (i.Fields[fieldName] != null && !string.IsNullOrWhiteSpace(i[fieldName]))
                    return i;
            }

            return null;
        }

        private int GetNationalRatingCount(string branch)
        {
            try
            {
                var rating = CacheService.Instance.Get("NationalRating_" + branch, () =>
                {
                    using (var client = new WebClient())
                    {
                        var data = new NameValueCollection();
                        data.Add("branchName", branch);
                        data.Add("Year", "0");

                        var result = client.UploadValues(Config.CustomerServiceAppCountUrl, data);

                        return int.Parse(Encoding.UTF8.GetString(result));
                    }
                });

                return rating;
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching National Rating", ex, this);
            }

            return 0;

        }

        private HtmlString GetPhoneNumber()
        {
            HtmlString phoneNumber = null;
            var dataItem = !string.IsNullOrWhiteSpace(Sitecore.Context.Item[FieldNames.FIELD_NAME_PHONE])
               ? Sitecore.Context.Item
               : Sitecore.Context.Item.Parent;
            if ((Context.Item.TemplateName.ToString() == TemplateNames.Article_Page || Context.Item.TemplateName.ToString() == TemplateNames.MOVING_ARTICLES_TEMPLATE_NAME) && Context.Item.Parent.TemplateName.ToString() == TemplateNames.BRANCH_TEMPLATE_NAME && string.IsNullOrWhiteSpace(Sitecore.Context.Item[FieldNames.FIELD_NAME_PHONE]))
            {
                phoneNumber = new HtmlString(FieldRenderer.Render(dataItem, !string.IsNullOrWhiteSpace(dataItem[FieldNames.FIELD_NAME_ARTICLE_PHONE]) ? FieldNames.FIELD_NAME_ARTICLE_PHONE : FieldNames.FIELD_NAME_PHONE, Sitecore.Context.Item.Equals(dataItem) ? string.Empty : "disable-web-editing=true"));
            }
            else
            {
                phoneNumber = new HtmlString(FieldRenderer.Render(dataItem, FieldNames.FIELD_NAME_PHONE, Sitecore.Context.Item.Equals(dataItem) ? string.Empty : "disable-web-editing=true"));
            }

            return phoneNumber;
        }

        private string GetBranchPhoneNumber()
        {
            string BranchSrc, BranchRef, phoneNumber = string.Empty;
            try{
                Item[] items = Sitecore.Context.Database.SelectItems("/sitecore/content/AMS/allmysons/System/BranchSourceInfo/*");
                if (HttpContext.Current.Request.QueryString["phone"] != null){
                    string str = HttpContext.Current.Request.QueryString["phone"];
                    string[] chars = new string[] { "-", ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "(", ")", ":", "|", "[", "]" };
                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (str.Contains(chars[i])){
                            str = str.Replace(chars[i], "");
                        }
                    }
                    str = str.Insert(3, "-").Insert(7, "-");
                    return str;
                }
                if (HttpContext.Current.Request.QueryString["BranchSrc"] != null)
                {
                    BranchSrc = HttpContext.Current.Request.QueryString["BranchSrc"];
                    BranchRef = HttpContext.Current.Request.QueryString["BranchRef"];
                    if (BranchSrc != null && BranchRef != null)
                    {
                        Item contactItem = items.Where(x => x.Fields["BranchRef"].ToString().ToLower() == BranchRef.ToLower() && x.Fields["BranchSrc"].ToString().ToLower() == BranchSrc.ToLower()).FirstOrDefault();
                        phoneNumber = contactItem != null ? contactItem.Fields["Phone"].ToString() : string.Empty;
                    }
                }
                else if (HttpContext.Current.Request.UrlReferrer != null)
                {
                    NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query);
                    BranchSrc = nameValueCollection["BranchSrc"];
                    BranchRef = nameValueCollection["BranchRef"];
                    if (BranchSrc != null && BranchRef != null)
                    {
                        Item contactItem = items.Where(x => x.Fields["BranchRef"].ToString().ToLower() == BranchRef.ToLower() && x.Fields["BranchSrc"].ToString().ToLower() == BranchSrc.ToLower()).FirstOrDefault();
                        phoneNumber = contactItem != null ? contactItem.Fields["Phone"].ToString() : string.Empty;
                    }
                }

                return phoneNumber;
            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException.ToString(), this);

                return phoneNumber;
            }
        }
    }
}