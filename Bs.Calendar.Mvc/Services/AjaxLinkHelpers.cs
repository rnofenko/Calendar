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
            var linkText = sortByStr;
            
            //When clicked again show default unsorted view
            if (sortByStr.Equals(usersVm.SortByStr))
                sortByStr = null;

            var link = ajaxHelper.ActionLink(
                linkText,
                "List",
                new {searchStr = usersVm.SearchStr, sortByStr = sortByStr, page = usersVm.CurrentPage},
                new AjaxOptions {UpdateTargetId = "user-table"}
                );

            return link;
        }

        public static MvcHtmlString AjaxPageLink(this HtmlHelper html, UsersVm usersVm, string linkText, int pageNumber)
        {
            var ajaxHelper = new AjaxHelper(html.ViewContext, html.ViewDataContainer);

            if (pageNumber < 1 || pageNumber > usersVm.TotalPages)
                pageNumber = usersVm.CurrentPage;

            var link = ajaxHelper.ActionLink(
                linkText,
                "List",
                new {searchStr = usersVm.SearchStr, sortByStr = usersVm.SortByStr, page = pageNumber},
                new AjaxOptions {UpdateTargetId = "user-table"}
                );

            return link;
        }
    }
}