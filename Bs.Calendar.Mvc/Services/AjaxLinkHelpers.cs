using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public static class AjaxLinkHelpers
    
    {
        public static MvcHtmlString AjaxSortLink(this HtmlHelper html, UsersVm usersVm, string sortByStr)
        {
            var ajaxHelper = new AjaxHelper(html.ViewContext, html.ViewDataContainer);

            var link = AjaxExtensions.ActionLink(
                ajaxHelper,
                sortByStr,
                "List",
                new {searchStr = usersVm.SearchStr, sortByStr = sortByStr, page = usersVm.CurrentPage},
                new AjaxOptions {UpdateTargetId = "user-table"}
                );

            return link;
        }

        public static MvcHtmlString AjaxPageLinks(this HtmlHelper html, UsersVm usersVm)
        {
            var pages = new StringBuilder();
            var ajaxHelper = new AjaxHelper(html.ViewContext, html.ViewDataContainer);

            for (var i = 1; i <= usersVm.TotalPages; i++)
            {
                var link = AjaxExtensions.ActionLink(
                    ajaxHelper,
                    i.ToString(),
                    "List",
                    new {searchStr = usersVm.SearchStr, sortByStr = usersVm.SortByStr, page = i},
                    new AjaxOptions {UpdateTargetId = "user-table"},
                    new {@class = i == usersVm.CurrentPage ? "primary badge" : ""}
                    );

                pages.Append(link);
            }
            
            return MvcHtmlString.Create(pages.ToString());
        }
    }
}