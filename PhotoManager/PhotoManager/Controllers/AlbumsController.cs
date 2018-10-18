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
      
        // GET: List of albums for current user, can select albums by album category, search by album title
        public ActionResult Index(AlbumCategory? albumCategory)
        {
            _log.Info("Show albums list for current login user");
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
        
        
        // GET: Albums/Details/id Show photos for selected album
        public ActionResult Details(int? id)
        {
            _log.Info("Show photos in album");
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
            _log.Info("Show album with public link");
            Album album = db.Albums.Where(a => a.Title == title).FirstOrDefault();
            if (album == null)
            {
                return HttpNotFound();
            }

            return View(album);
        }

        // GET: Albums/Create Create new album
        public ActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create Create new album
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,AlbumType,AlbumCategory,Description")] Album album)
        {
            try
            {
                _log.Info("Create new album");
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
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Unable to add new album");
                _log.Error(ex, "Unable to add new album");
            }

            return View(album);
        }

        [HttpGet]
        // GET: Albums/Edit/5
        public ActionResult Edit(int? albumId)
        {
            _log.Info("Update current album");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Album album)
        {
            _log.Info("Update current album");
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            var albumToUpdate = db.Albums.Find(album.ID);
            if (db.Albums.Any(a => a.UserID == user.Id && a.Title == album.Title) && albumToUpdate.Title != album.Title)
            {
                    ModelState.AddModelError("Title", album.Title + " already exists in your albums");
            }
            if (TryUpdateModel(albumToUpdate, new string[] { "Title", "AlbumType", "AlbumCategory", "Description" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    _log.Error(ex);
                }
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? albumId)
        {
            _log.Info("Delete current album");
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
            _log.Info("Delete current album");
            Album album = db.Albums.Find(albumId);
            db.Albums.Remove(album);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //GET:
        [HttpGet]
        public ActionResult DeletePhotoFromAlbum(int? photoId, int? albumId)
        {
            _log.Info("Delete photo from current album");
            Album album = db.Albums.Find(albumId);
            ViewData["photoId"] = photoId;
            return View(album);
        }

        //POST:
        [HttpPost]
        public ActionResult DeletePhotoFromAlbum(int photoId, int? albumId)
        {
            _log.Info("Delete photo from current album");
            Album album = db.Albums.Find(albumId);
            Photo photo = db.Photos.Find(photoId);
            album.Photos.Remove(photo);

            db.SaveChanges();

            return RedirectToAction("Edit", new { albumId = album.ID });
        }

        //public ActionResult PhotoAssignedAlbumsView()
        //{
        //    return PartialView("_PhotoAssignedToAlbums");
        //}

        //search on page of albums for current user
        public ActionResult AlbumSearch(string searchString)
        {
            _log.Info("Search album by title (ajax)");
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            var albums = db.Albums.Where(a => a.UserID == user.Id);
            if (!String.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(a => a.Title.ToLower() == searchString);
            }
            return PartialView("_AlbumsList", albums.ToList());
        }


        [HttpGet]
        public ActionResult ManagePhotosToAlbum(int? albumId)
        {
            _log.Info("Manage photos for current album");
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
            GetPhotosAssignedToAlbum(albumId, user.Id);
            return View(album);
        }

        //POST: Update photos for current album
        [HttpPost]
        public ActionResult UpdatePhotosToAlbum(int? id, string[] selectedAssignedPhotos, string[] selectedNotAssignedPhotos)
        {
            _log.Info("Update photos for current album (ajax)");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
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
                catch (Exception ex)
                {
                    _log.Error(ex, "Unable to update photos");
                    ModelState.AddModelError("", "Unable to update photos");
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
            _log.Info("Get list of photos for current album and for current user");
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
            _log.Info("Check current album for current user");
            return db.Albums.Any(a => a.UserID == userId && a.ID == albumId);
        }

        //check for null and existing in all get method (edit, delete, update)
        //private ActionResult CheckAlbum(int? albumId)
        //{
        //    
        //}

    }
}
