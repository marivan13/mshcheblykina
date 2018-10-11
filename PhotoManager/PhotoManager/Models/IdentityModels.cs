using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PhotoManager.Models
{
    //You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }
    //}

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }


    //}


    //public class AppDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    //{
    //    protected override void Seed(ApplicationDbContext context)
    //    {
    //        var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
    //        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));


    //        var regularRole = new IdentityRole { Name = "regular" };
    //        var paidRole = new IdentityRole { Name = "paid" };
    //        var adminRole = new IdentityRole { Name = "admin" };

    //        roleManager.Create(regularRole);
    //        roleManager.Create(paidRole);
    //        roleManager.Create(adminRole);

    //        var regularUser = new ApplicationUser { Email = "regular.user@gmail.com", UserName = "regular.user@gmail.com" };
    //        var paidUser = new ApplicationUser { Email = "paid.user@gmail.com", UserName = "paid.user@gmail.com" };
    //        var adminUser = new ApplicationUser { Email = "admin.user@gmail.com", UserName = "admin.user@gmail.com" };
    //        string password = "Qwerty123_";

    //        if (userManager.Create(regularUser, password).Succeeded)
    //        {
    //            userManager.AddToRole(regularUser.Id, regularRole.Name);
    //        }

    //        if (userManager.Create(paidUser, password).Succeeded)
    //        {
    //            userManager.AddToRole(paidUser.Id, paidRole.Name);
    //        }

    //        if (userManager.Create(adminUser, password).Succeeded)
    //        {

    //            userManager.AddToRole(adminUser.Id, adminRole.Name);
    //            userManager.AddToRole(adminUser.Id, paidRole.Name);
    //        }

    //        base.Seed(context);
    //    }
    //}




}