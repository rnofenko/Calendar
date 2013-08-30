using System.Web.Optimization;

namespace Bs.Calendar.Mvc.Server
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-1.10.3.min.js",
                "~/Scripts/modernizr-2.6.2.js",
                "~/Scripts/modernizr-2.6.2.min.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.timepicker/jquery.timepicker.min.js",
                "~/Scripts/jquery.scrollbar/jquery.scrollbar.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/Gumby").Include(
                "~/Scripts/Gumby/gumby.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Moment").Include(
                "~/Scripts/Moment/moment.min.js",
                "~/Scripts/Moment/MomentExtensions.js"));

            bundles.Add(new ScriptBundle("~/bundles/Mediator").Include(
                "~/Scripts/Mediator/Mediator.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/Knockout").Include(
                "~/Scripts/Knockout/knockout-2.3.0.js",
                "~/Scripts/Knockout/knockout.mapping-latest.js"));
                
            var scripts = new Bundle("~/Scripts/all", new JsMinify());
            scripts.Include("~/Scripts/Shared/MediatorSetup.js");
            scripts.Include("~/Scripts/Shared/UserColumnList.js");
            scripts.Include("~/Scripts/Shared/SimpleTeamList.js");
            scripts.Include("~/Scripts/Shared/RoomOrderList.js");
            scripts.Include("~/Scripts/Layout/list-view.js");
            scripts.Include("~/Scripts/Rooms/SetupEvents.js");
            scripts.Include("~/Scripts/Rooms/ColorPicker.js");
            scripts.Include("~/Scripts/Rooms/RoomListVm.js");
            scripts.Include("~/Scripts/Teams/TeamFrameFilter.js");
            scripts.Include("~/Scripts/Teams/TeamEdit.js");
            scripts.Include("~/Scripts/Teams/TeamList.js");
            scripts.Include("~/Scripts/Users/user.js");
            scripts.Include("~/Scripts/Users/UserList.js");
            scripts.Include("~/Scripts/Users/UserFrameFilter.js");
            scripts.Include("~/Scripts/Home/Calendar.js");
            scripts.Include("~/Scripts/Home/Month.js");
            scripts.Include("~/Scripts/Home/Week.js");
            scripts.Include("~/Scripts/Home/DayTimeBlock.js");
            scripts.Include("~/Scripts/Event/CalendarEvent.js");
            BundleTable.Bundles.Add(scripts);

            var css = new Bundle("~/Content/css", new CssMinify());
            css.Include("~/Content/");
            css.Include("~/Content/gumby/gumby.css");
            css.Include("~/Content/jquery.timepicker/jquery.timepicker.css");
            css.Include("~/Content/Shared/UserContacts.css");
            css.Include("~/Content/Shared/UserColumnList.css");
            css.Include("~/Content/Shared/SimpleTeamList.css");
            css.Include("~/Content/Shared/RoomOrderList.css");
            css.Include("~/Content/Rooms/ColorPicker.css");
            css.Include("~/Content/Users/RoleStateFilterWindow.css");
            css.Include("~/Content/Layout/layout.css");
            css.Include("~/Content/Teams/TeamEdit.css");
            css.Include("~/Content/Event/CreateEvent.css");
            css.Include("~/Content/Event/CalendarEvent.css");
            css.Include("~/Content/Event/SliderHelper.css");
            css.Include("~/Content/Calendar/Calendar.css");
            css.Include("~/Content/Calendar/CalendarWeek.css");
            css.Include("~/Content/jquery.scrollbar/jquery.scrollbar.css");
            css.Include("~/Content/themes/jquery-ui-1.10.3.custom.min.css");
            BundleTable.Bundles.Add(css);
        }
    }
}