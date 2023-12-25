using System.Web;
using System.Web.Http;
using Mapster;
using NBCZ.BLL.Services.IService;
using NBCZ.Common;
using NBCZ.Common.CustomException;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 系统菜单
    /// </summary>
    [Verify]
    [RoutePrefix("system/menu")]
    // [ApiExplorerSettings(GroupName = "sys")]
    public class SysMenuController : BaseController
    {
        private readonly ISysRoleService _sysRoleService;
        private readonly ISysMenuService _sysMenuService;
        private readonly ISysRoleMenuService _sysRoleMenuService;

        public SysMenuController(
            ISysRoleService sysRoleService,
            ISysMenuService sysMenuService,
            ISysRoleMenuService sysRoleMenuService)
        {
            this._sysRoleService = sysRoleService;
            this._sysMenuService = sysMenuService;
            this._sysRoleMenuService = sysRoleMenuService;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:menu:list")]
        [HttpGet, Route("list")]
        public IHttpActionResult TreeMenuList([FromUri] MenuQueryDto menu)
        {
            long userId = HttpContext.Current.GetUId();
            return SUCCESS(_sysMenuService.SelectTreeMenuList(menu, userId), "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 根据菜单编号获取详细信息
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet, Route("{menuId}")]
        // [ActionPermissionFilter(Permission = "system:menu:query")]
        public IHttpActionResult GetMenuInfo(int menuId = 0)
        {
            return SUCCESS(_sysMenuService.GetMenuByMenuId(menuId), "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 根据菜单编号获取菜单列表，菜单管理首次进入
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpGet, Route("list/{menuId}")]
        // [ActionPermissionFilter(Permission = "system:menu:query")]
        public IHttpActionResult GetMenuList(int menuId = 0)
        {
            long userId = HttpContext.Current.GetUId();
            return SUCCESS(_sysMenuService.GetMenusByMenuId(menuId, userId), "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取角色菜单信息
        /// 加载对应角色菜单列表树
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>        
        [HttpGet, Route("roleMenuTreeselect/{roleId}")]
        public IHttpActionResult RoleMenuTreeselect(int roleId)
        {
            long userId = HttpContext.Current.GetUId();
            var menus = _sysMenuService.SelectMenuList(new MenuQueryDto(), userId);
            var checkedKeys = _sysRoleService.SelectUserRoleMenus(roleId);
            return SUCCESS(new
            {
                checkedKeys,
                menus = _sysMenuService.BuildMenuTreeSelect(menus),
            });
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="menuDto"></param>
        /// <returns></returns>
        [HttpPost, Route("edit")]
        // [Log(Title = "菜单管理", BusinessType = BusinessType.UPDATE)]
        // [ActionPermissionFilter(Permission = "system:menu:edit")]
        public IHttpActionResult MenuEdit([FromBody] MenuDto menuDto)
        {
            if (menuDto == null) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }
            //if (UserConstants.NOT_UNIQUE.Equals(sysMenuService.CheckMenuNameUnique(MenuDto)))
            //{
            //    return ToResponse(ApiResult.Error($"修改菜单'{MenuDto.menuName}'失败，菜单名称已存在"));
            //}
            var config = new TypeAdapterConfig();
            //映射规则
            config.ForType<SysMenu, MenuDto>()
                .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);//忽略字段名称的大小写;//忽略除以上配置的所有字段

            // var modal = menuDto.Adapt<SysMenu>(config).ToUpdate(HttpContext);
            var modal = menuDto.Adapt<SysMenu>(config);
            if (UserConstants.YES_FRAME.Equals(modal.IsFrame) && !modal.Path.StartsWith("http"))
            {
                return ToResponse(ApiResult.Error($"修改菜单'{modal.MenuName}'失败，地址必须以http(s)://开头"));
            }
            if (modal.MenuId.Equals(modal.ParentId))
            {
                return ToResponse(ApiResult.Error($"修改菜单'{modal.MenuName}'失败，上级菜单不能选择自己"));
            }
            // modal.Update_by = HttpContext.GetUId();
            // modal.Update_name = HttpContext.GetNickName();
            long result = _sysMenuService.EditMenu(modal);

            return ToResponse(result);
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menuDto"></param>
        /// <returns></returns>
        [HttpPut, Route("add")]
        // [Log(Title = "菜单管理", BusinessType = BusinessType.INSERT)]
        // [ActionPermissionFilter(Permission = "system:menu:add")]
        public IHttpActionResult MenuAdd([FromBody] MenuDto menuDto)
        {
            var config = new TypeAdapterConfig();
            //映射规则
            config.ForType<SysMenu, MenuDto>()
                .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
            // var menu = menuDto.Adapt<SysMenu>(config).ToCreate(HttpContext);
            var menu = menuDto.Adapt<SysMenu>(config);
            if (menu == null) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }
            if (UserConstants.NOT_UNIQUE.Equals(_sysMenuService.CheckMenuNameUnique(menu)))
            {
                return ToResponse(ApiResult.Error($"新增菜单'{menu.MenuName}'失败，菜单名称已存在"));
            }
            if (UserConstants.YES_FRAME.Equals(menu.IsFrame) && !menu.Path.StartsWith("http"))
            {
                return ToResponse(ApiResult.Error($"新增菜单'{menu.MenuName}'失败，地址必须以http(s)://开头"));
            }

            // menu.Create_by = HttpContext.GetUId();
            // menu.Create_name = HttpContext.GetNickName();
            long result = _sysMenuService.AddMenu(menu);

            return ToResponse(result);
        }

        /// <summary>
        /// 菜单删除
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{menuId}")]
        // [Log(Title = "菜单管理", BusinessType = BusinessType.DELETE)]
        // [ActionPermissionFilter(Permission = "system:menu:remove")]
        public IHttpActionResult Remove(int menuId = 0)
        {
            if (_sysMenuService.HasChildByMenuId(menuId))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "存在子菜单,不允许删除");
            }
            if (_sysRoleMenuService.CheckMenuExistRole(menuId))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, "菜单已分配,不允许删除");
            }
            int result = _sysMenuService.DeleteMenuById(menuId);

            return ToResponse(result);
        }

        /// <summary>
        /// 保存排序
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:menu:update")]
        [HttpGet, Route("ChangeSort")]
        // [Log(Title = "保存排序", BusinessType = BusinessType.UPDATE)]
        public IHttpActionResult ChangeSort(int id = 0, int value = 0)
        {
            MenuDto MenuDto = new MenuDto()
            {
                MenuId = id,
                OrderNum = value
            };
            if (MenuDto == null) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }

            int result = _sysMenuService.ChangeSortMenu(MenuDto);
            return ToResponse(result);
        }
    }
}
