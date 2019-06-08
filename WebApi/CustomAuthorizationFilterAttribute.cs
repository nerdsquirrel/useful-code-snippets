using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Filters
{
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        private string _reason = "";

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, "You are not authorized to access this resource");
            if (!string.IsNullOrEmpty(_reason))
                actionContext.Response.ReasonPhrase = _reason;
        }

        private bool IsAllowAnonymous(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any() ||
                   actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any();
        }

        private bool IsSecuredApiCallRequested(HttpActionContext actionContext)
        {
            var apiAttributes = actionContext.ActionDescriptor.GetCustomAttributes<AuthorizationFilterAttribute>(true);
            if (apiAttributes != null && apiAttributes.Any())
            {
                return true;
            }

            apiAttributes = actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizationFilterAttribute>(true);
            if (apiAttributes != null && apiAttributes.Any())
            {
                return true;
            }

            return false;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (IsAllowAnonymous(actionContext))
                return true;

            if (!IsSecuredApiCallRequested(actionContext) || !actionContext.RequestContext.Principal.Identity.IsAuthenticated)
                return false;

            string userLoginGroupId = actionContext.RequestContext.Principal.Identity.Name;
            if (string.IsNullOrEmpty(userLoginGroupId))
            {               
                _reason = "Unauthorized request";
                return false;
            }

            return true;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (IsAuthorized(actionContext))
                return;

            HandleUnauthorizedRequest(actionContext);
        }
    }
}