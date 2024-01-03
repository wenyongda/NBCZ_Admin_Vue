using NBCZ.Model.System;

namespace NBCZ.BLL.Services.System.IService
{
    /// <summary>
    /// 参数配置service接口
    ///
    /// @author mr.zhao
    /// @date 2021-09-29
    /// </summary>
    public interface ISysConfigService : IBaseService<SysConfig>
    {
        SysConfig GetSysConfigByKey(string key);
    }
}
