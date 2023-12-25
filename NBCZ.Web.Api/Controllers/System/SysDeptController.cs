using System.Collections;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using NBCZ.BLL.Services.IService;
using NBCZ.Common.CustomException;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;
using ZR.Common;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 部门
    /// </summary>
    [Verify]
    [RoutePrefix("system/dept")]
    public class SysDeptController : BaseController
    {
        private readonly ISysDeptService _deptService;
        private readonly ISysUserService _userService;
        public SysDeptController(ISysDeptService deptService
            , ISysUserService userService)
        {
            _deptService = deptService;
            _userService = userService;
        }

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("list")]
        public IHttpActionResult List([FromUri] SysDeptQueryDto dept)
        {
            return SUCCESS(_deptService.GetSysDepts(dept), TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 查询部门列表（排除节点）
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [HttpGet, Route("list/exclude/{deptId}")]
        public IHttpActionResult ExcludeChild(long deptId)
        {
            var depts = _deptService.GetSysDepts(new SysDeptQueryDto());

            for (int i = 0; i < depts.Count; i++)
            {
                SysDept d = depts[i];
                long[] deptIds = Tools.SpitLongArrary(d.Ancestors);
                if (d.DeptId == deptId || ((IList)deptIds).Contains(deptId))
                {
                    depts.Remove(d);
                }
            }
            return SUCCESS(depts);
        }

        /// <summary>
        /// 获取部门下拉树列表
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpGet, Route("treeselect")]
        public IHttpActionResult TreeSelect(SysDeptQueryDto dept)
        {
            var depts = _deptService.GetSysDepts(dept);

            return SUCCESS(_deptService.BuildDeptTreeSelect(depts), TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 获取角色部门信息
        /// 加载对应角色部门列表树
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>        
        [HttpGet, Route("roleDeptTreeselect/{roleId}")]
        public IHttpActionResult RoleMenuTreeselect(int roleId)
        {
            var depts = _deptService.GetSysDepts(new SysDeptQueryDto());
            var checkedKeys = _deptService.SelectRoleDepts(roleId);
            return SUCCESS(new
            {
                checkedKeys,
                depts = _deptService.BuildDeptTreeSelect(depts),
            });
        }

        /// <summary>
        /// 根据部门编号获取详细信息
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("{deptId}")]
        // [ActionPermissionFilter(Permission = "system:dept:query")]
        public IHttpActionResult GetInfo(long deptId)
        {
            var info = _deptService.GetFirst(f => f.DeptId == deptId);
            return SUCCESS(info);
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        // [Log(Title = "部门管理", BusinessType = BusinessType.INSERT)]
        // [ActionPermissionFilter(Permission = "system:dept:add")]
        public IHttpActionResult Add([FromBody] SysDept dept)
        {
            if (UserConstants.NOT_UNIQUE.Equals(_deptService.CheckDeptNameUnique(dept)))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, $"新增部门{dept.DeptName}失败，部门名称已存在");
            }
            // dept.Create_by = HttpContext.GetUId();
            // dept.Create_name = HttpContext.GetNickName();
            return ToResponse(_deptService.InsertDept(dept));
        }

        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPut]
        // [Log(Title = "部门管理", BusinessType = BusinessType.UPDATE)]
        // [ActionPermissionFilter(Permission = "system:dept:update")]
        public IHttpActionResult Update([FromBody] SysDept dept)
        {
            if (UserConstants.NOT_UNIQUE.Equals(_deptService.CheckDeptNameUnique(dept)))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, $"修改部门{dept.DeptName}失败，部门名称已存在");
            }
            else if (dept.ParentId.Equals(dept.DeptId))
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, $"修改部门{dept.DeptName}失败，上级部门不能是自己");
            }
            // dept.Update_by = HttpContext.GetUId();
            // dept.Update_name = HttpContext.GetNickName();
            return ToResponse(_deptService.UpdateDept(dept));
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("{deptId}")]
        // [ActionPermissionFilter(Permission = "system:dept:remove")]
        // [Log(Title = "部门管理", BusinessType = BusinessType.DELETE)]
        public IHttpActionResult Remove(long deptId)
        {
            if (_deptService.Queryable().Count(it => it.ParentId == deptId && it.DelFlag == 0) > 0)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, $"存在下级部门，不允许删除");
            }
            if (_userService.Queryable().Count(it => it.DeptId == deptId && it.DelFlag == 0) > 0)
            {
                return ToResponse(ResultCode.CUSTOM_ERROR, $"部门存在用户，不允许删除");
            }

            return SUCCESS(_deptService.Delete(deptId));
        }
    }
}
