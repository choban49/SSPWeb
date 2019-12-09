using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SSPWeb.Startup))]
namespace SSPWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
