using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
        public ActionResult Index()
        {
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
            PhotoUploadViewModel photoViewModel = new PhotoUploadViewModel();
            return View(photoViewModel);
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
                        var photoUrl = string.Format("{0}/{1}","~/Images", fileName);

                        photoViewModel.PhotoUpload.SaveAs(savePath);

                        var photo = new Photo
                        {
                            PhotoUrl = photoUrl,
                            PhotoName = fileName,
                            ISO = photoViewModel.ISO,
                            ShutterSpeed = photoViewModel.ShutterSpeed,
                            CameraModel = photoViewModel.CameraModel,
                            Location = photoViewModel.Location,
                            Keywords = photoViewModel.Keywords,
                            Lens = photoViewModel.Lens
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
                ISO = photo.ISO,
                Keywords = photo.Keywords,
                Lens = photo.Lens,
                Albums = photo.Albums,
                CameraModel = photo.CameraModel,
                ShutterSpeed = photo.ShutterSpeed
                //AlbumsNotAssignedToPhoto = GetAlbumsNotAssignedToPhototest(photo.ID)
            };
            GetAlbumsAssignedToPhoto(id);
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

                        if (photoUpdateViewModel.AlbumsNotAssigned != null)
                        {
                            var selectedAlbumsSet = new HashSet<int>(photoUpdateViewModel.AlbumsNotAssigned);
                            var photoAlbums = new HashSet<int>(photoToUpdate.Albums.Select(a => a.ID));
                            foreach (var album in db.Albums)
                            {
                                if (selectedAlbumsSet.Contains(album.ID))
                                {
                                    if (!photoAlbums.Contains(album.ID))
                                    {
                                        photoToUpdate.Albums.Add(album);
                                    }
                                }
                                else
                                {
                                    if (!photoAlbums.Contains(album.ID))
                                    {

                                    }
                                }
                            }
                        }

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Unable to save");
                    }
                }

                // var albums = photoUpdateViewModel.AlbumsNotAssignedToPhoto.SelectedValues;
                // AddPhotoToAlbum(photoUpdateViewModel.PhotoId, photoUpdateViewModel.AlbumsNotAssignedToPhoto.SelectedValues)
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
        public ActionResult AddPhotoToAlbum(int? id, string[] selectedAlbums)
        {
            //var selectedAlbumsTemp = Request.Params["selectedAlbums"];
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
                        var selectedAlbumsSet = new HashSet<string>(selectedAlbums);
                        var photoAlbums = new HashSet<int>(photoToUpdate.Albums.Select(a => a.ID));
                        foreach (var album in db.Albums)
                        {
                            if (selectedAlbumsSet.Contains(album.ID.ToString()))
                            {
                                if (!photoAlbums.Contains(album.ID))
                                {
                                    photoToUpdate.Albums.Add(album);
                                }
                            }
                            else
                            {
                                if (!photoAlbums.Contains(album.ID))
                                {
                                    photoToUpdate.Albums.Remove(album);
                                }
                            }
                        }
                        db.SaveChanges();
                        GetAlbumsAssignedToPhoto(id);
                        return PartialView("AlbumsViewList");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Unable to save");
                }
            }

            return View(photoToUpdate);

        }
        [HttpPost]
        public ActionResult UpdatePhotoDetails(PhotoUpdateViewModel photoUpdateViewModel)
        {

            if (ModelState.IsValid)
            {

                var photoToUpdate = db.Photos.Find(photoUpdateViewModel.PhotoId);
                if (TryUpdateModel(photoToUpdate))
                {
                    try
                    {

                        if (photoUpdateViewModel.AlbumsNotAssigned != null)
                        {
                            var selectedAlbumsSet = new HashSet<int>(photoUpdateViewModel.AlbumsNotAssigned);
                            var photoAlbums = new HashSet<int>(photoToUpdate.Albums.Select(a => a.ID));
                            foreach (var album in db.Albums)
                            {
                                if (selectedAlbumsSet.Contains(album.ID))
                                {
                                    if (!photoAlbums.Contains(album.ID))
                                    {
                                        photoToUpdate.Albums.Add(album);
                                    }
                                }
                                else
                                {
                                    if (!photoAlbums.Contains(album.ID))
                                    {

                                    }
                                }
                            }
                        }

                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Unable to save");
                    }
                }

                // var albums = photoUpdateViewModel.AlbumsNotAssignedToPhoto.SelectedValues;
                // AddPhotoToAlbum(photoUpdateViewModel.PhotoId, photoUpdateViewModel.AlbumsNotAssignedToPhoto.SelectedValues)
                //    db.Entry(photo).State = EntityState.Modified;
                //     db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photoUpdateViewModel);
        }

        public ActionResult AlbumsList()
        {
            return PartialView("AlbumsList");
        }

        public ActionResult AlbumsViewList()
        {
            return PartialView("AlbumsViewList");
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

        //Get list of albums and check if album assigned to current photo
        private void GetAlbumsAssignedToPhoto(int? id)
        {
            Photo photo = db.Photos.Find(id);
            var photoAlbums = photo.Albums.Select(a => a.ID);
            var allAlbums = db.Albums;
            var albumsAssignedViewModel = new List<AlbumAssignedPhotosViewModel>();
            foreach (var album in allAlbums)
            {
                albumsAssignedViewModel.Add(new AlbumAssignedPhotosViewModel
                {
                    AlbumID = album.ID,
                    AlbumTitle = album.Title,
                    AlbumAssigned = photoAlbums.Contains(album.ID)

                });
            }

            ViewBag.AlbumsAssignedList = albumsAssignedViewModel;

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
}
