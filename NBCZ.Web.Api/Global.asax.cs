using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            // var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            // json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            // json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            // json.SerializerSettings.DateFormatString = BaseController.TIME_FORMAT_FULL;
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
