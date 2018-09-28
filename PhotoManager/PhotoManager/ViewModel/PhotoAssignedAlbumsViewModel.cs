using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PhotoManager.ViewModel
{
    public class PhotoAssignedAlbumsViewModel
    {
        public int PhotoID { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoName { get; set; }
        public bool PhotoAssigned { get; set; }
    }
}