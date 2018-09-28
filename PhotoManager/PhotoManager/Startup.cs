using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhotoManager.Startup))]
namespace PhotoManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
