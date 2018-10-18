using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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
    [Authorize]
    public class PhotosController : AccountController
    {
        private PhotoManagerDBContext db = new PhotoManagerDBContext();
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private readonly int allowPhotosCountForRegularUser = 10;

        // GET: Photos
        public ActionResult Index(string searchString)
        {
            _log.Info("Show all photos fo current user");
            SetPhotoInfo();
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            ViewBag.DisabledRegularUser = false;

            if (user != null)
            {
                var userRole = UserManager.GetRoles(user.Id).FirstOrDefault();

                var photos = db.Photos.Where(a => a.UserID == user.Id).ToList();

                if (userRole == "regular" && photos.Count() >= allowPhotosCountForRegularUser)
                {
                    ViewBag.DisabledRegularUser = true;
                }

                if (!String.IsNullOrEmpty(searchString))
                {
                    //todo: find by all parameters
                    photos = photos.Where(a => a.ISO.ToString().ToLower() == searchString || a.Keywords.ToLower() == searchString).ToList();
                }
                return View(photos);
            }

            return View();
        }

        // GET: Photos/Details/5
        public ActionResult Details(int? photoId)
        {
            _log.Info("Details for current photo, enable to add or remove from album");
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (photoId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!IsPhotoAssignedToCurrentUser(photoId, user.Id))
            {
                return RedirectToAction("Index", "Errors");
            }
            Photo photo = db.Photos.Find(photoId);
            if (photo == null)
            {
                return HttpNotFound();
            }

            GetAlbumsAssignedToPhoto(photoId, photo.UserID);
            return View(photo);
        }

       

        // GET: Photos/Create
        [HttpGet]
        public ActionResult Create()
        {
            SetPhotoInfo();
            return View();
        }

        // POST: Photos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PhotoUploadViewModel photoViewModel)
        {
            var validPhotoTypes = new string[]
            {
                  "image/jpg",
                  "image/jpeg",
            };
            try
            {

                if (photoViewModel.PhotoUpload == null || photoViewModel.PhotoUpload.ContentLength == 0)
                {
                    ModelState.AddModelError("PhotoUpload", "Reqired");
                }

                if (!validPhotoTypes.Contains(photoViewModel.PhotoUpload.ContentType))
                {
                    ModelState.AddModelError("PhotoUpload", "Please choose JPG image");
                }

                if (ModelState.IsValid)
                {
                    ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
                    var photoFile = Bitmap.FromStream(photoViewModel.PhotoUpload.InputStream);


                    if (photoViewModel.PhotoUpload != null && photoViewModel.PhotoUpload.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(photoViewModel.PhotoUpload.FileName);
                        var savePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                        var photoUrl = string.Format("{0}/{1}", "~/Images", fileName);

                        photoViewModel.PhotoUpload.SaveAs(savePath);

                        var photo = new Photo
                        {
                            UserID = user.Id,
                            PhotoUrl = photoUrl,
                            PhotoName = fileName,
                            ISO = photoViewModel.ISO,
                            ShutterSpeed = photoViewModel.ShutterSpeed,
                            CameraModel = db.Cameras.Find(photoViewModel.CameraId),
                            Location = photoViewModel.Location,
                            Keywords = photoViewModel.Keywords,
                            LensModel = db.Lenses.Find(photoViewModel.LensId)
                        };

                        db.Photos.Add(photo);
                        db.SaveChanges();
                        return RedirectToAction("Index");


                    }
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to upload photo");
            }
            SetPhotoInfo();
            return View(photoViewModel);
        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int photoId)
        {

            if (photoId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (!IsPhotoAssignedToCurrentUser(photoId, user.Id))
            {
                return RedirectToAction("Index", "Errors");
            }
            Photo photo = db.Photos.Find(photoId);
            if (photo == null)
            {
                return HttpNotFound();
            }

            PhotoUpdateViewModel photoUpdateViewModel = new PhotoUpdateViewModel
            {
                PhotoId = photo.ID,
                PhotoName = photo.PhotoName,
                PhotoUrl = photo.PhotoUrl,
                Location = photo.Location,
                Diaphragm = photo.Diaphragm,
                ISO = photo.ISO,
                LensId = photo.LensModel.LensId,
                Albums = photo.Albums,
                CameraId = photo.CameraModel.CameraId,
                ShutterSpeed = photo.ShutterSpeed
            };

            SetPhotoInfo(photoUpdateViewModel.ISO, photoUpdateViewModel.ShutterSpeed, photoUpdateViewModel.Diaphragm);
            return View(photoUpdateViewModel);
        }

        // POST: Photos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PhotoUpdateViewModel photoUpdateViewModel)
        {
            if (ModelState.IsValid)
            {

                var photoToUpdate = db.Photos.Find(photoUpdateViewModel.PhotoId);
                if (TryUpdateModel(photoToUpdate))
                {
                    try
                    {
                        photoToUpdate.CameraModel = db.Cameras.Find(photoUpdateViewModel.CameraId);
                        photoToUpdate.LensModel = db.Lenses.Find(photoUpdateViewModel.LensId);

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Unable to save");
                    }
                }
                return RedirectToAction("Index");
            }
            return View(photoUpdateViewModel);
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int photoId)
        {
            if (photoId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            if (!IsPhotoAssignedToCurrentUser(photoId, user.Id))
            {
                return RedirectToAction("Index", "Errors");
            }
            Photo photo = db.Photos.Find(photoId);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int photoId)
        {
            Photo photo = db.Photos.Find(photoId);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //add photo to albums on details page
        [HttpPost]
        public ActionResult AddAlbumsToPhoto(int? id, string[] selectedAlbums)
        {
            _log.Info("Add photo to albums on details page");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var photoToUpdate = db.Photos.Find(id);
            if (TryUpdateModel(photoToUpdate))
            {
                try
                {

                    if (selectedAlbums != null)
                    {

                        foreach (var album in selectedAlbums)
                        {
                            var albumToAdd = db.Albums.Find(int.Parse(album));
                            photoToUpdate.Albums.Add(albumToAdd);
                        }
                        db.SaveChanges();
                        GetAlbumsAssignedToPhoto(id, photoToUpdate.UserID);
                        return PartialView("_AlbumsAssignedToPhoto");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save");
                }
            }

            return View(photoToUpdate);

        }

        [HttpGet]
        public ActionResult UpdateAlbumsToPhoto(int? id)
        {
            ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
            GetAlbumsAssignedToPhoto(id, user.Id);
            return View();
        }


        public ActionResult UpdateAlbumsToPhoto(int? id, string[] selectedNotAssignedAlbums, string[] selectedAssignedAlbums)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var photoToUpdate = db.Photos.Find(id);
            if (TryUpdateModel(photoToUpdate))
            {
                try
                {
                    if (selectedNotAssignedAlbums != null || selectedAssignedAlbums != null)
                    {

                        if (selectedNotAssignedAlbums != null)
                        {

                            foreach (var album in selectedNotAssignedAlbums)
                            {
                                var albumToAdd = db.Albums.Find(int.Parse(album));
                                photoToUpdate.Albums.Add(albumToAdd);
                            }

                        }
                        if (selectedAssignedAlbums != null)
                        {
                            foreach (var album in selectedAssignedAlbums)
                            {
                                var albumToAdd = db.Albums.Find(int.Parse(album));
                                photoToUpdate.Albums.Remove(albumToAdd);
                            }

                        }
                        db.SaveChanges();
                        GetAlbumsAssignedToPhoto(id, photoToUpdate.UserID);
                        return RedirectToAction("Edit", new {photoId = id});
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save");
                }
            }

            return View(photoToUpdate);
        }

   
        //Get list of albums and check if album assigned to current photo
        private void GetAlbumsAssignedToPhoto(int? id, string userId)
        {
            Photo photo = db.Photos.Find(id);
            var photoAlbums = photo.Albums.Select(a => a.ID).ToList();
            var allAlbums = db.Albums.Where(a => a.UserID == userId);
            var albumsAssignedViewModel = new List<AlbumsAssignedToPhotoViewModel>();
            foreach (var album in allAlbums)
            {
                albumsAssignedViewModel.Add(new AlbumsAssignedToPhotoViewModel
                {
                    AlbumID = album.ID,
                    AlbumTitle = album.Title,
                    AlbumAssigned = photoAlbums.Contains(album.ID)
                });
            }

            ViewBag.AlbumsAssignedList = albumsAssignedViewModel;

        }
        [HttpPost]
        public ActionResult PhotoAdvancedSearch(string searchField)
        {
            _log.Info("Advanced search with using stored procedure");
            try
            {
                ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
                if (!string.IsNullOrEmpty(searchField))
                {
                    List<Photo> result = db.Photos.SqlQuery("SP_AdvancedTypePhotoSearch @Search, @UserId", new SqlParameter("@Search", searchField), new SqlParameter("@UserId", user.Id)).ToList();
                    return PartialView("_PhotosList", result);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to search photos");
            }
            return View();
        }

        public ActionResult PhotoAdvancedFilterSearch(Dictionary<string,string> filter)
        {
            _log.Info("Advanced Filter search");
            try
            {
                ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
                var photos = db.Photos.Where(a => a.UserID == user.Id);

                foreach (var filterItem in filter)
                {
                    if (filterItem.Value != "All")
                    {
                        switch (filterItem.Key)
                        {
                            case "camera":
                                photos = photos.Where(a => a.CameraModel.CameraModel.Contains(filterItem.Value));
                                break;
                            case "lens":
                                photos = photos.Where(a => a.LensModel.LensModel.Contains(filterItem.Value));
                                break;
                            case "iso":
                                photos = photos.Where(a => a.ISO.Contains(filterItem.Value));
                                break;
                            case "diaphragm":
                                photos = photos.Where(a => a.Diaphragm.Contains(filterItem.Value));
                                break;
                            case "shutterspeed":
                                photos = photos.Where(a => a.ShutterSpeed.Contains(filterItem.Value));
                                break;
                            default:
                                break;
                        }
                    }
                }

                return PartialView("_PhotosList", photos.ToList());
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable to filter photos");

            }
            return View();
        }


        public ActionResult GetAllPhotosForUser()
        {
            try
            {
                ApplicationUser user = UserManager.FindByEmail(User.Identity.Name);
                var photos = db.Photos.Where(a => a.UserID == user.Id);
                return PartialView("_PhotosList", photos.ToList());
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Unable get all photots for current user");
            }
            return View();
        }


        private void SetPhotoInfo(string ISOItem = null, string ShutterSpeedItem = null, string DiaphragmItem = null)
        {
            _log.Info("Set additional info for photos");
            ViewBag.Cameras = new SelectList(db.Cameras, "CameraId", "CameraModel");
            ViewBag.Lenses = new SelectList(db.Lenses, "LensId", "LensModel");
            ViewBag.ISO = new SelectList(new string[] { "100", "200", "400", "800", "1600", "3200", "6400" }, ISOItem);
            ViewBag.ShutterSpeed = new SelectList(new string[] { "1/2", "1/4", "1/8", "1/15", "1/30", "1/60", "1/80", "1/100", "1/125", "1/200", "1/250", "1/500", "1/1000" }, ShutterSpeedItem);
            ViewBag.Diaphragm = new SelectList(new string[] { "f1.2", "f1.4", "f1.8", "f2.0", "f2.2", "f2.8", "f3.5", "f4.0", "f5.6", "f8.0", "f11.0", "f16.0", "f22.0" }, DiaphragmItem);

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IsPhotoAssignedToCurrentUser(int? photoId, string userId)
        {
            _log.Info("Check current album for current user");
            return db.Photos.Any(a => a.UserID == userId && a.ID == photoId);
        }
    }

}
