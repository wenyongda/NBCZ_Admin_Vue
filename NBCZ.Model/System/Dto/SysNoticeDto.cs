using System;
using ZR.Model;

namespace NBCZ.Model.System.Dto
{
    /// <summary>
    /// 通知公告表输入对象
    /// </summary>
    public class SysNoticeDto
    {
        public string NoticeId { get; set; }
        public string NoticeTitle { get; set; }
        public int NoticeType { get; set; }
        public string NoticeContent { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; }
        public string Create_name { get; set; }
        public DateTime Create_time { get; set; }
    }

    /// <summary>
    /// 通知公告表查询对象
    /// </summary>
    public class SysNoticeQueryDto : PagerInfo
    {
        public string NoticeTitle { get; set; }
        public int? NoticeType { get; set; }
        public string CreateName { get; set; }
        public int? Status { get; set; }
    }
}
