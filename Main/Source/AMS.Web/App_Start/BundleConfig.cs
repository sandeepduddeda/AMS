using System.Web;
using System.Web.Optimization;

namespace AMS.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var jquery = new ScriptBundle("~/Content/AMS/allmysons/js/cdnjquery",
                "//ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js")
                .Include("~/Content/AMS/allmysons/js/jquery.min.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jquery);

            var bootstrap = new ScriptBundle("~/Content/AMS/allmysons/js/cdnbootstrap",
                "//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.3.5/js/bootstrap.min.js")
                .Include("~/Content/AMS/allmysons/js/bootstrap.js");
            bootstrap.CdnFallbackExpression = "$.fn.modal";
            bundles.Add(bootstrap);

            bundles.Add(new ScriptBundle("~/Content/AMS/allmysons/js/scripts").Include(
                      "~/Content/AMS/allmysons/js/select.js",
                      "~/Content/AMS/allmysons/js/smoothscroll.js",
                      "~/Content/AMS/allmysons/js/owl-carousel.js",
                      "~/Content/AMS/allmysons/js/wow.min.js",
                      "~/Content/AMS/allmysons/js/stellar.min.js",
//                      "~/Content/AMS/allmysons/js/html5lightbox.js",                      
                      "~/Content/AMS/allmysons/js/jquery-ui-1.11.4.custom.min.js",
                      "~/Content/AMS/allmysons/js/jquery.validate.min.js",
                      "~/Content/AMS/allmysons/js/jquery.maskedinput.min.js",
//                      "~/Content/AMS/allmysons/js/html5lightbox.js",
                      "~/Content/AMS/allmysons/js/slick.min.js",
                      "~/Content/AMS/allmysons/js/masonry.pkgd.min.js"
//                      "~/Content/AMS/allmysons/js/myscript.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/AMS/allmysons/css/styles").Include(
                      "~/Content/AMS/allmysons/css/bootstrap.css",
//                      "~/Content/AMS/allmysons/css/style.css",
//                      "~/Content/AMS/allmysons/css/responsive.css",
                      "~/Content/AMS/allmysons/css/animate.css",
                      "~/Content/AMS/allmysons/css/owl.css",
                      "~/Content/AMS/allmysons/css/owl.theme.css",
                      "~/Content/AMS/allmysons/css/font-awesome.css",
                      "~/Content/AMS/allmysons/css/datepicker.css",
                      "~/Content/AMS/allmysons/css/onoffswitch.css",
                      "~/Content/AMS/allmysons/css/slick.css"
                      ));

        }
    }
}
