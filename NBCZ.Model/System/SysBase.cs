using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCZ.Model.System
{
    public class SysBase
    {
        /// <summary>
        /// 创建人
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        [JsonProperty(propertyName: "CreateBy")]
        public long Create_by { get; set; }

        [SugarColumn(IsOnlyIgnoreUpdate = true)]
        [JsonProperty(propertyName: "CreateName")]
        public string Create_name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(IsOnlyIgnoreUpdate = true, IsNullable = true)]
        [JsonProperty(propertyName: "CreateTime")]
        public DateTime Create_time { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新人
        /// </summary>
        [JsonIgnore]
        [JsonProperty(propertyName: "UpdateBy")]
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public long Update_by { get; set; }

        [JsonIgnore]
        [JsonProperty(propertyName: "UpdateName")]
        [SugarColumn(IsOnlyIgnoreInsert = true)]
        public string Update_name { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        //[JsonIgnore]
        [SugarColumn(IsOnlyIgnoreInsert = true, IsNullable = true)]
        [JsonProperty(propertyName: "UpdateTime")]
        public DateTime? Update_time { get; set; }
        [SugarColumn(Length = 500)]
        public string Remark { get; set; }
    }
}
