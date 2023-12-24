using System.Collections.Generic;

namespace NBCZ.Model.System.Dto
{
    public class SysDictDataDto
    {
        public string DictType { get; set; }
        public string ColumnName { get; set; }
        public List<SysDictData> List { get; set; }
    }
}
