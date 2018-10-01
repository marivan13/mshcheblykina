using PhotoManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoManager.ViewModel
{
    public class PhotoUpdateViewModel
    {
        public int PhotoId { get; set; }
        public string PhotoUrl { get; set; }

        public string PhotoName { get; set; }
        public string Location { get; set; }

        public string Keywords { get; set; }
        public int CameraId { get; set; }

        public string ISO { get; set; }

        public string Diaphragm { get; set; }

        public string ShutterSpeed { get; set; }
        public int LensId { get; set; }

        public virtual IEnumerable<Album> AlbumsNotAssignedToPhoto { get; set; }
        public virtual ICollection<Album> Albums { get; set; }

    }
}