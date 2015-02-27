using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IdentitySample.Models
{
    public class PermissionFilter : ActionFilterAttribute
    {
        private readonly ApplicationGroupManager _groupManager = new ApplicationGroupManager();
        private readonly ActionPermissionManager _permissionManager = new ActionPermissionManager();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var response = filterContext.HttpContext.Response;
            if (request.IsAjaxRequest())
            {
                #region Preventing caching of ajax request in IE browser

                response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                response.Cache.SetValidUntilExpires(false);
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetNoStore();

                #endregion Preventing caching of ajax request in IE browser
            }
            var currentActionName = filterContext.ActionDescriptor.ActionName;
            var currentControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var userId = HttpContext.Current.User.Identity.GetUserId();

            if (!_groupManager.UserHasAdministratorAccess(userId))
            {
                if (!_permissionManager.HasPermission(currentControllerName, currentActionName, userId))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}