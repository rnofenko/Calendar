using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Bs.Calendar.Mvc.Services
{
    public class ValidateAjaxAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest()) return;

            var modelState = filterContext.Controller.ViewData.ModelState;
            if (modelState.IsValid) return;

            var errorModel = modelState.Values.Where(v => v.Errors.Count > 0).Select(
                v => new { errors = v.Errors.Select(e => e.ErrorMessage).ToArray()});

            filterContext.Result = new JsonResult { Data = errorModel };
            filterContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        }
    }
}