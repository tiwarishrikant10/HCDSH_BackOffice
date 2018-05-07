using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ST_BO.Startup))]
namespace ST_BO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
