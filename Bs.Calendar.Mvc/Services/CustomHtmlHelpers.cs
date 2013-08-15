using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;

namespace Bs.Calendar.Mvc.Services
{
    public static class CustomHtmlHelpers
    {
        public static IHtmlString RawJson(this HtmlHelper html, object value)
        {
            return html.Raw(Json.Encode(value ?? new Object()));
        }

        public static MvcHtmlString CustomDateEditor(this HtmlHelper html, Expression<Func<UserEditVm,DateTime?>> expression)
        {
            var htmlHelper = new HtmlHelper<UserEditVm>(html.ViewContext, html.ViewDataContainer);

            var tag = htmlHelper.EditorFor(user => user.BirthDate).ToString();
            tag = tag.Insert(tag.IndexOf("text-box", StringComparison.InvariantCulture), "wide text input ");
            return new MvcHtmlString(tag);
        }
    }
}