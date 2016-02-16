using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mysts.Startup))]
namespace mysts
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
