using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;

namespace Filters
{
    public class ModelValidationAttribute : ActionFilterAttribute
    {
        private static readonly ConcurrentDictionary<MethodInfo, ParameterInfo[]> MethodCache = new ConcurrentDictionary<MethodInfo, ParameterInfo[]>();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
           /* if (AppSettings.GetValue(AppSettingsKeys.RUNNING_MODE).Equals("DEBUG"))
                Logger.DoInfoLog("Debug Arp.Api Entry point ArpModelValidationAttribute:OnActionExecuting()"); */
            ValidateParameters(actionContext);

            if (actionContext.ModelState.IsValid) return;

            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState);
        }

        private void ValidateParameters(HttpActionContext context)
        {
            var descriptor = context.ActionDescriptor;
            if (descriptor == null) return;

            foreach (var param in descriptor.GetParameters())
            {
                object arg;

                context.ActionArguments.TryGetValue(param.ParameterName, out arg);

                Validate(param, arg, context.ModelState);
            }
        }

        private void Validate(HttpParameterDescriptor parameter, object argument, ModelStateDictionary modelState)
        {
            var paramAttrs = parameter.GetCustomAttributes<ValidationAttribute>().Where(x => typeof(ValidationAttribute).IsAssignableFrom(x.GetType()));

            foreach (var attr in paramAttrs)
            {

                var validationAttribute = parameter.GetCustomAttributes<ValidationAttribute>().FirstOrDefault(p => p.GetType() == attr.GetType()); //  (attr.AttributeType) as ValidationAttribute;

                if (validationAttribute == null) continue;
                if (validationAttribute.IsValid(argument)) continue;

                modelState.AddModelError(parameter.ParameterName, validationAttribute.FormatErrorMessage(parameter.ParameterName));
            }
        }

        private static IEnumerable<ParameterInfo> GetParameters(MethodInfo method) => MethodCache.GetOrAdd(method, x => x.GetParameters());
    }
}