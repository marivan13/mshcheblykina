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
        public CameraModel CameraModel { get; set; }
        public string Lens { get; set; }

        public ISO ISO { get; set; } 

        public Diaphragm Diaphragm { get; set; }

        public ShutterSpeed ShutterSpeed { get; set; }

        public HttpPostedFileBase PhotoUpload { get; set; }


    }

}