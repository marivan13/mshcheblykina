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
            var photos = new List<Photo>
                {
                 new Photo
                    {
                        PhotoName = "IMG_123.jpeg",
                        PhotoUrl = "~/Images/IMG_123.jpeg",
                        UserID = 1,
                        CameraModel = CameraModel.Canon,
                        Location = "Stambul",
                        Diaphragm = Diaphragm.f1_8,
                        ISO = ISO.ISO_200,
                        Lens = "Canon 24-70",
                        ShutterSpeed = ShutterSpeed.S1_200,
                        Keywords = "landscape"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_124.jpeg",
                        UserID = 1,
                        PhotoUrl = "~/Images/IMG_124.jpeg",
                        CameraModel = CameraModel.Nikon,
                        Location = "Stambul",
                        Diaphragm = Diaphragm.f3_5,
                        ISO = ISO.ISO_400,
                        Lens = "Canon 85",
                        ShutterSpeed = ShutterSpeed.S1_100,
                        Keywords = "lovestory"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_125.jpeg",
                        PhotoUrl = "~/Images/IMG_123.jpeg",
                        UserID = 1,
                        CameraModel = CameraModel.Canon,
                        Location = "Stambul",
                        Diaphragm = Diaphragm.f1_8,
                        ISO = ISO.ISO_200,
                        Lens = "Canon 24-70",
                        ShutterSpeed = ShutterSpeed.S1_200,
                        Keywords = "portrait"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_126.jpeg",
                        UserID = 1,
                        PhotoUrl = "~/Images/IMG_124.jpeg",
                        CameraModel = CameraModel.Nikon,
                        Location = "Stambul",
                        Diaphragm = Diaphragm.f3_5,
                        ISO = ISO.ISO_400,
                        Lens = "Canon 85",
                        ShutterSpeed = ShutterSpeed.S1_100,
                        Keywords = "wedding"
                    }
                };

            photos.ForEach(p => context.Photos.AddOrUpdate(i => i.PhotoName, p));

            var albums = new List<Album>
                {
                    new Album {
                    Title = "Landscape1",
                    Description = "Landscape1Description",
                    UserID = 1,
                    AlbumType = AlbumType.PublicAlbum,
                    AlbumCategory = AlbumCategory.Landscape,
                    Photos = photos.ToList()

                  },
                    new Album
                    {
                        Title = "Landscape2",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Landscape,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Lovestory1",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.LoveStory,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Lovestory2",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.LoveStory,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Portrait1",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Portrait,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Portrait2",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Portrait,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Wedding1",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Wedding,
                        Photos = photos.ToList()
                    },
                    new Album
                    {
                        Title = "Wedding2",
                        Description = "Landscape1Description",
                        UserID = 1,
                        AlbumType = AlbumType.PublicAlbum,
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