using System;

namespace NBCZ.Common
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AppServiceAttribute : System.Attribute
    {
        /// <summary>
        /// 服务声明周期
        /// 不给默认值的话注册的是AddSingleton
        /// </summary>
        public LifeTime ServiceLifetime { get; set; } = LifeTime.Scoped;
        /// <summary>
        /// 指定服务类型
        /// </summary>
        public Type ServiceType { get; set; }
        /// <summary>
        /// 是否可以从第一个接口获取服务类型
        /// </summary>
        public bool InterfaceServiceType { get; set; }
    }
    
    public enum LifeTime
    {
        Transient, Scoped, Singleton
    }
}