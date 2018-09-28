using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PhotoManager.DataAccess;
using PhotoManager.Models;
using PhotoManager.ViewModel;

namespace PhotoManager.Controllers
{
    public class AlbumsController : Controller
    {
        private PhotoManagerDBContext db = new PhotoManagerDBContext();

        // GET: Albums
        public ActionResult Index(AlbumCategory? albumCategory)
        {
            var albums = from m in db.Albums
                         where m.AlbumType == AlbumType.PublicAlbum
                         select m;

            if (albumCategory != null)
            {
                albums = albums.Where(a => a.AlbumCategory == albumCategory);
            }
            return View(albums.ToList());
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
        public ActionResult Create([Bind(Include = "ID,UserID,Title,AlbumType,Description,DataCreation")] Album album)
        {
            try
            {
                if (db.Albums.Select(a => a.Title).Contains(album.Title))
                {
                    ModelState.AddModelError("Title", album.Title + " already exists in your albums");
                }
                if (ModelState.IsValid)
                {

                    album.UserID = 1;
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

        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
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
            GetPhotosAssignedToAlbum(id);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,Title,AlbumType,Description,DataCreation")] Album album)
        {
            if (ModelState.IsValid)
            {
                db.Entry(album).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
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

            return RedirectToAction("Edit", new { id = album.ID });
        }

        public ActionResult ShowPhotoInAlbum(IEnumerable<Photo> photos)
        {
            return PartialView("AlbumPhotoCarousel");
        }

        public ActionResult PhotoAssignedAlbumsView()
        {
            return PartialView("PhotoAssignedAlbumsView");
        }


        private void GetPhotosAssignedToAlbum(int? id)
        {
            Album album = db.Albums.Find(id);
            var albumsPhoto = album.Photos.Select(a => a.ID);
            var allPhotos = db.Photos;
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

        [HttpPost]
        public ActionResult UpdatePhotosInAlbum(int? id, string[] selectedPhotos)
        {
            //var selectedAlbumsTemp = Request.Params["selectedAlbums"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var albumToUpdate = db.Albums.Find(id);
            if (TryUpdateModel(albumToUpdate))
            {
                try
                {

                    if (selectedPhotos != null)
                    {
                        var selectedPhotosSet = new HashSet<string>(selectedPhotos);
                        var photosInAlbum = new HashSet<int>(albumToUpdate.Photos.Select(a => a.ID));
                        foreach (var photo in db.Photos)
                        {
                            if (selectedPhotosSet.Contains(photo.ID.ToString()))
                            {
                                if (!photosInAlbum.Contains(photo.ID))
                                {
                                    albumToUpdate.Photos.Add(photo);
                                }
                            }
                            else
                            {
                                if (!photosInAlbum.Contains(photo.ID))
                                {
                                    albumToUpdate.Photos.Remove(photo);
                                }
                            }
                        }
                        db.SaveChanges();
                        GetPhotosAssignedToAlbum(id);
                        return PartialView("PhotoAssignedAlbumsView");
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
    }
}
