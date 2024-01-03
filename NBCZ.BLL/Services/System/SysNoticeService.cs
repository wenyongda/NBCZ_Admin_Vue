using System.Collections.Generic;
using NBCZ.BLL.Services.System.IService;
using NBCZ.Common;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using SqlSugar;
using ZR.Model;

namespace NBCZ.BLL.Services.System
{
    /// <summary>
    /// 通知公告表Service业务层处理
    ///
    /// @author zr
    /// @date 2021-12-15
    /// </summary>
    [AppService(ServiceType = typeof(ISysNoticeService), ServiceLifetime = LifeTime.Transient)]
    public class SysNoticeService : BaseService<SysNotice>, ISysNoticeService
    {
        #region 业务逻辑代码

        /// <summary>
        /// 查询系统通知
        /// </summary>
        /// <returns></returns>
        public List<SysNotice> GetSysNotices()
        {
            var predicate = Expressionable.Create<SysNotice>();

            predicate = predicate.And(m => m.Status == 0);
            return Queryable()
                .Where(predicate.ToExpression())
                .OrderByDescending(f => f.Create_time)
                .ToList();
        }

        public PagedInfo<SysNotice> GetPageList(SysNoticeQueryDto parm)
        {
            var predicate = Expressionable.Create<SysNotice>();

            predicate = predicate.AndIF(!string.IsNullOrEmpty(parm.NoticeTitle), 
                m => m.NoticeTitle.Contains(parm.NoticeTitle));
            predicate = predicate.AndIF(parm.NoticeType != null, m => m.NoticeType == parm.NoticeType);
            predicate = predicate.AndIF(!string.IsNullOrEmpty(parm.CreateName), 
                m => m.Create_name.Contains(parm.CreateName) || m.Update_name.Contains(parm.CreateName));
            predicate = predicate.AndIF(parm.Status != null, m => m.Status == parm.Status);
            var response = GetPages(predicate.ToExpression(), parm);
            return response;
        }

        #endregion
    }
}