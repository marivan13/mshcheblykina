using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PhotoManager.Helpers.Url
{
    public static class UrlExtensions
    {
        public static string PublicAlbumUrl(this UrlHelper helper, string albumTitle)
        {
            return helper.RouteUrl("PublicAlbum", new
            {
                controller = "Albums",
                action = "ShowAlbum",
                Title = albumTitle
            });

        }

    }
}