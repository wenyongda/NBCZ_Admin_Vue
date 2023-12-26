using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(NBCZ.Web.Api.Startup))]
namespace NBCZ.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 配置SignalR
            app.MapSignalR("/msghub", new HubConfiguration());
        }
    }
}