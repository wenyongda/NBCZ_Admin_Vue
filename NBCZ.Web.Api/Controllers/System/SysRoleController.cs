using System.Web;
using System.Web.Http;
using Mapster;
using NBCZ.BLL.Services.IService;
using NBCZ.Common.CustomException;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using ZR.Common;
using ZR.Model;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 角色信息
    /// </summary>
    // [Verify]
    [RoutePrefix("system/role")]
    public class SysRoleController : BaseController
    {
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysMenuService _sysMenuService;

        public SysRoleController(
            ISysRoleService sysRoleService,
            ISysMenuService sysMenuService)
        {
            this._sysRoleService = sysRoleService;
            this._sysMenuService = sysMenuService;
        }

        /// <summary>
        /// 获取系统角色管理
        /// </summary>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:role:list")]
        [HttpGet, Route("list")]
        public IHttpActionResult List([FromUri] SysRole role, [FromUri] PagerInfo pager)
        {
            var list = _sysRoleService.SelectRoleList(role, pager);

            return SUCCESS(list, TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 根据角色编号获取详细信息
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet, Route("{roleId}")]
        public IHttpActionResult GetInfo(long roleId = 0)
        {
            var info = _sysRoleService.SelectRoleById(roleId);

            return SUCCESS(info, TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        // [ActionPermissionFilter(Permission = "system:role:add")]
        // [Log(Title = "角色管理", BusinessType = BusinessType.INSERT)]
        [Route("edit")]
        public IHttpActionResult RoleAdd([FromBody] SysRoleDto dto)
        {
            if (dto == null) return ToResponse(ApiResult.Error(101, "请求参数错误"));
            SysRole sysRoleDto = dto.Adapt<SysRole>();
            if (UserConstants.NOT_UNIQUE.Equals(_sysRoleService.CheckRoleKeyUnique(sysRoleDto)))
            {
                return ToResponse(ApiResult.Error((int)ResultCode.CUSTOM_ERROR, $"新增角色'{sysRoleDto.RoleName}'失败，角色权限已存在"));
            }

            // sysRoleDto.Create_by = HttpContext.GetUId();
            // sysRoleDto.Create_name = HttpContext.GetNickName();
            long roleId = _sysRoleService.InsertRole(sysRoleDto);

            return ToResponse(roleId);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut]
        // [ActionPermissionFilter(Permission = "system:role:edit")]
        // [Log(Title = "角色管理", BusinessType = BusinessType.UPDATE)]
        [Route("edit")]
        public IHttpActionResult RoleEdit([FromBody] SysRoleDto dto)
        {
            if (dto == null || dto.RoleId <= 0 || string.IsNullOrEmpty(dto.RoleKey))
            {
                return ToResponse(ApiResult.Error(101, "请求参数错误"));
            }
            SysRole sysRoleDto = dto.Adapt<SysRole>();
            _sysRoleService.CheckRoleAllowed(sysRoleDto);
            var info = _sysRoleService.SelectRoleById(sysRoleDto.RoleId);
            if (info != null && info.RoleKey != sysRoleDto.RoleKey)
            {
                if (UserConstants.NOT_UNIQUE.Equals(_sysRoleService.CheckRoleKeyUnique(sysRoleDto)))
                {
                    return ToResponse(ApiResult.Error($"编辑角色'{sysRoleDto.RoleName}'失败，角色权限已存在"));
                }
            }
            // sysRoleDto.Update_by = HttpContext.GetUId();
            // sysRoleDto.Update_name = HttpContext.GetNickName();
            int upResult = _sysRoleService.UpdateRole(sysRoleDto);
            if (upResult > 0)
            {
                return SUCCESS(upResult);
            }
            return ToResponse(ApiResult.Error($"修改角色'{sysRoleDto.RoleName}'失败，请联系管理员"));
        }

        /// <summary>
        /// 根据角色分配菜单
        /// </summary>
        /// <param name="sysRoleDto"></param>
        /// <returns></returns>
        [HttpPut, Route("dataScope")]
        // [ActionPermissionFilter(Permission = "system:role:authorize")]
        // [Log(Title = "角色管理", BusinessType = BusinessType.UPDATE)]
        public IHttpActionResult DataScope([FromBody] SysRoleDto sysRoleDto)
        {
            if (sysRoleDto == null || sysRoleDto.RoleId <= 0) return ToResponse(ApiResult.Error(101, "请求参数错误"));
            SysRole sysRole = sysRoleDto.Adapt<SysRole>();
            // sysRoleDto.Create_by = HttpContext.GetUId();
            // sysRoleDto.Create_name = HttpContext.GetNickName();
            _sysRoleService.CheckRoleAllowed(sysRole);

            bool result = _sysRoleService.AuthDataScope(sysRoleDto);

            return SUCCESS(result);
        }

        /// <summary>
        /// 角色删除
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{roleId}")]
        // [Log(Title = "角色管理", BusinessType = BusinessType.DELETE)]
        // [ActionPermissionFilter(Permission = "system:role:remove")]
        public IHttpActionResult Remove(string roleId)
        {
            long[] roleIds = Tools.SpitLongArrary(roleId);
            int result = _sysRoleService.DeleteRoleByRoleId(roleIds);

            return ToResponse(result);
        }

        /// <summary>
        /// 修改角色状态
        /// </summary>
        /// <param name="roleDto">角色对象</param>
        /// <returns></returns>
        [HttpPut, Route("changeStatus")]
        // [Log(Title = "修改角色状态", BusinessType = BusinessType.UPDATE)]
        // [ActionPermissionFilter(Permission = "system:role:edit")]
        public IHttpActionResult ChangeStatus([FromBody] SysRole roleDto)
        {
            _sysRoleService.CheckRoleAllowed(roleDto);
            int result = _sysRoleService.UpdateRoleStatus(roleDto);

            return ToResponse(result);
        }

        // /// <summary>
        // /// 角色导出
        // /// </summary>
        // /// <returns></returns>
        // // [Log(BusinessType = BusinessType.EXPORT, IsSaveResponseData = false, Title = "角色导出")]
        // [HttpGet, Route("export")]
        // //[ActionPermissionFilter(Permission = "system:role:export")]
        // public IHttpActionResult Export()
        // {
        //     var list = _sysRoleService.SelectRoleAll();
        //
        //     string sFileName = ExportExcel(list, "sysrole", "角色");
        //     return SUCCESS(new { path = "/export/" + sFileName, fileName = sFileName });
        // }

        // /// <summary>
        // /// 导出角色菜单
        // /// </summary>
        // /// <param name="roleId"></param>
        // /// <returns></returns>
        // [Log(BusinessType = BusinessType.EXPORT, IsSaveResponseData = false, Title = "角色菜单导出")]
        // [HttpGet("exportRoleMenu")]
        // [AllowAnonymous]
        // public IHttpActionResult ExportRoleMenu(int roleId)
        // {
        //     MenuQueryDto dto = new() { Status = "0", MenuTypeIds = "M,C,F" };
        //
        //     var list = _sysMenuService.SelectRoleMenuListByRole(dto, roleId);
        //
        //     var result = ExportExcelMini(list, roleId.ToString(), "角色菜单");
        //     return ExportExcel(result.Item2, result.Item1);
        // }
    }
}
