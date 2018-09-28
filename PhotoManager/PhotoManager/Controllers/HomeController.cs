using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoManager.Models;
using PhotoManager.ViewModel;
using PhotoManager.DataAccess;
using System.Data.Entity;

namespace PhotoManager.Controllers
{
    public class HomeController : Controller
    {
        PhotoManagerDBContext db = new PhotoManagerDBContext();

        public ActionResult Index(AlbumCategory? albumCategory, string searchString)
        {

            var albums = from m in db.Albums
                         where m.AlbumType == AlbumType.PublicAlbum
                         select m;

            if (albumCategory != null)
            {
                albums = albums.Where(a => a.AlbumCategory == albumCategory);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(a => a.AlbumCategory.ToString().ToLower() == searchString);
            }
            return View(albums.ToList());

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}