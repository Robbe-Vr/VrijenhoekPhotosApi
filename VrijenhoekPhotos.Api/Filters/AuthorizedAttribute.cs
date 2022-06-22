using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SYWCentralLogging;
using VrijenhoekPhotos.Api.Authorization;
using VrijenhoekPhotos.Core.Handlers;
using VrijenhoekPhotos.Exchange.Classes;

namespace VrijenhoekPhotos.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class |
                    AttributeTargets.Method)]
    public class AuthorizedAttribute : TypeFilterAttribute
    {
        public AuthorizedAttribute(int minRightsInt) : base(typeof(AuthorizedFilter))
        {
            Rights minRights = (Rights)minRightsInt;
            Arguments = new object[] { minRights };
        }
    }

    public class AuthorizedFilter : IAuthorizationFilter
    {
        private Rights minimumRequiredRights;

        public AuthorizedFilter(Rights minRequired)
        {
            minimumRequiredRights = minRequired;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            StringValues accessTokens = context.HttpContext.Request.Headers["VrijenhoekPhotos_AccessToken"];

            if (accessTokens.Count < 1 && Regex.IsMatch(context.HttpContext.Request.Path.Value, "/api/photos/webcontent/[0-9]+") && !context.HttpContext.Request.Query.TryGetValue("VrijenhoekPhotos_AccessToken", out accessTokens))
            {
                Logger.Log($"Denied request from {context.HttpContext.Request.Host}. Reason: No access_token provided!");
                context.Result = new ContentResult
                {
                    ContentType = "text/html",
                    Content = "<html><body>No access_token provided!\n<a href=\"https://photos.sywapps.com/Login" + "\">Login here</a></body></html>",
                };
                return;
            }
            string accessToken = accessTokens.ToString().Replace(' ', '+');

            UserDTO user = AuthManager.GetUser(accessToken);

            if (user == null)
            {
                Logger.Log($"Denied request from {context.HttpContext.Request.Host}. Reason: Unknown access_token '{accessToken}'!");
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    ContentType = "text/html",
                    Content = "<html><body>Unknown access_token!\n<a href=\"https://photos.sywapps.com/signin/Login" + "\">Login here</a></body></html>",
                };
                return;
            }

            if (minimumRequiredRights > user.Rights)
            {
                Logger.Log($"Denied request from {context.HttpContext.Request.Host}. Reason: The user associated with the access_token '{accessToken}' is not authorized for this endpoint!");
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    ContentType = "text/html",
                    Content = "<html><body>The user associated with this access_token is not authorized for this endpoint!\n<a href=\"https://photos.sywapps.com/Login" + "\">Login here</a></body></html>",
                };
                return;
            }
        }
    }
}
