using System.Web.Optimization;

namespace Bs.Calendar.Mvc.Server
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/modernizr-2.6.2.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Gumby").Include(
                "~/Scripts/Gumby/gumby.min.js"));

            var scripts = new Bundle("~/Scripts/all", new JsMinify());
            scripts.Include("~/Scripts/Rooms/Utilities.js");
            scripts.Include("~/Scripts/Rooms/ColorPicker.js");
            scripts.Include("~/Scripts/Rooms/SetupEvents.js");

            scripts.Include("~/Scripts/Users/user.js");
            BundleTable.Bundles.Add(scripts);

            var css = new Bundle("~/Content/css", new CssMinify());
            css.Include("~/Content/css/gumby.css");
            css.Include("~/Content/css/style.css");
            css.Include("~/Content/ColorPicker.css");
            css.Include("~/Content/Layout/layout.css");
            BundleTable.Bundles.Add(css);
        }
    }
}