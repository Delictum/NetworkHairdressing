using System.Web;
using System.Web.Optimization;

namespace NetworkHairdressing
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.12.1.js",
                "~/Scripts/jquery-ui-timepicker-addon.min.js"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/themes/base/accordion.css",
                "~/Content/themes/base/all.css",
                "~/Content/themes/base/base.css",
                "~/Content/themes/base/button.css",
                "~/Content/themes/base/theme.css",
                "~/Content/themes/base/menu.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base").Include(
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/jquery-ui-timepicker-addon.min.css",
                "~/Content/jquery-ui-timepicker-addon.css"));
        }
    }
}
