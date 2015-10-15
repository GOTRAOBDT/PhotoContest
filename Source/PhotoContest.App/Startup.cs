using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PhotoContest.App.Startup))]
namespace PhotoContest.App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
