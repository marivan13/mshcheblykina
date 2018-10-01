using PhotoManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoManager.ViewModel
{
    public class PhotoUploadViewModel
    {

        public string Location { get; set; }

        public string Keywords { get; set; }
        public int CameraId{ get; set; }
        public int LensId { get; set; }

        public string ISO { get; set; } 

        public string Diaphragm { get; set; }

        public string ShutterSpeed { get; set; }

        public HttpPostedFileBase PhotoUpload { get; set; }


    }

}