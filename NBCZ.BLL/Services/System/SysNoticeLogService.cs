using NBCZ.BLL.Services.System.IService;
using NBCZ.Common;
using NBCZ.Model.System;

namespace NBCZ.BLL.Services.System
{
    [AppService(ServiceType = typeof(ISysNoticeLogService), ServiceLifetime = LifeTime.Transient)]
    public class SysNoticeLogService : BaseService<SysNoticeLog>, ISysNoticeLogService
    {
    
    }
}
