using NBCZ.Model.System.Enums;

namespace NBCZ.Model.System.Dto
{
    public class RoleMenuExportDto
    {
        /// <summary>
        /// 一级目录名
        /// </summary>
        public string MenuName { get; set; }
        //[ExcelColumn(Name = "菜单名", Width = 20)]
        //public string MenuName1 { get; set; }
        //[ExcelColumn(Name = "权限按钮", Width = 20)]
        //public string MenuName2 { get; set; }
        public string Path { get; set; }
        public string Component { get; set; }
        public string Perms { get; set; }
        public MenuType MenuType { get; set; }
        public MenuStatus Status { get; set; }
    }
}
