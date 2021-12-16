using AMS.Web.Core;
using AMS.Web.Models;
using AMS.Web.Services;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AMS.Web.Extensions;

namespace AMS.Web.Controllers
{
    [ExecutionTimeFilter]
    public class ContentController : BaseController
    {
        public ActionResult Banner()
        {
            return View("~/Views/AMS/allmysons/Renderings/Banner.cshtml", new ContentService().GetBanner());
        }

        public ActionResult BannerWithForm()
        {
            return View("~/Views/AMS/allmysons/Renderings/BannerWithForm.cshtml", new ContentService().GetBanner());
        }

        public ActionResult HalfBanner()
        {
            return View("~/Views/AMS/allmysons/Renderings/HalfBanner.cshtml", new ContentService().GetBanner());
        }

        public ActionResult RelatedLocations()
        {
            return View("~/Views/AMS/allmysons/Renderings/Branch/RelatedLocations.cshtml", new ContentService().GetRelatedLocations());
        }

        public ActionResult RequestQuote(QuoteRequest model)
        {
            string customerId = null;
            string promo = model.PromoCode != null ? model.PromoCode.ToString().ToLower() : string.Empty;
            string source = GetSource(model);
            string Move_Type = model.MoveType;
            try
            {
                using (var service = new AMS.Web.Service.AMSLeadImportSoapClient())
                {
                    var clientIpAddr = GetClientIPAddress();
                    Sitecore.Diagnostics.Log.Info(string.Format("Submitting Quote Request: First Name:{0}, LastName:{1},Phone:{2}, Email:{3}, MoveDate:{4}, MoveSize:{5} OrizinZip:{6}, Source:{7}, MoveType:{8}, APIReferenceID:{9}, DestinationZip:{10}, CanWeText:{11}, clientIpAddr:{12}",
                        model.GetFirstName(), model.GetLastName(), model.Phone, model.Email, model.MoveDate, model.GetMoveSize(), model.OriginZip, source, model.MoveType, 5, model.DestinationZip, model.CanWeText, clientIpAddr), this);

                    customerId = service.ImportLead06(
                        FirstName: model.GetFirstName(),
                        LastName: model.GetLastName(),
                        Telephone: model.Phone,
                        Email: model.Email,
                        MoveDate: model.MoveDate,
                        MoveSize: model.GetMoveSize(),
                        OriginZip: model.OriginZip,
                        Src: source,
                        MoveType: Move_Type,
                        APIReferenceID: 5,
                        DestinationZip: model.DestinationZip,
                        CanWeText: model.CanWeText,
                        ClientIPAddress: clientIpAddr,
                        PromoCode: promo);
                }

                TempData["CustomerId"] = customerId;
                TempData["PPCConversionScript"] = GetPPCConversionScript(model.PPCBranchRef, model.PPCBranchSrc);

                string thankYouPageURL = "/ams_quote/ams_thanks.aspx";
                if (Sitecore.Context.Item.ID.ToString() == "{3B32AC4C-5773-46D0-92F4-6FB86D39F56A}")
                    thankYouPageURL = "/mobile1/ams_quote/ams_thanks.aspx";
                else if (Sitecore.Context.Item.ID.ToString() == "{0464B7C8-D6C7-4A6F-AD79-8FA97F711951}")
                    thankYouPageURL = "/mobile2/ams_quote/ams_thanks.aspx";

                return Redirect((ContentService.IsPPC() ? "/quote/thankyou.aspx" : thankYouPageURL) + ContentService.GetPPCQueryString(model.PPCBranchRef, model.PPCBranchSrc));
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                Sitecore.Diagnostics.Log.Error("Quote error :" + ex.Message, this);
                return Redirect((ContentService.IsPPC() ? "/quote.aspx" : "/ams_quote.aspx") + ContentService.GetPPCQueryString(model.PPCBranchRef, model.PPCBranchSrc));
            }
        }

        public ActionResult GridReviews(string branch, int pageNum = 1, int pageSize = 15)
        {
            return View("~/Views/AMS/allmysons/Renderings/GridReviews.cshtml", new HomeService().GetReviews(branch, pageNum, pageSize));
        }

        private string GetPPCConversionScript(string branchName, string branchSrc)
        {
            try
            {
                var ppcItem = ContentService.GetPPCItemForBranch(branchName);

                if (ppcItem == null)
                    return null;

                return ppcItem["PPC " + branchSrc + " Conversion Script"];
            }
            catch(Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unrecognized PPC source", ex, this);
                return null;
            }
        }

        private string GetClientIPAddress()
        {
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }

        private string GetSource(QuoteRequest model)
        {
            string source = "AMSMainLead";
            string promo = model.PromoCode != null ? model.PromoCode.ToString().ToLower() : string.Empty;
            string BrachSrc = !string.IsNullOrWhiteSpace(Request.QueryString["BranchSrc"]) ? Request.QueryString["BranchSrc"] : !string.IsNullOrEmpty((HttpUtility.ParseQueryString(Request.UrlReferrer.Query))["BranchSrc"]) ? (HttpUtility.ParseQueryString(Request.UrlReferrer.Query))["BranchSrc"] : string.Empty;
            Sitecore.Diagnostics.Log.Info("context ID : " + Sitecore.Context.Item.ID.ToString(), this);
            
            if (!string.IsNullOrWhiteSpace(model.PPCBranchSrc))
                source = model.PPCBranchSrc;
            else if (!string.IsNullOrWhiteSpace(Request.QueryString["utm_source"]))
                source = Request.QueryString["utm_source"];
            else if (Sitecore.Context.Item.TemplateName.ToString() == TemplateNames.BRANCH_TEMPLATE_NAME)
                source = "AMSLocalLead";
            else if (Request.Browser.IsMobileDevice)
                source = (Sitecore.Context.Item.TemplateName.ToString() == TemplateNames.BRANCH_TEMPLATE_NAME) ? "MobileAMSLocalLead" : "MobileAMSMainLead";
            else if (!string.IsNullOrWhiteSpace(Request.QueryString["src"]))
                source = Request.QueryString["src"];
            else if (!string.IsNullOrWhiteSpace(BrachSrc))
                source = BrachSrc;
            else if (Request.Cookies["ams_src"] != null && !string.IsNullOrWhiteSpace(Request.Cookies["ams_src"].Value))
                source = Request.Cookies["ams_src"].Value;
            else if (promo != string.Empty && promo == "smartchoice")
                source = "Sinclair OTT";

            if (Sitecore.Context.Item.ID.ToString() == "{3B32AC4C-5773-46D0-92F4-6FB86D39F56A}")
                source = "AMSMobile";
            else if (Sitecore.Context.Item.ID.ToString() == "{0464B7C8-D6C7-4A6F-AD79-8FA97F711951}")
                source = "AMSMobile2";
            return source;
        }
    }
}