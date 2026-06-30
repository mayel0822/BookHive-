using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookHive.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AllowMISAnonymousAttribute : Attribute { }

    public class RequireMISLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var hasAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowMISAnonymousAttribute>()
                .Any();

            if (hasAnonymous) return;

            var session = context.HttpContext.Session.GetString("MISUser");

            if (string.IsNullOrEmpty(session))
            {
                context.Result = new RedirectToActionResult("Login", "MIS", null);
            }
        }
    }
}
