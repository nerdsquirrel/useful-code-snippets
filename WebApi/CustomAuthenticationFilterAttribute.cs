using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Filters
{
    public class AuthenticationFilterAttribute : Attribute, IAuthenticationFilter
    {        
        public bool AllowMultiple => false;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var actionDescriptor = context.ActionContext.ActionDescriptor;
            var isAnonymousAllowed = actionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any() ||
                                    context.ActionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any();
            if (isAnonymousAllowed)
            {
                return Task.FromResult(0);
            }

            HttpRequestMessage request = context.ActionContext.Request;
            HttpRequestHeaders headers = request.Headers;           

            if (headers == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing headers", request);
                return Task.FromResult(0);
            }
            var userInfo =  GetUserInfo(headers);   // get user information from header        
            if (IsValidUser(userInfo)) // user validation based on requirement
            {
                context.ErrorResult = new AuthenticationFailureResult("User is not authenticated.", request);
                return Task.FromResult(0);
            }

            IPrincipal principal = new GenericPrincipal(new GenericIdentity(userInfo.UserLoginGroupId), null);
            context.Principal = principal;            

            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {            
            return Task.FromResult(0);
        }
    }
}