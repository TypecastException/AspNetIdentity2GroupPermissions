using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace IdentitySample.Models
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ActionAuthorize : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var actionsPermissions = WebCache.Get(Constants.actionsKey) as List<string>;
                var user = filterContext.HttpContext.User;

                if (!actionsPermissions.Any(x => user.IsInRole(x)))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
                throw new UnauthorizedAccessException();
        }
    }

    public class Constants
    {
        public const string actionsKey = "appActionActions";
        public const string userRolesKey = "userRoles";
    }
}