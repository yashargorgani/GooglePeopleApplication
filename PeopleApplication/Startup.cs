using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PeopleApplication.Startup))]
namespace PeopleApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
