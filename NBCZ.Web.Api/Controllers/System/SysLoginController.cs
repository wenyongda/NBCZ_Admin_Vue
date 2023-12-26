﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Infrastructure.Extensions;
using Infrastructure.Model;
using Mapster;
using NBCZ.BLL.Services.IService;
using NBCZ.Common;
using NBCZ.Common.CustomException;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;
using WebApi.Jwt;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 登录
    /// </summary>
    // [ApiExplorerSettings(GroupName = "sys")]
    // [Verify]
    public class SysLoginController : BaseController
    {
        //static readonly NLog.Logger logger = NLog.LogManager.GetLogger("LoginController");
        // private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISysUserService sysUserService;
        private readonly ISysMenuService sysMenuService;
        private readonly ISysLoginService sysLoginService;
        private readonly ISysPermissionService permissionService;
        // private readonly ICaptcha SecurityCodeHelper;
        private readonly ISysConfigService sysConfigService;
        private readonly ISysRoleService roleService;
        // private readonly OptionsSetting optionSettings;

        public SysLoginController(
            // IHttpContextAccessor contextAccessor,
            ISysMenuService sysMenuService,
            ISysUserService sysUserService,
            ISysLoginService sysLoginService,
            ISysPermissionService permissionService,
            ISysConfigService configService,
            ISysRoleService sysRoleService
            // ICaptcha captcha,
            // IOptions<OptionsSetting> optionSettings
            )
        {
            // httpContextAccessor = contextAccessor;
            // SecurityCodeHelper = captcha;
            this.sysMenuService = sysMenuService;
            this.sysUserService = sysUserService;
            this.sysLoginService = sysLoginService;
            this.permissionService = permissionService;
            this.sysConfigService = configService;
            roleService = sysRoleService;
            // this.optionSettings = optionSettings.Value;
        }
        
        // RSA私钥
        // private static readonly string PrivatePem = AppSettings.GetConfig("RSA:PrivatePem");

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginBody">登录对象</param>
        /// <returns></returns>
        // [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        // [Log(Title = "登录")]
        public IHttpActionResult Login([FromBody] LoginBodyDto loginBody)
        {
            if (loginBody == null) { throw new CustomException("请求参数错误"); }
            // loginBody.LoginIP = HttpContextExtension.GetClientUserIp(HttpContext);
            var sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
            // var headers = HttpContext.Request.Headers;
            // var isRemoteInvoke = headers["Remote-Invoke"].FirstOrDefault().ParseToBool();
            // if (sysConfig?.ConfigValue != "off" && !SecurityCodeHelper.Validate(loginBody.Uuid, loginBody.Code)
            //                                     && !isRemoteInvoke)
            // {
            //     return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
            // }

            // sysLoginService.CheckLockUser(loginBody.Username);
            // string location = HttpContextExtension.GetIpInfo(loginBody.LoginIP);
            
            // RSA解密
            // loginBody.Password = EncryptProvider.RSADecryptWithPem(PrivatePem, loginBody.Password);
                
            var user = sysLoginService.Login(loginBody, new SysLogininfor() { LoginLocation = string.Empty });

            List<SysRole> roles = roleService.SelectUserRoleListByUserId(user.UserId);
            //权限集合 eg *:*:*,system:user:list
            List<string> permissions = permissionService.GetMenuPermission(user);

            TokenModel loginUser = new TokenModel(user.Adapt<TokenModel>(), roles.Adapt<List<Roles>>());
            // CacheService.SetUserPerms(GlobalConstant.UserPermKEY + user.UserId, permissions);
            return SUCCESS(JwtUtil.GenerateJwtToken(JwtUtil.AddClaims(loginUser)));
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        // [Log(Title = "注销")]
        [HttpPost, Route("logout")]
        public IHttpActionResult LogOut()
        {
            //Task.Run(async () =>
            //{
            //    //注销登录的用户，相当于ASP.NET中的FormsAuthentication.SignOut  
            //    await HttpContext.SignOutAsync();
            //}).Wait();
            var userid = HttpContext.Current.GetUId();
            var name = HttpContext.Current.GetName();

            // CacheService.RemoveUserPerms(GlobalConstant.UserPermKEY + userid);
            return SUCCESS(new { name, id = userid });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Verify]
        [HttpGet, Route("getInfo")]
        public IHttpActionResult GetUserInfo()
        {
            long userid = HttpContext.Current.GetUId();
            var user = sysUserService.SelectUserById(userid);

            //前端校验按钮权限使用
            //角色集合 eg: admin,yunying,common
            List<string> roles = permissionService.GetRolePermission(user);
            //权限集合 eg *:*:*,system:user:list
            List<string> permissions = permissionService.GetMenuPermission(user);
            user.WelcomeContent = GlobalConstant.WelcomeMessages[new Random().Next(0, GlobalConstant.WelcomeMessages.Length)];

            return SUCCESS(new { user, roles, permissions });
        }

        /// <summary>
        /// 获取路由信息
        /// </summary>
        /// <returns></returns>
        [Verify]
        [HttpGet, Route("getRouters")]
        public IHttpActionResult GetRouters()
        {
            long uid = HttpContext.Current.GetUId();
            var menus = sysMenuService.SelectMenuTreeByUserId(uid); 

            return SUCCESS(sysMenuService.BuildMenus(menus));
        }

        // /// <summary>
        // /// 生成图片验证码
        // /// </summary>
        // /// <returns></returns>
        // [HttpGet, Route("captchaImage")]
        // public IHttpActionResult CaptchaImage()
        // {
        //     string uuid = Guid.NewGuid().ToString().Replace("-", "");
        //
        //     SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
        //     var captchaOff = sysConfig?.ConfigValue ?? "0";
        //     var info = SecurityCodeHelper.Generate(uuid, 60);
        //     var obj = new { captchaOff, uuid, img = info.Base64 };// File(stream, "image/png")
        //
        //     return SUCCESS(obj);
        // }

        // /// <summary>
        // /// 注册
        // /// </summary>
        // /// <param name="dto"></param>
        // /// <returns></returns>
        // [HttpPost("/register")]
        // [AllowAnonymous]
        // [Log(Title = "注册", BusinessType = BusinessType.INSERT)]
        // public IHttpActionResult Register([FromBody] RegisterDto dto)
        // {
        //     SysConfig config = sysConfigService.GetSysConfigByKey("sys.account.register");
        //     if (config?.ConfigValue != "true")
        //     {
        //         return ToResponse(ResultCode.CUSTOM_ERROR, "当前系统没有开启注册功能！");
        //     }
        //     SysConfig sysConfig = sysConfigService.GetSysConfigByKey("sys.account.captchaOnOff");
        //     if (sysConfig?.ConfigValue != "off" && !SecurityCodeHelper.Validate(dto.Uuid, dto.Code))
        //     {
        //         return ToResponse(ResultCode.CAPTCHA_ERROR, "验证码错误");
        //     }
        //
        //     SysUser user = sysUserService.Register(dto);
        //     if (user.UserId > 0)
        //     {
        //         return SUCCESS(user);
        //     }
        //     return ToResponse(ResultCode.CUSTOM_ERROR, "注册失败，请联系管理员");
        // }

        #region 二维码登录

        // /// <summary>
        // /// 生成二维码
        // /// </summary>
        // /// <param name="uuid"></param>
        // /// <param name="deviceId"></param>
        // /// <returns></returns>
        // [HttpGet("/GenerateQrcode")]
        // public IHttpActionResult GenerateQrcode(string uuid, string deviceId)
        // {
        //     var state = Guid.NewGuid().ToString();
        //     var dict = new Dictionary<string, object>
        //     {
        //         { "state", state }
        //     };
        //     CacheService.SetScanLogin(uuid, dict);
        //     return SUCCESS(new
        //     {
        //         status = 1,
        //         state,
        //         uuid,
        //         codeContent = new { uuid, deviceId }// "https://qm.qq.com/cgi-bin/qm/qr?k=kgt4HsckdljU0VM-0kxND6d_igmfuPlL&authKey=r55YUbruiKQ5iwC/folG7KLCmZ++Y4rQVgNlvLbUniUMkbk24Y9+zNuOmOnjAjRc&noverify=0"
        //     });
        // }

        // /// <summary>
        // /// 轮询判断扫码状态
        // /// </summary>
        // /// <param name="dto"></param>
        // /// <returns></returns>
        // [HttpPost("/VerifyScan")]
        // [AllowAnonymous]
        // public IHttpActionResult VerifyScan([FromBody] ScanDto dto)
        // {
        //     int status = -1;
        //     object token = string.Empty;
        //     if (CacheService.GetScanLogin(dto.Uuid) is Dictionary<string, object> str)
        //     {
        //         status = 0;
        //         str.TryGetValue("token", out token);
        //         if (str.ContainsKey("status") && (string)str.GetValueOrDefault("status") == "success")
        //         {
        //             status = 2;//扫码成功
        //             CacheService.RemoveScanLogin(dto.Uuid);
        //         }
        //     }
        //
        //     return SUCCESS(new { status, token });
        // }

        // /// <summary>
        // /// 移动端扫码登录
        // /// </summary>
        // /// <param name="dto"></param>
        // /// <returns></returns>
        // [HttpPost("/ScanLogin")]
        // [Log(Title = "扫码登录")]
        // [Verify]
        // public IHttpActionResult ScanLogin([FromBody] ScanDto dto)
        // {
        //     if (dto == null) { return ToResponse(ResultCode.CUSTOM_ERROR, "扫码失败"); }
        //     var name = App.HttpContext.GetName();
        //     
        //     sysLoginService.CheckLockUser(name);
        //
        //     TokenModel tokenModel = JwtUtil.GetLoginUser(HttpContext);
        //     if (CacheService.GetScanLogin(dto.Uuid) is not null)
        //     {
        //         Dictionary<string, object> dict = new() { };
        //         dict.Add("status", "success");
        //         dict.Add("token", JwtUtil.GenerateJwtToken(JwtUtil.AddClaims(tokenModel)));
        //         CacheService.SetScanLogin(dto.Uuid, dict);
        //         
        //         return SUCCESS(1);
        //     }
        //     return ToResponse(ResultCode.FAIL, "二维码已失效");
        // }
        #endregion

    }
}
