using System;
using Newtonsoft.Json;
using SqlSugar;

namespace NBCZ.Model.System
{
    /// <summary>
    /// 通知公告表
    ///
    /// @author zr
    /// @date 2021-12-15
    /// </summary>
    [SugarTable("sys_notice", "通知公告表")]
    [Tenant(0)]
    public class SysNotice
    {
        /// <summary>
        /// 公告ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnName = "notice_id")]
        [JsonConverter(typeof(ValueToStringConverter))]
        public long NoticeId { get; set; }
        /// <summary>
        /// 公告标题
        /// </summary>
        [SugarColumn(ColumnName = "notice_title", ExtendedAttribute = ProteryConstant.NOTNULL)]
        public string NoticeTitle { get; set; }
        /// <summary>
        /// 公告类型 (1通知 2公告)
        /// </summary>
        [SugarColumn(ColumnName = "notice_type", ExtendedAttribute = ProteryConstant.NOTNULL)]
        public int NoticeType { get; set; }
        /// <summary>
        /// 公告内容
        /// </summary>
        [SugarColumn(ColumnName = "notice_content", ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string NoticeContent { get; set; }
        /// <summary>
        /// 公告状态 (0正常 1关闭)
        /// </summary>
        [SugarColumn(DefaultValue = "0", ExtendedAttribute = ProteryConstant.NOTNULL)]
        public int Status { get; set; }
        
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        [JsonProperty(propertyName: "CreateBy")]
        public long Create_by { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        [JsonProperty(propertyName: "CreateName")]
        public string Create_name { get; set; }

        [JsonProperty(propertyName: "CreateTime")]
        public DateTime Create_time { get; set; } = DateTime.Now;

        [JsonIgnore]
        [JsonProperty(propertyName: "UpdateBy")]
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public long Update_by { get; set; }

        [JsonIgnore]
        [JsonProperty(propertyName: "UpdateName")]
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public string Update_name { get; set; }

        //[JsonIgnore]
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        [JsonProperty(propertyName: "UpdateTime")]
        public DateTime? Update_time { get; set; }
    }
}