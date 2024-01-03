using System.Web.Http;
using NBCZ.BLL.Services.System.IService;
using NBCZ.Common.CustomException;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 用户角色管理
    /// </summary>
    [Verify]
    [RoutePrefix("system/userRole")]
    // [ApiExplorerSettings(GroupName = "sys")]
    public class SysUserRoleController : BaseController
    {
        private readonly ISysUserRoleService _sysUserRoleService;
        private readonly ISysUserService _userService;

        public SysUserRoleController(
            ISysUserRoleService sysUserRoleService,
            ISysUserService userService)
        {
            _sysUserRoleService = sysUserRoleService;
            _userService = userService;
        }

        /// <summary>
        /// 根据角色编号获取已分配的用户
        /// </summary>
        /// <param name="roleUserQueryDto"></param>
        /// <returns></returns>
        [HttpGet, Route("list")]
        // [ActionPermissionFilter(Permission = "system:roleusers:list")]
        public IHttpActionResult GetList([FromUri] RoleUserQueryDto roleUserQueryDto)
        {
            var list = _sysUserRoleService.GetSysUsersByRoleId(roleUserQueryDto);

            return SUCCESS(list, TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 添加角色用户
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("create")]
        // [ActionPermissionFilter(Permission = "system:roleusers:add")]
        // [Log(Title = "添加角色用户", BusinessType = BusinessType.INSERT)]
        public IHttpActionResult Create([FromBody] RoleUsersCreateDto roleUsersCreateDto)
        {
            var response = _sysUserRoleService.InsertRoleUser(roleUsersCreateDto);

            return SUCCESS(response);
        }

        /// <summary>
        /// 删除角色用户
        /// </summary>
        /// <param name="roleUsersCreateDto"></param>
        /// <returns></returns>
        [HttpPost, Route("delete")]
        // [ActionPermissionFilter(Permission = "system:roleusers:remove")]
        // [Log(Title = "删除角色用户", BusinessType = BusinessType.DELETE)]
        public IHttpActionResult Delete([FromBody] RoleUsersCreateDto roleUsersCreateDto)
        {
            return SUCCESS(_sysUserRoleService.DeleteRoleUserByUserIds(roleUsersCreateDto.RoleId, roleUsersCreateDto.UserIds));
        }

        /// <summary>
        /// 获取未分配用户角色
        /// </summary>
        /// <param name="roleUserQueryDto"></param>
        /// <returns></returns>
        [HttpGet, Route("GetExcludeUsers")]
        public IHttpActionResult GetExcludeUsers([FromUri] RoleUserQueryDto roleUserQueryDto)
        {
            if (roleUserQueryDto.RoleId <= 0)
            {
                throw new CustomException(ResultCode.PARAM_ERROR, "roleId不能为空");
            }

            // 获取未添加用户
            var list = _sysUserRoleService.GetExcludedSysUsersByRoleId(roleUserQueryDto);

            return SUCCESS(list, TIME_FORMAT_FULL);
        }
    }
}
