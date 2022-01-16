using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Thesius_001.Startup))]
namespace Thesius_001
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
