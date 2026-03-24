using Contrack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Contrack
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    public class IsUserLoggedInAttribute : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            //return;
            if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length > 0)
            {
                return;
            }
            if (SessionManager.LoginSession == null)
            {
                filterContext.Result = new RedirectToRouteResult(
              new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });

            }
        }
    }
    public class LogUserActivityAttribute : FilterAttribute, IActionFilter
    {
        private Stopwatch _stopwatch;

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                _stopwatch.Stop();

                //var request = filterContext.HttpContext.Request;
                //var userSession = SessionManager.LoginSession;

                //var log = new UserActivityLog
                //{
                //    UserId = userSession?.UserID ?? 0,
                //    LoginId = userSession?.LoginID ?? 0,
                //    Endpoint = request.Url.AbsolutePath,
                //    ProcessingTimeMs = Common.ToInt(_stopwatch.ElapsedMilliseconds),
                //    IpAddress = Common.IPAddress,
                //    RequestData = request.HttpMethod == "POST" ? GetPostData(request) : ""
                //};

                //log.SaveUserActivityLog();
            }
            catch (Exception ex)
            {
            }
        }

        private string GetPostData(HttpRequestBase request)
        {
            try
            {
                request.InputStream.Position = 0;
                using (var reader = new StreamReader(request.InputStream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }

    public class TrackNavigationAttribute : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var request = filterContext.HttpContext.Request;

                if (request.HttpMethod == "GET" && !request.IsAjaxRequest() &&
                    filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).Length == 0)
                {
                    var url = request.Url?.PathAndQuery;
                    if (!string.IsNullOrEmpty(url))
                    {
                        NavigationStackManager.PushUrl(url);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }

    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
            : base()
        {
            Roles = string.Join(",", roles);

        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            return;
            if (Roles == "")
                return;
            var LoggedSession = SessionManager.LoginSession;
            if (LoggedSession != null)
            {
                var roles = Roles.Split(',');
                if (roles.Length > 0)
                {
                    if (Array.IndexOf(roles, LoggedSession.login.Type.NumericValue.ToString()) == -1)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                             new RouteValueDictionary { { "controller", "Access" }, { "action", "Unauthorized" } });
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
              new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
            }
        }
    }
    public class AuthorizeRolesTypesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesTypesAttribute(params int[] roles)
            : base()
        {
            Roles = string.Join(",", roles);

        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            return;
            if (Roles == "")
                return;
            var LoggedSession = SessionManager.LoginSession;
            if (LoggedSession != null)
            {
                var roles = Roles.Split(',');
                if (roles.Length > 0)
                {
                    if (Array.IndexOf(roles, LoggedSession.login.RoleID.NumericValue.ToString()) == -1)
                    {
                        filterContext.Result = new RedirectToRouteResult(
                             new RouteValueDictionary { { "controller", "Access" }, { "action", "Unauthorized" } });
                    }
                }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
              new RouteValueDictionary { { "controller", "Account" }, { "action", "Login" } });
            }
        }
    }
}