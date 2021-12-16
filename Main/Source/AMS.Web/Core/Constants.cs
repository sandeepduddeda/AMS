using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMS.Web.Core
{
    public static class FieldNames
    {
        public static readonly string FIELD_NAME_MENU_ITEMS = "Menu Items";
        public static readonly string FIELD_NAME_CORPORATE_NAME = "Corporate Name";
        public static readonly string FIELD_NAME_STREET = "Street";
        public static readonly string FIELD_NAME_STREET2 = "Street 2";
        public static readonly string FIELD_NAME_CITY = "City";
        public static readonly string FIELD_NAME_STATE = "State";
        public static readonly string FIELD_NAME_ZIP = "Zip";
        public static readonly string FIELD_NAME_TEL = "Tel";
        public static readonly string FIELD_NAME_FAX = "Fax";
        public static readonly string FIELD_NAME_ADDITIONAL_INFO = "Additional Location Info";
        public static readonly string FIELD_NAME_DBA = "DBA";
        public static readonly string FIELD_NAME_CONTENT = "Content";
        public static readonly string FIELD_NAME_PHONE = "Phone";
        public static readonly string FIELD_NAME_TITLE = "Title";
        public static readonly string FIELD_NAME_SUBTITLE = "Subtitle";
        public static readonly string FIELD_NAME_ARTICLE_PHONE = "ArticlePhone";
        public static readonly string FIELD_NAME_BANNER_TITLE = "Banner Title";
        public static readonly string FIELD_NAME_BANNER_SUBTITLE = "Banner Subtitle";
        public static readonly string FIELD_NAME_BANNER_IMAGE = "Banner Image";
        public static readonly string FIELD_NAME_RELATED_LOCATIONS = "Related Locations";
        public static readonly string FIELD_VALUE = "Value";
        public static readonly string FIELD_POSTDATE = "Post Date";
        public static readonly string FIELD_BLOG_CATEGORY = "Blog Category";
    }
    public static class FieldIds
    {
        public static readonly string FIELD_ID_PPC_PHONE = "{570D61FB-DC77-476E-83CA-E081572C3D58}";
        public static readonly string FIELD_ID_PPC_BING_PHONE = "{D18E572B-6BD9-4FE8-BD04-A63C862EBC2E}";
        public static readonly string FIELD_ID_PPC_FOOTER = "{550D89C5-2F09-4155-ACE0-9BF8DF677E1E}";
    }
    public static class TemplateNames
    {
        public static readonly string BRANCH_TEMPLATE_NAME = "Branch Page";
        public static readonly string CITY_TEMPLATE_NAME = "City Page";
        public static readonly string MOVING_ARTICLES_TEMPLATE_NAME = "Moving Articles Page";
        public static readonly string PPC_TEMPLATE_NAME = "PPC Page";
        public static readonly string BRANCH_PPC_TEMPLATE_NAME = "Branch PPC";
        public static readonly string Article_Page = "Article Page";
    }
    public static class ItemPaths
    {
        public static readonly string PATH_HOME = "/sitecore/content/AMS/allmysons/Home";
        public static readonly string PATH_FOOTER_TOP = "/sitecore/content/AMS/allmysons/System/Footer/Top";
        public static readonly string PATH_FOOTER_COPYRIGHT = "/sitecore/content/AMS/allmysons/System/Footer/Copyright";
        public static readonly string PATH_FOOTER_LINKS = "/sitecore/content/AMS/allmysons/System/Footer/Links";
        public static readonly string PATH_QUOTE_FORM = "/sitecore/content/AMS/allmysons/System/Get a Quote Form";
        public static readonly string PATH_CAROUSEL_BLOCK = "/sitecore/content/AMS/allmysons/System/Carousel Block";
        public static readonly string PATH_QUOTE_FORM_FOR_NewForm = "/sitecore/content/AMS/allmysons/System/NewGetQuoteForm/Get a Quote Form New";
    }
    public static class TemplateIds
    {
        public static readonly string TEMPLATE_ID_BRANCH_PAGE = "{B99305F7-6700-4499-8989-96E87BB32E7D}";
        public static ID BLOG_POST_PAGE = new ID("{5694DD5A-8C72-4135-91FA-5280875B3868}");
        public static ID BLOG_HOME_PAGE = new ID("{823CB4D8-6D23-4634-A3B1-3B7DB8F943A4}");
        public static ID BLOG_ARCHIVE_PAGE = new ID("{34982A51-3BAC-4393-B1EB-131746010103}");
        public static ID BLOG_CATEGORY_PAGE = new ID("{E1395E00-548B-4480-B0F9-50090AE00A3D}");
        public static readonly string TEMPLATE_ID_CITY_PAGE = "{EE5AE0D2-02CF-4815-A2E3-8E861D60793E}";
        public static ID TEMPLATE_ID_HOMEPAGE = new ID("{94FCB934-7F10-4D66-B5B8-771862EFC423}");
    }
    public static class PPC
    {
        public static readonly string BranchReferenceQueryStringParameter = "BranchRef";
        public static readonly string BranchSourceQueryStringParameter = "BranchSrc";
        public static readonly string BingBranchSourceQueryStringValue = "Bing";
        public static readonly string GoogleBranchSourceQueryStringValue = "Google";
        //public static readonly string[] PageNames = new[] { "local_moves.aspx", "quick_quote.aspx", "out_of_state_moves.aspx", "same_day_next_day_moves.aspx" };
    }
    public static class Items
    {
        public static ID TermsAndConditions = new ID("{5CBE7C40-7DF0-4DB4-B2F3-FF5F055B6C00}");
        public static ID PrivacyPolicy = new ID("{0A513052-E195-4EC1-BCF2-93AD37112EA4}");
        public static ID BlogCategories = new ID("{93A3B194-55AF-4F04-9583-BDD17CB15E9B}");
    }
}