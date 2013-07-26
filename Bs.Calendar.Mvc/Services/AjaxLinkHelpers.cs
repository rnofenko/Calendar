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
        public static MvcHtmlString AjaxSortLink(this HtmlHelper html, PagingVm paginVm, string sortByStr, AjaxOptions options)
        {
            var ajaxHelper = new AjaxHelper(html.ViewContext, html.ViewDataContainer);
            var linkText = sortByStr;
            
            //When clicked again show default unsorted view
            if (sortByStr.Equals(paginVm.SortByStr))
                sortByStr = null;

            var link = ajaxHelper.ActionLink(
                linkText,
                "List",
                new PagingVm(paginVm.SearchStr, sortByStr, paginVm.TotalPages, paginVm.Page),
                options
                );

            return link;
        }

        public static MvcHtmlString AjaxPageLink(this HtmlHelper html, PagingVm paginVm, string linkText, int pageNumber, AjaxOptions options)
        {
            var ajaxHelper = new AjaxHelper(html.ViewContext, html.ViewDataContainer);

            var link = ajaxHelper.ActionLink(
                linkText,
                "List",
                new PagingVm(paginVm.SearchStr, paginVm.SortByStr, paginVm.TotalPages, pageNumber),
                options
                );

            return link;
        }
    }
}