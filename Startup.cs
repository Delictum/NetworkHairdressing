using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetworkHairdressing.Startup))]
namespace NetworkHairdressing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
