using System;
using System.Collections.Generic;
using System.Web.Http;
using NBCZ.BLL.Services.System.IService;
using NBCZ.Common.CustomException;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;
using ZR.Model;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [Verify]
    [RoutePrefix("system/user")]
    public class SysUserController : BaseController
    {
        private readonly ISysUserService _userService;
        private readonly ISysRoleService _roleService;
        private readonly ISysPostService _postService;
        private readonly ISysUserPostService _userPostService;

        public SysUserController(
            ISysUserService userService,
            ISysRoleService roleService,
            ISysPostService postService,
            ISysUserPostService userPostService)
        {
            _userService = userService;
            _roleService = roleService;
            _postService = postService;
            _userPostService = userPostService;
        }

        /// <summary>
        /// 用户管理 -> 获取用户
        /// /system/user/list
        /// </summary>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:user:list")]
        [HttpGet, Route("list")]
        public IHttpActionResult List([FromUri] SysUserQueryDto user,[FromUri] PagerInfo pager)
        {
            var list = _userService.SelectUserList(user, pager);

            return SUCCESS(list);
        }

        /// <summary>
        /// 用户管理 -> 编辑、添加用户获取用户，信息查询
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet, Route("{userId:int=0}")]
        public IHttpActionResult GetInfo(int userId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var roles = _roleService.SelectRoleAll();
            dic.Add("roles", roles);
            //dic.Add("roles", SysUser.IsAdmin(userId) ? roles : roles.FindAll(f => !f.IsAdmin()));
            dic.Add("posts", _postService.GetAll());

            //编辑
            if (userId > 0)
            {
                SysUser sysUser = _userService.SelectUserById(userId);
                dic.Add("user", sysUser);
                dic.Add("postIds", _userPostService.GetUserPostsByUserId(userId));
                dic.Add("roleIds", sysUser.RoleIds);
            }

            return SUCCESS(dic);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("edit")]
        public IHttpActionResult AddUser([FromBody] SysUser user)
        {
            if (user == null) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }
            if (UserConstants.NOT_UNIQUE.Equals(_userService.CheckUserNameUnique(user.UserName)))
            {
                return ToResponse(ApiResult.Error($"新增用户 '{user.UserName}'失败，登录账号已存在"));
            }

            // user.Create_by = HttpContext.GetUId();
            // user.Create_name = HttpContext.GetNickName();
            user.Create_time = DateTime.Now;
            // user.Password = NETCore.Encrypt.EncryptProvider.Md5(user.Password);

            return SUCCESS(_userService.InsertUser(user));
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut, Route("edit")]
        // [Log(Title = "用户管理", BusinessType = BusinessType.UPDATE)]
        // [ActionPermissionFilter(Permission = "system:user:edit")]
        public IHttpActionResult UpdateUser([FromBody] SysUser user)
        {
            if (user == null || user.UserId <= 0) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }

            // user.Update_by = HttpContext.GetUId();
            // user.Update_name = HttpContext.GetNickName();
            int upResult = _userService.UpdateUser(user);

            return ToResponse(upResult);
        }

        /// <summary>
        /// 改变用户状态
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut, Route("changeStatus")]
        // [Log(Title = "修改用户状态", BusinessType = BusinessType.UPDATE)]
        // [ActionPermissionFilter(Permission = "system:user:update")]
        public IHttpActionResult ChangeStatus([FromBody] SysUser user)
        {
            if (user == null) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }

            int result = _userService.ChangeUserStatus(user);
            return ToResponse(result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpDelete, Route("{userId}")]
        // [Log(Title = "用户管理", BusinessType = BusinessType.DELETE)]
        // [ActionPermissionFilter(Permission = "system:user:remove")]
        public IHttpActionResult Remove(int userid = 0)
        {
            if (userid <= 0) { return ToResponse(ApiResult.Error(101, "请求参数错误")); }
            if (userid == 1) return ToResponse(ResultCode.FAIL, "不能删除管理员账号");
            int result = _userService.DeleteUser(userid);

            return ToResponse(result);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPut, Route("resetPwd")]
        // [Log(Title = "重置密码", BusinessType = BusinessType.UPDATE)]
        // [ActionPermissionFilter(Permission = "system:user:resetPwd")]
        public IHttpActionResult ResetPwd([FromBody] SysUserDto sysUser)
        {
            //密码md5
            // sysUser.Password = NETCore.Encrypt.EncryptProvider.Md5(sysUser.Password);

            int result = _userService.ResetPwd(sysUser.UserId, sysUser.Password);
            return ToResponse(result);
        }

        // /// <summary>
        // /// 导入
        // /// </summary>
        // /// <param name="formFile">使用IFromFile必须使用name属性否则获取不到文件</param>
        // /// <returns></returns>
        // [HttpPost, Route("importData")]
        // // [Log(Title = "用户导入", BusinessType = BusinessType.IMPORT, IsSaveRequestData = false, IsSaveResponseData = true)]
        // // [ActionPermissionFilter(Permission = "system:user:import")]
        // public IHttpActionResult ImportData([FromForm(Name = "file")] IFormFile formFile)
        // {
        //     List<SysUser> users = new List<SysUser>();
        //     using (var stream = formFile.OpenReadStream())
        //     {
        //         users = stream.Query<SysUser>(startCell: "A2").ToList();
        //     }
        //
        //     return SUCCESS(_userService.ImportUsers(users));
        // }

        // /// <summary>
        // /// 用户导入模板下载
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet, Route("importTemplate")]
        // // [Log(Title = "用户模板", BusinessType = BusinessType.EXPORT, IsSaveRequestData = true, IsSaveResponseData = false)]
        // [AllowAnonymous]
        // public IHttpActionResult ImportTemplateExcel()
        // {
        //     (string, string) result = DownloadImportTemplate("user");
        //     return ExportExcel(result.Item2, result.Item1);
        // }

        // /// <summary>
        // /// 用户导出
        // /// </summary>
        // /// <param name="user"></param>
        // /// <returns></returns>
        // [HttpGet("export")]
        // [Log(Title = "用户导出", BusinessType = BusinessType.EXPORT)]
        // [ActionPermissionFilter(Permission = "system:user:export")]
        // public IHttpActionResult UserExport([FromQuery] SysUserQueryDto user)
        // {
        //     var list = _userService.SelectUserList(user, new PagerInfo(1, 10000));
        //
        //     var result = ExportExcelMini(list.Result, "user", "用户列表");
        //     return ExportExcel(result.Item2, result.Item1);
        // }
    }
}
