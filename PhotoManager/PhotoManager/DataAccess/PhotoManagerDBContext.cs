using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using PhotoManager.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PhotoManager.DataAccess
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class PhotoManagerDBContext : IdentityDbContext<ApplicationUser>
    {
 
        public PhotoManagerDBContext() : base("DefaultConnection")
        {

        }

        public static PhotoManagerDBContext Create()
        {
            return new PhotoManagerDBContext();
        }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<Lens> Lenses { get; set; }

        //public virtual DbSet<IdentityRole> Roles { get; set; }
        //public virtual DbSet<ApplicationUser> Users { get; set; }
        //public virtual DbSet<IdentityUserClaim> UserClaims { get; set; }
        //public virtual DbSet<IdentityUserLogin> UserLogins { get; set; }
        //public virtual DbSet<IdentityUserRole> UserRoles { get; set; }


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
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Error> Error { get; set; }
        public DbSet<LogEntry> LogEntry { get; set; }
    }

    //public class PhotoManagerDbInitializer : DropCreateDatabaseAlways<PhotoManagerDBContext>
    public class PhotoManagerDbInitializer : DropCreateDatabaseAlways<PhotoManagerDBContext>
    {
        protected override void Seed(PhotoManagerDBContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));


            var regularRole = new IdentityRole { Name = "regular" };
            var paidRole = new IdentityRole { Name = "paid" };
            var adminRole = new IdentityRole { Name = "admin" };

            roleManager.Create(regularRole);
            roleManager.Create(paidRole);
            roleManager.Create(adminRole);

            var regularUser = new ApplicationUser { Email = "regular.user@gmail.com", UserName = "regular.user@gmail.com" };
            var paidUser = new ApplicationUser { Email = "paid.user@gmail.com", UserName = "paid.user@gmail.com" };
            var adminUser = new ApplicationUser { Email = "admin.user@gmail.com", UserName = "admin.user@gmail.com" };
            string password = "Qwerty123_";

            if (userManager.Create(regularUser, password).Succeeded)
            {
                userManager.AddToRole(regularUser.Id, regularRole.Name);
            }

            if (userManager.Create(paidUser, password).Succeeded)
            {
                userManager.AddToRole(paidUser.Id, paidRole.Name);
            }

            if (userManager.Create(adminUser, password).Succeeded)
            {

                userManager.AddToRole(adminUser.Id, adminRole.Name);
                userManager.AddToRole(adminUser.Id, paidRole.Name);
            }



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
                        UserID =  regularUser.Id,
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
                        UserID =  regularUser.Id,
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
                        PhotoUrl = "~/Images/IMG_125.jpeg",
                        UserID =  regularUser.Id,
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
                        UserID = regularUser.Id,
                        PhotoUrl = "~/Images/IMG_126.jpeg",
                        CameraModel = cameraModels[3],
                        Location = "Stambul",
                        Diaphragm = "f2.2",
                        ISO = "800",
                        LensModel = lensModels[3],
                        ShutterSpeed = "1/125",
                        Keywords = "#portrait #lovestory"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_127.jpeg",
                        UserID = paidUser.Id,
                        PhotoUrl = "~/Images/IMG_127.jpeg",
                        CameraModel = cameraModels[3],
                        Location = "Stambul",
                        Diaphragm = "f2.2",
                        ISO = "800",
                        LensModel = lensModels[3],
                        ShutterSpeed = "1/125",
                        Keywords = "#portrait #lovestory"
                    },
                    new Photo
                    {
                        PhotoName = "IMG_128.jpeg",
                        UserID = paidUser.Id,
                        PhotoUrl = "~/Images/IMG_128.jpeg",
                        CameraModel = cameraModels[3],
                        Location = "Stambul",
                        Diaphragm = "f2.4",
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
                    UserID = regularUser.Id,
                    AlbumType = AlbumType.PublicAlbum,
                    AlbumCategory = AlbumCategory.Landscape,
                    Photos = photos.GetRange(0,3)

                  },
                    new Album
                    {
                        Title = "Landscape2",
                        Description = "Landscape2 Description",
                        UserID = regularUser.Id,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Landscape,
                        Photos = photos.GetRange(2,3)
                    },
                    new Album
                    {
                        Title = "Lovestory1",
                        Description = "Lovestory1 Description",
                        UserID = regularUser.Id,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.LoveStory,
                        Photos = photos.GetRange(0,4)
                    },
                    new Album
                    {
                        Title = "Lovestory2",
                        Description = "Lovestory2 Description",
                        UserID = regularUser.Id,
                        AlbumType = AlbumType.PrivateAlbum,
                        AlbumCategory = AlbumCategory.LoveStory,
                        Photos = photos.GetRange(1,1)
                    },
                    new Album
                    {
                        Title = "Portrait1",
                        Description = "Portrait1 Description",
                        UserID = regularUser.Id,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Portrait,
                        Photos = photos.GetRange(1,3)
                    },
                    new Album
                    {
                        Title = "Portrait2",
                        Description = "Portrait2 Description",
                        UserID = regularUser.Id,
                        AlbumType = AlbumType.PublicAlbum,
                        AlbumCategory = AlbumCategory.Portrait,
                        Photos = photos.GetRange(2,2)
                    },
                    new Album
                    {
                        Title = "Wedding1",
                        Description = "Wedding1 Description",
                        UserID = paidUser.Id,
                        AlbumType = AlbumType.PrivateAlbum,
                        AlbumCategory = AlbumCategory.Wedding,
                        Photos = photos.GetRange(4,2)
                    },
                    new Album
                    {
                        Title = "Wedding2",
                        Description = "Wedding2 Description",
                        UserID = paidUser.Id,
                        AlbumType = AlbumType.PrivateAlbum,
                        AlbumCategory = AlbumCategory.Wedding,
                        Photos = photos.GetRange(5,1)
                    }
                };

            albums.ForEach(a => context.Albums.AddOrUpdate(i => i.Title, a));
            context.SaveChanges();
            base.Seed(context);
        }



    }
}