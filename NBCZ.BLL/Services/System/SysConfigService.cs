using NBCZ.BLL.Services.System.IService;
using NBCZ.Common;
using NBCZ.Model.System;

namespace NBCZ.BLL.Services.System
{
    /// <summary>
    /// 参数配置Service业务层处理
    /// </summary>
    [AppService(ServiceType = typeof(ISysConfigService), ServiceLifetime = LifeTime.Transient)]
    public class SysConfigService : BaseService<SysConfig>, ISysConfigService
    {
        #region 业务逻辑代码

        public SysConfig GetSysConfigByKey(string key)
        {
            return Queryable().First(f => f.ConfigKey == key);
        }

        #endregion
    }
}