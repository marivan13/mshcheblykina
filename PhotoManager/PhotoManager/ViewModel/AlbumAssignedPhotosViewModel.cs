using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoManager.ViewModel
{
    public class AlbumAssignedPhotosViewModel
    {
        public int AlbumID { get; set; }
        public string AlbumTitle { get; set; }
        public bool AlbumAssigned { get; set; }
    }
}