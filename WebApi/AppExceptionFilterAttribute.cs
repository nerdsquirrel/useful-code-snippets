using ARP.Core;
using ARP.Core.Exceptions;
using ARP.Core.Utilities;
using ARP.Services.Exceptions;
using ARP.Services.Webshop.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Filters
{
    public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            if (actionExecutedContext.Exception is NotImplementedException)
            {
                HandleException(HttpStatusCode.NotImplemented, actionExecutedContext);
            }
            else if (actionExecutedContext.Exception is ArgumentNullException)
            {
                HandleException(HttpStatusCode.BadRequest, actionExecutedContext);
            }      
            // custom exceptions      
            else if (actionExecutedContext.Exception is ServiceException)
            {
                var arpException = (ArpServiceException)actionExecutedContext.Exception;
                HandleException(HttpStatusCode.InternalServerError, actionExecutedContext, arpException.GetComponent);
            }            
            else
            {
                HandleException(HttpStatusCode.InternalServerError, actionExecutedContext);
            }

            base.OnException(actionExecutedContext);
        }

        static void HandleException(HttpStatusCode statusCode, HttpActionExecutedContext actionExecutedContext)
        {
            Logger.DoErrorLog(actionExecutedContext.Exception.Message, actionExecutedContext.Exception);
            SetExceptionToResponse(statusCode, actionExecutedContext);
        }

        static void HandleException(HttpStatusCode statusCode, HttpActionExecutedContext actionExecutedContext, string component)
        {
            Logger.DoErrorLog($"Error on component:{component}, Error:{actionExecutedContext.Exception.Message} ", actionExecutedContext.Exception);
            SetExceptionToResponse(statusCode, actionExecutedContext);
        }

        static void SetExceptionToResponse(HttpStatusCode statusCode, HttpActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(statusCode, ArpApiConstants.SYSTEM_ERROR_MESSAGE);
            if (AppSettings.GetValue(AppSettingsKeys.RUNNING_MODE).Equals("DEBUG") || statusCode != HttpStatusCode.InternalServerError )
                actionExecutedContext.Response = actionExecutedContext.Request.CreateErrorResponse(statusCode, actionExecutedContext.Exception.Message);
        }
    }
}