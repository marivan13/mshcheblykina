using NLog;
using PhotoManager.Controllers;
using PhotoManager.DataAccess;
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
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                StringBuilder err = new StringBuilder();
                err.Append("Error caught in Application_Error event\n");
                err.Append("Error in: " + (Context.Session == null ? string.Empty : Request.Url.ToString()));
                err.Append("\nError Message:" + ex.Message);
                if (null != ex.InnerException)
                    err.Append("\nInner Error Message:" + ex.InnerException.Message);
                err.Append("\n\nStack Trace:" + ex.StackTrace);
                Server.ClearError();

                if (null != Context.Session)
                {
                    err.Append($"Session: Identity name:[{Thread.CurrentPrincipal.Identity.Name}] IsAuthenticated:{Thread.CurrentPrincipal.Identity.IsAuthenticated}");
                }
                _log.Error(err.ToString());

                if (null != Context.Session)
                {
                    var routeData = new RouteData();
                    routeData.Values.Add("controller", "Errors");
                    routeData.Values.Add("action", "Error");
                    routeData.Values.Add("exception", ex);

                    if (ex.GetType() == typeof(HttpException))
                    {
                        routeData.Values.Add("statusCode", ((HttpException)ex).GetHttpCode());
                    }
                    else
                    {
                        routeData.Values.Add("statusCode", 500);
                    }
                    Response.TrySkipIisCustomErrors = true;
                    IController controller = new ErrorsController();
                    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
                    Response.End();
                }
            }
        }
    }
}
