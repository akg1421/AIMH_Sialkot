using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AIMH_Sialkot.Startup))]
namespace AIMH_Sialkot
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
