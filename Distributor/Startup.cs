using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Distributor.Startup))]
namespace Distributor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
