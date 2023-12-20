using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using NBCZ.BLL.T4.DapperExt;
using NBCZ.Common;

namespace NBCZ.Web.Api
{
    public static class ContainerBuilerCommon
    {
        /// <summary>
        /// 注册引用程序域中所有有AppService标记的类的服务
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppService()
        {
            var cls = new string[]
            {
                // "NBCZ.Web.Api",
                "NBCZ.BLL"
            };
            var builder = new ContainerBuilder();
            foreach (var item in cls)
            {
                Register(item, builder);
            }
            HttpConfiguration config = GlobalConfiguration.Configuration;
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            SqlSugarSetup.AddDb(builder);
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
        
        private static void Register(string item, ContainerBuilder builder)
        {
            Assembly assembly = Assembly.Load(item);
            foreach (var type in assembly.GetTypes())
            {
                var serviceAttribute = type.GetCustomAttribute<AppServiceAttribute>();

                if (serviceAttribute != null)
                {
                    var serviceType = serviceAttribute.ServiceType;
                    //情况1 适用于依赖抽象编程，注意这里只获取第一个
                    if (serviceType == null && serviceAttribute.InterfaceServiceType)
                    {
                        serviceType = type.GetInterfaces().FirstOrDefault();
                    }
                    //情况2 不常见特殊情况下才会指定ServiceType，写起来麻烦
                    if (serviceType == null)
                    {
                        serviceType = type;
                    }

                    
                    switch (serviceAttribute.ServiceLifetime)
                    {
                        case LifeTime.Singleton:
                            builder.RegisterType(type).As(serviceType).SingleInstance();
                            break;
                        case LifeTime.Scoped: 
                            builder.RegisterType(type).As(serviceType).InstancePerLifetimeScope();
                            break;
                        case LifeTime.Transient:
                            builder.RegisterType(type).As(serviceType).InstancePerDependency();
                            break;
                        default:
                            builder.RegisterType(type).As(serviceType).InstancePerDependency();
                            break;
                    }
                    //System.Console.WriteLine($"注册：{serviceType}");
                }
            }
        }
    }
}