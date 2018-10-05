using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using PhotoManager.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;

namespace PhotoManager.DataAccess
{
    public class PhotoManagerDBContext : DbContext
    {
 
        public PhotoManagerDBContext() : base("DefaultConnection")
        {

        }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Lens> Lenses { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>()
               .HasMany<Photo>(s => s.Photos)
               .WithMany(c => c.Albums)
               .Map(cs =>
               {
                   cs.MapLeftKey("AlbumRefId");
                   cs.MapRightKey("PhotoRefId");
                   cs.ToTable("AlbumPhoto");
               });

            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
        }
    }

    public class PhotoManagerDbInitializer : DropCreateDatabaseIfModelChanges<PhotoManagerDBContext>
    {
        protected override void Seed(PhotoManagerDBContext context)
        {
            var cameraModels = new List<Camera>
            {
                new Camera { CameraModel = "Canon 6d" },
                new Camera { CameraModel = "Canon 60d"},
                new Camera { CameraModel = "Canon 70d"},
                new Camera { CameraModel = "Canon 7d"}
            };

            cameraModels.ForEach(p => context.Cameras.AddOrUpdate(i => i.CameraModel, p));

            var lensModels = new List<Lens>
            {
                new Lens { LensModel = "Canon 24-70" },
                new Lens { LensModel = "Canon 135"},
                new Lens { LensModel = "Canon 35"},
                new Lens { LensModel = "Canon 16-24"}
            };

           

            lensModels.ForEach(p => context.Lenses.AddOrUpdate(i => i.LensModel, p));

            var keywords = new List<string> { "landscape", "lovestory", "portrait", "wedding" };
            var photos = new List<Photo>
                {
                 new Photo
                    {
                        PhotoName = "IMG_123.jpeg",
                        PhotoUrl = "~/Images/IMG_123.jpeg",
                        UserID = 1,
                        CameraModel = cameraModels[0],
                        Location = "Stambul",
                        Diaphragm = "f2.0",
                        ISO = "100",
                        LensModel = lensModels[0],
                        ShutterSpeed = "1/125",
                        Keywords = "#landscape"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_124.jpeg",
                        UserID = 1,
                        PhotoUrl = "~/Images/IMG_124.jpeg",
                        CameraModel = cameraModels[1],
                        Location = "Stambul",
                        Diaphragm = "f2.2",
                        ISO = "200",
                        LensModel = lensModels[1],
                        ShutterSpeed = "1/125",
                        Keywords = "#lovestory"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_125.jpeg",
                        PhotoUrl = "~/Images/IMG_123.jpeg",
                        UserID = 1,
                        CameraModel = cameraModels[2],
                        Location = "Stambul",
                        Diaphragm = "f1.4",
                        ISO = "400",
                        LensModel = lensModels[2],
                        ShutterSpeed = "1/125",
                        Keywords = "#portrait"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_126.jpeg",
                        UserID = 1,
                        PhotoUrl = "~/Images/IMG_124.jpeg",
                        CameraModel = cameraModels[3],
                        Location = "Stambul",
                        Diaphragm = "f2.2",
                        ISO = "800",
                        LensModel = lensModels[3],
                        ShutterSpeed = "1/125",
                        Keywords = "#portrait #lovestory"
                    }
                };

            photos.ForEach(p => context.Photos.AddOrUpdate(i => i.PhotoName, p));

            var albums = new List<Album>
                {
                    new Album {
                    Title = "Landscape1",
                    Description = "Landscape1 Description",
                    UserID = 1,
                    AlbumType = AlbumType.PublicAlbum,
                    AlbumCategory = AlbumCategory.Landscape,
                    Photos = photos.ToList()

                  },
                    new Album
                    {
                        Title = "Landscape2",
                        Description = "Landscape2 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Landscape,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Lovestory1",
                        Description = "Lovestory1 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.LoveStory,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Lovestory2",
                        Description = "Lovestory2 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.LoveStory,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Portrait1",
                        Description = "Portrait1 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Portrait,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Portrait2",
                        Description = "Portrait2 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Portrait,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Wedding1",
                        Description = "Wedding1 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PrivateAlbum,
                        AlbumCategory = AlbumCategory.Wedding,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Wedding2",
                        Description = "Wedding2 Description",
                        UserID = 1,
                        AlbumType = AlbumType.PrivateAlbum,
                        AlbumCategory = AlbumCategory.Wedding,
                        Photos = photos.ToList()
                    }
                };

            albums.ForEach(a => context.Albums.AddOrUpdate(i => i.Title, a));
            context.SaveChanges();
            base.Seed(context);
        }

    }
}