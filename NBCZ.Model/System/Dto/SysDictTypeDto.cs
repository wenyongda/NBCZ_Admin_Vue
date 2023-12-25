namespace NBCZ.Model.System.Dto
{
    public class SysDictTypeDto
    {
        public long DictId { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        public string DictName { get; set; }
        /// <summary>
        /// 字典类型
        /// </summary>
        public string DictType { get; set; }
        public string Status { get; set; }
        /// <summary>
        /// 系统内置 Y是 N否
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 自定义sql
        /// </summary>
        public string CustomSql { get; set; }
    }
}
