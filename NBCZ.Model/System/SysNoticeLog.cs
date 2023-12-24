using Newtonsoft.Json;
using SqlSugar;

namespace NBCZ.Model.System
{
    [SugarTable("sys_notice_log")]
    public class SysNoticeLog
    {
        [SugarColumn(IsPrimaryKey = true)]
        [JsonConverter(typeof(ValueToStringConverter))]
        public long Id { get; set; }
    
        [JsonConverter(typeof(ValueToStringConverter))]
        public long NoticeId { get; set; }
    
        [JsonConverter(typeof(ValueToStringConverter))]
        public long UserId { get; set; }
    
        public string Status { get; set; }

        public SysNoticeLog()
        {
        }

        public SysNoticeLog(long noticeId, long userId, string status)
        {
            NoticeId = noticeId;
            UserId = userId;
            Status = status;
        }
    }
}