using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoManager.Controllers
{
    public class ErrorsController : Controller
    {

        public ActionResult Index()
        {
            return RedirectToAction("GenericError", new HandleErrorInfo(new HttpException(403, "Don't allow access the error pages"), "ErrorController", "Index"));
        }

        public ViewResult GenericError(HandleErrorInfo exception)
        {
            return View("Error", exception);
        }
        //public ActionResult Error(int statusCode, Exception exception)
        //{
        //    Response.StatusCode = statusCode;
        //    var error = new Models.Error
        //    {
        //        StatusCode = statusCode.ToString() + " error",
        //        StatusDescription = HttpWorkerRequest.GetStatusDescription(statusCode),
        //        Message = exception.Message,
        //        DateTime = DateTime.Now
        //    };
        //    return View(error);
        //}

        public ViewResult NotFound (HandleErrorInfo exception)
        {
            ViewBag.Title = "Page Not Found";
            return View("Error", exception);
        }
    }
}