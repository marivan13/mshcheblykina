using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NLog;
using PhotoManager.DataAccess;
using PhotoManager.Models;
using PhotoManager.ViewModel;

namespace PhotoManager.Controllers
{
    public class AlbumsController : AccountController
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private PhotoManagerDBContext db = new PhotoManagerDBContext();
       
        private readonly int allowAlbumsCountForRegularUser = 5;
      
        // GET: Albums
        public ActionResult Index(AlbumCategory? albumCategory)
        {
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            ViewBag.DisabledRegularUser = false;

            if (user != null)
            {
               var userRole = UserManager.GetRoles(user.Id).FirstOrDefault();
               var albums = db.Albums.Where(a => a.UserID == user.Id).ToList();

                if (userRole == "regular" && albums.Count() >= allowAlbumsCountForRegularUser)
                {
                    ViewBag.DisabledRegularUser = true;
                }

                if (albumCategory != null)
                {
                    albums = albums.Where(a => a.AlbumCategory == albumCategory).ToList();
                }
                return View(albums);
            }

            return RedirectToAction("Index", "Home");  
        }
        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            return View(album);
        }

        //public link to album
        [Route("Album/{title}")]
        public ActionResult ShowAlbum(string title)
        {
            Album album = db.Albums.Where(a => a.Title == title).FirstOrDefault();
            if (album == null)
            {
                return HttpNotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,AlbumType,AlbumCategory,Description")] Album album)
        {
            try
            {
                ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);

                if (db.Albums.Any(a => a.UserID == user.Id && a.Title == album.Title))
                {
                    ModelState.AddModelError("Title", album.Title + " already exists in your albums");
                }
                if (ModelState.IsValid)
                {
                    album.UserID = user.Id;
                    db.Albums.Add(album);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Unable to add new album");
            }

            return View(album);
        }

        [HttpGet]
        // GET: Albums/Edit/5
        public ActionResult Edit(int? albumId)
        {

            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (albumId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!IsAlbumAssignedToCurrentUser(albumId, user.Id))
            {
                return RedirectToAction("Index", "Errors");
            }
            Album album = db.Albums.Find(albumId);
            if (album == null)
            {
                return HttpNotFound();
            }
            //CheckAlbum(albumId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Album album)
        {
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            var editAlbum = db.Albums.Find(album.ID);
            if (db.Albums.Any(a => a.UserID == user.Id && a.Title == album.Title) && editAlbum.Title != album.Title)
            {
                    ModelState.AddModelError("Title", album.Title + " already exists in your albums");
            }
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? albumId)
        {
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (albumId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!IsAlbumAssignedToCurrentUser(albumId, user.Id))
            {
                return RedirectToAction("Index", "Errors");
            }
            Album album = db.Albums.Find(albumId);
            if (album == null)
            {
                return View("Error");
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int albumId)
        {
            Album album = db.Albums.Find(albumId);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeletePhotoFromAlbum(int? photoId, int? albumId)
        {
            Album album = db.Albums.Find(albumId);
            ViewData["photoId"] = photoId;
            return View(album);
        }

        [HttpPost]
        public ActionResult DeletePhotoFromAlbum(int photoId, int? albumId)
        {
            Album album = db.Albums.Find(albumId);
            Photo photo = db.Photos.Find(photoId);
            album.Photos.Remove(photo);

            db.SaveChanges();

            return RedirectToAction("Edit", new { albumId = album.ID });
        }

        public ActionResult PhotoAssignedAlbumsView()
        {
            return PartialView("_PhotoAssignedToAlbums");
        }

        public ActionResult AlbumSearch(string searchString)
        {

            var albums = from m in db.Albums
                         select m;
            if (!String.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(a => a.Title.ToLower() == searchString);
            }
            return PartialView("_AlbumsList", albums.ToList());
        }

        [HttpGet]
        public ActionResult ManagePhotosToAlbum(int? albumId)
        {
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (albumId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!IsAlbumAssignedToCurrentUser(albumId, user.Id))
            {
                return RedirectToAction("Index", "Errors");
            }
            Album album = db.Albums.Find(albumId);
            if (album == null)
            {
                return HttpNotFound();
            }
            GetPhotosAssignedToAlbum(albumId, album.UserID);
            return View(album);
        }

        [HttpPost]
        public ActionResult ManagePhotosToAlbum(int? id, string[] selectedAssignedPhotos, string[] selectedNotAssignedPhotos)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var albumToUpdate = db.Albums.Find(id);
            if (TryUpdateModel(albumToUpdate))
            {
                try
                {

                    if (selectedAssignedPhotos != null || selectedNotAssignedPhotos != null)
                    {
                        if (selectedNotAssignedPhotos != null)
                        {

                            foreach (var photo in selectedNotAssignedPhotos)
                            {
                                var photoToAdd = db.Photos.Find(int.Parse(photo));
                                albumToUpdate.Photos.Add(photoToAdd);
                            }

                        }
                        if (selectedAssignedPhotos != null)
                        {
                            foreach (var photo in selectedAssignedPhotos)
                            {
                                var photoToAdd = db.Photos.Find(int.Parse(photo));
                                albumToUpdate.Photos.Remove(photoToAdd);
                            }

                        }
                        db.SaveChanges();
                        GetPhotosAssignedToAlbum(id, albumToUpdate.UserID);
                        return PartialView("_PhotoAssignedToAlbums");
                        
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save");
                }
            }

            return View(albumToUpdate);

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void GetPhotosAssignedToAlbum(int? albumId, string userId)
        {
            Album album = db.Albums.Find(albumId);
            var albumsPhoto = album.Photos.Select(a => a.ID);
            var allPhotos = db.Photos.Where(p => p.UserID == userId);
            var photosAssignedViewModel = new List<PhotoAssignedAlbumsViewModel>();
            foreach (var photo in allPhotos)
            {
                photosAssignedViewModel.Add(new PhotoAssignedAlbumsViewModel
                {
                    PhotoID = photo.ID,
                    PhotoName = photo.PhotoName,
                    PhotoUrl = photo.PhotoUrl,
                    PhotoAssigned = albumsPhoto.Contains(photo.ID)

                });
            }

            ViewBag.PhotosAssignedList = photosAssignedViewModel;

        }

        private bool IsAlbumAssignedToCurrentUser(int? albumId, string userId)
        {
            return db.Albums.Any(a => a.UserID == userId && a.ID == albumId);
        }

        //method  to check for null and existing in all get method (edit, delete, update)
        //private ActionResult CheckAlbum(int? albumId)
        //{
        //    
        //}

    }
}
