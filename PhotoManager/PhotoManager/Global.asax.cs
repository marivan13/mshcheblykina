using NLog;
using PhotoManager.Controllers;
using PhotoManager.DataAccess;
using PhotoManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PhotoManager
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        protected void Application_Start()
        {
           
            Database.SetInitializer(new PhotoManagerDbInitializer());
            
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //Exception ex = Server.GetLastError();
            //if (ex != null)
            //{
            //    StringBuilder err = new StringBuilder();
            //    err.Append("Error caught in Application_Error event\n");
            //    err.Append("Error in: " + (Context.Session == null ? string.Empty : Request.Url.ToString()));
            //    err.Append("\nError Message:" + ex.Message);
            //    if (null != ex.InnerException)
            //        err.Append("\nInner Error Message:" + ex.InnerException.Message);
            //    err.Append("\n\nStack Trace:" + ex.StackTrace);
            //    Server.ClearError();

            //    if (null != Context.Session)
            //    {
            //        err.Append($"Session: Identity name:[{Thread.CurrentPrincipal.Identity.Name}] IsAuthenticated:{Thread.CurrentPrincipal.Identity.IsAuthenticated}");
            //    }
            //    _log.Error(err.ToString());

            //    if (null != Context.Session)
            //    {
            //        var routeData = new RouteData();
            //        routeData.Values.Add("controller", "Errors");
            //        routeData.Values.Add("action", "Error");
            //        routeData.Values.Add("exception", ex);

            //        if (ex.GetType() == typeof(HttpException))
            //        {
            //            routeData.Values.Add("statusCode", ((HttpException)ex).GetHttpCode());
            //        }
            //        else
            //        {
            //            routeData.Values.Add("statusCode", 500);
            //        }
            //        Response.TrySkipIisCustomErrors = true;
            //        IController controller = new ErrorsController();
            //        controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            //        Response.End();
            //    }
            //}
            var httpContext = ((MvcApplication)sender).Context;
            var currentController = String.Empty;
            var currentAction = String.Empty;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                   currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var ex = Server.GetLastError();
            var routeData = new RouteData();
            var action = "GenericError";

            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    case 404:
                        action = "NotFound";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;

            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;
            routeData.Values["exception"] = new HandleErrorInfo(ex, currentController, currentAction);

            IController errormanagerController = new ErrorsController();
            HttpContextWrapper wrapper = new HttpContextWrapper(httpContext);
            var rc = new RequestContext(wrapper, routeData);
            errormanagerController.Execute(rc);

        }
    }
}
