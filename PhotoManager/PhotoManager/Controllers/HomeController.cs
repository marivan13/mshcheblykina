using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PhotoManager.Models;
using PhotoManager.ViewModel;
using PhotoManager.DataAccess;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PhotoManager.Controllers
{
    public class HomeController : Controller
    {
        PhotoManagerDBContext db = new PhotoManagerDBContext();

        public ActionResult Index()
        {

            var albums = from m in db.Albums
                         where m.AlbumType == AlbumType.PublicAlbum
                         select m;

            return View(albums.ToList());

        }

        public ActionResult AlbumSearch(string searchString)
        {

            var albums = from m in db.Albums
                         select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(a => a.AlbumCategory.ToString().ToLower() == searchString);
            }
            return PartialView("_PublicAlbumsList", albums.ToList());
        }


        public ActionResult SortByAlbumCategory(AlbumCategory? albumCategory)
        {
            var albums = from m in db.Albums
            where m.AlbumType == AlbumType.PublicAlbum
            select m;

            if (albumCategory != null)
            {
                albums = albums.Where(a => a.AlbumCategory == albumCategory);
            }

            return PartialView("_PublicAlbumsList", albums.ToList());
        }

    }
}