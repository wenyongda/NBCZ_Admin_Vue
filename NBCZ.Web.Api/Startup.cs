using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using NBCZ.Common;
using Owin;

[assembly: OwinStartup(typeof(NBCZ.Web.Api.Startup))]
namespace NBCZ.Web.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            hubConfiguration.EnableJavaScriptProxies = false;
            // hubConfiguration.EnableJSONP = true;
            // 配置SignalR
            app.MapSignalR("/msghub", hubConfiguration);
            LogHelper logHelper = LogFactory.GetLogger(nameof(Startup));
            logHelper.Info("滋滋");
            // app.Map("/msghub", map =>
            // {
            //     var hubConfiguration = new HubConfiguration();
            //     hubConfiguration.EnableDetailedErrors = true;
            //     hubConfiguration.EnableJavaScriptProxies = false;
            //     
            //     map.RunSignalR(hubConfiguration);
            // });
            // app.MapSignalR();
        }
    }
}