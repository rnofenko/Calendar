using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public static class CustomHtmlHelpers
    
    {
        public static MvcHtmlString AjaxSortLink(this HtmlHelper html, PagingVm paginVm, string sortByStr, AjaxOptions options)
        {
            var ajaxHelper = new AjaxHelper(html.ViewContext, html.ViewDataContainer);
            var linkText = sortByStr;
            
            //On click choose descending or ascending sorting
            if (sortByStr.Equals(paginVm.SortByStr))
            {
                var index = sortByStr.IndexOf("Desc", StringComparison.Ordinal);
                sortByStr = index == -1 ? sortByStr + "Desc" : sortByStr.Remove(index);
            }

            var link = ajaxHelper.ActionLink(
                linkText,
                "List",
                "Users",
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
                "Users",
                new PagingVm(paginVm.SearchStr, paginVm.SortByStr, paginVm.TotalPages, pageNumber),
                options
                );

            return link;
        }

        public static MvcHtmlString CustomDateEditor(this HtmlHelper html, Expression<Func<UserEditVm,DateTime?>> expression)
        {
            var htmlHelper = new HtmlHelper<UserEditVm>(html.ViewContext, html.ViewDataContainer);

            var tag = htmlHelper.EditorFor(user => user.BirthDate).ToString();
            tag = tag.Insert(tag.IndexOf("text-box", StringComparison.InvariantCulture), "wide text input ");
            //tag = tag.Insert(6, " data-bind=\"value: model.BirthDate\"");
            return new MvcHtmlString(tag);
        }
    }
}