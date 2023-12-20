using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.WebApi;
using NBCZ.BLL.T4.DapperExt;
using NBCZ.Common;
using NBCZ.Web.Api.Controllers;
using SqlSugar;

namespace NBCZ.Web.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            InstanceFactory.CustomAssemblies = new[]
                { typeof(SqlSugar.MySqlConnector.MySqlProvider).Assembly };
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ContainerBuilerCommon.AddAppService();
        }

        protected void Application_BeginRequest()
        {
            //跨域
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.End();
            }
        }
    }
}
