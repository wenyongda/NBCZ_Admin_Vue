using System.Collections.Generic;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using ZR.Model;

namespace NBCZ.BLL.Services.System.IService
{
    /// <summary>
    /// 通知公告表service接口
    ///
    /// @author zr
    /// @date 2021-12-15
    /// </summary>
    public interface ISysNoticeService : IBaseService<SysNotice>
    {
        List<SysNotice> GetSysNotices();

        PagedInfo<SysNotice> GetPageList(SysNoticeQueryDto parm);
    }
}
