using System.Web.Optimization;

namespace Bs.Calendar.Mvc.Server
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            var scripts = new Bundle("~/Scripts/all", new JsMinify());
            scripts.Include("~/Scripts/Rooms/SetupEvents.js");

            scripts.Include("~/Scripts/Users/user.js");
            BundleTable.Bundles.Add(scripts);

            var css = new Bundle("~/Content/css", new CssMinify());
            css.Include("~/Content/gumby.css");
            css.Include("~/Content/Rooms/ColorPicker.css");
            BundleTable.Bundles.Add(css);
        }
    }
}