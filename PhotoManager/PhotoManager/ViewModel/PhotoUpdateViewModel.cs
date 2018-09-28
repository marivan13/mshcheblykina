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
        public CameraModel CameraModel { get; set; }

        public ISO ISO { get; set; }

        public Diaphragm Diaphragm { get; set; }

        public ShutterSpeed ShutterSpeed { get; set; }
        public string Lens { get; set; }

        public IEnumerable<int> AlbumsNotAssigned { get; set; }
        public IEnumerable<Album> AlbumsNotAssignedToPhoto { get; set; }
        public virtual ICollection<Album> Albums { get; set; }

    }
}