using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoManager.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Error(int statusCode, Exception exception)
        {
            Response.StatusCode = statusCode;
            var error = new Models.Error
            {
                StatusCode = statusCode.ToString() + " error",
                StatusDescription = HttpWorkerRequest.GetStatusDescription(statusCode),
                Message = exception.Message,
                DateTime = DateTime.Now
            };
            return View(error);
        }
    }
}