using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Custom.Attributes
{
    public static class ApiAntiForgeryConfig
    {
        public const string AntiForgeryHeaderName = "x-antiForgeryToken";

        public const string AntiForgerySkipValidationAppSettingsName = "AntiForgery.SkipValidation";
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiValidateAntiForgeryTokenAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(ApiAntiForgeryConfig.AntiForgerySkipValidationAppSettingsName))
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings[ApiAntiForgeryConfig.AntiForgerySkipValidationAppSettingsName]))
                {
                    return true;
                }
            }

            var headers = actionContext.Request.Headers;

            string headerToken = headers.Contains(ApiAntiForgeryConfig.AntiForgeryHeaderName)
                ? headers.GetValues(ApiAntiForgeryConfig.AntiForgeryHeaderName).FirstOrDefault()
                : null;

            if (headerToken == null)
            {
                return false;
            }

            var cookieToken = headers
              .GetCookies()
              .Select(c => c[AntiForgeryConfig.CookieName])
              .FirstOrDefault();

            try
            {
                AntiForgery.Validate(cookieToken?.Value, headerToken);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}