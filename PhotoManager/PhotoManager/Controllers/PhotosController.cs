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
using PhotoManager.DataAccess;
using PhotoManager.Models;
using PhotoManager.ViewModel;

namespace PhotoManager.Controllers
{
    public class PhotosController : Controller
    {
        private PhotoManagerDBContext db = new PhotoManagerDBContext();

        // GET: Photos
        public ActionResult Index(string searchString)
        {
            SetPhotoInfo();
            var photos = from m in db.Photos
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                photos = photos.Where(a => a.ISO.ToString().ToLower() == searchString);
            }

            //add for  user
            return View(db.Photos.ToList());
        }

        // GET: Photos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }

            GetAlbumsAssignedToPhoto(id);
            return View(photo);
        }

        // GET: Photos/Create
        [HttpGet]
        public ActionResult Create()
        {
            SetPhotoInfo();
            return View();
        }


        private void SetPhotoInfo(string ISOItem = null, string ShutterSpeedItem = null, string DiaphragmItem = null)
        {
            ViewBag.Cameras = new SelectList(db.Cameras, "CameraId", "CameraModel");
            ViewBag.Lenses = new SelectList(db.Lenses, "LensId", "LensModel");
            ViewBag.ISO = new SelectList(new string[] { "100", "200", "400" }, ISOItem);
            ViewBag.ShutterSpeed = new SelectList(new string[] { "1/100", "1/125", "1/200" }, ShutterSpeedItem);
            ViewBag.Diaphragm = new SelectList(new string[] { "f1.2", "f1.4", "f1.8" }, DiaphragmItem);

        }
       

        // POST: Photos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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
                    var image = Bitmap.FromStream(photoViewModel.PhotoUpload.InputStream);


                    if (photoViewModel.PhotoUpload != null && photoViewModel.PhotoUpload.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(photoViewModel.PhotoUpload.FileName);
                        var savePath = Path.Combine(Server.MapPath("~/Images"), fileName);
                        var photoUrl = string.Format("{0}/{1}", "~/Images", fileName);

                        photoViewModel.PhotoUpload.SaveAs(savePath);

                        var photo = new Photo
                        {
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

            return View(photoViewModel);

        }

        // GET: Photos/Edit/5
        public ActionResult Edit(int id)
        {
           
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
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
                //AlbumsNotAssignedToPhoto = GetAlbumsNotAssignedToPhototest(photo.ID)
            };

            SetPhotoInfo(photoUpdateViewModel.ISO, photoUpdateViewModel.ShutterSpeed, photoUpdateViewModel.Diaphragm);
            //GetAlbumsAssignedToPhoto(id);
            return View(photoUpdateViewModel);
        }

        // POST: Photos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
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


                        //if (photoUpdateViewModel.AlbumsNotAssigned != null)
                        //{
                        //    var selectedAlbumsSet = new HashSet<int>(photoUpdateViewModel.AlbumsNotAssigned);
                        //    var photoAlbums = new HashSet<int>(photoToUpdate.Albums.Select(a => a.ID));
                        //    foreach (var album in db.Albums)
                        //    {
                        //        if (selectedAlbumsSet.Contains(album.ID))
                        //        {
                        //            if (!photoAlbums.Contains(album.ID))
                        //            {
                        //                photoToUpdate.Albums.Add(album);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (!photoAlbums.Contains(album.ID))
                        //            {

                        //            }
                        //        }
                        //    }
                        //}

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Unable to save");
                    }
                }

                //    db.Entry(photo).State = EntityState.Modified;
                //     db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photoUpdateViewModel);
        }

        // GET: Photos/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photos.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Photo photo = db.Photos.Find(id);
            db.Photos.Remove(photo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult AddAlbumsToPhoto(int? id, string[] selectedAlbums)
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

                    if (selectedAlbums != null)
                    {

                        foreach (var album in selectedAlbums)
                        {
                            var albumToAdd = db.Albums.Find(int.Parse(album));
                                photoToUpdate.Albums.Add(albumToAdd);
                        }
                        db.SaveChanges();
                        GetAlbumsAssignedToPhoto(id);
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
            GetAlbumsAssignedToPhoto(id);
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
                        GetAlbumsAssignedToPhoto(id);
                        return RedirectToAction("Edit", new { id });
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save");
                }
            }

            return View(photoToUpdate);
        }

        public ActionResult AlbumsList()
        {
            return PartialView("AlbumsList");
        }

        public ActionResult AlbumsViewList()
        {
            return PartialView("AlbumsViewList");
        }


        //Get list of albums and check if album assigned to current photo
        private void GetAlbumsAssignedToPhoto(int? id)
        {
            Photo photo = db.Photos.Find(id);
            var photoAlbums = photo.Albums.Select(a => a.ID);
            var allAlbums = db.Albums;
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
        public ActionResult PhotoAdvancedSearch(string keywords)
        {
            try
            {
                var searchString = new SqlParameter("@Search", "lovestory");
                List<Photo> result = db.Photos.SqlQuery("SP_AdvancedTypePhotoSearch @Search", searchString).ToList();
                return PartialView("_PhotosList", result);
            }
            catch(Exception exception)
            {
                
                //logger
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    //private MultiSelectList GetAlbumsNotAssignedToPhoto(int? id)
    //{
    //    Photo photo = db.Photos.Find(id);
    //    var photoAlbums = photo.Albums.Select(a => a.ID);
    //    var albumsNotAssignedToPhoto = db.Albums.ToList().Except(photo.Albums);
    //    return new MultiSelectList(albumsNotAssignedToPhoto, "ID", "Title");

    //}

    //private IEnumerable<Album> GetAlbumsNotAssignedToPhototest(int? id)
    //{
    //    Photo photo = db.Photos.Find(id);
    //    var photoAlbums = photo.Albums.Select(a => a.ID);
    //    var albumsNotAssignedToPhoto = db.Albums.ToList().Except(photo.Albums);
    //    return albumsNotAssignedToPhoto;

    //}
}
