using System.Collections.Generic;
using NBCZ.Model.System;

namespace NBCZ.BLL.Services.IService
{
    public interface ISysPermissionService
    {
        public List<string> GetRolePermission(SysUser user);
        public List<string> GetMenuPermission(SysUser user);
    }
}
