using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Infrastructure.Extensions;

namespace NBCZ.Common
{
    /// <summary>
    /// HttpContext扩展类
    /// </summary>
    public static partial class HttpContextExtension
    {
        /// <summary>
        /// 是否是ajax请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //return request.Headers.ContainsKey("X-Requested-With") &&
            //       request.Headers["X-Requested-With"].Equals("XMLHttpRequest");

            return request.Headers["X-Requested-With"] == "XMLHttpRequest" || request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        /// <summary>
        /// 判断是否IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// 获取登录用户id
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static long GetUId(this HttpContext context)
        {
            var uid = ((ClaimsIdentity)context.User.Identity).FindFirst(ClaimTypes.PrimarySid).Value;
            return !string.IsNullOrEmpty(uid) ? long.Parse(uid) : 0;
        }

        /// <summary>
        /// 获取登录用户名
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetName(this HttpContext context)
        {
            var uid = ((ClaimsIdentity)context.User.Identity).FindFirst(ClaimTypes.Name).Value;

            return uid;
        }

        public static string? GetNickName(this HttpContext context)
        {
            var nickName = ((ClaimsIdentity)context.User.Identity).FindFirst("NickName").Value;
            if (string.IsNullOrEmpty(nickName))
                nickName = null;
            return nickName;
        }

        /// <summary>
        /// 判断是否是管理员
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsAdmin(this HttpContext context)
        {
            var userName = context.GetName();
            return userName == GlobalConstant.AdminRole;
        }

        // /// <summary>
        // /// ClaimsIdentity
        // /// </summary>
        // /// <param name="context"></param>
        // /// <returns></returns>
        // public static IEnumerable<ClaimsIdentity> GetClaims(this HttpContext context)
        // {
        //     return ((ClaimsIdentity)context.User.Identity);
        // }

        public static string GetUserAgent(this HttpContext context)
        {
            return context.Request.Headers["User-Agent"];
        }
        /// <summary>
        /// 获取请求令牌
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetToken(this HttpContext context)
        {
            // Modify as needed for classic ASP.NET
            return context.Request.Headers["Authorization"];
        }

        /// <summary>
        /// 获取请求Url
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRequestUrl(this HttpContext context)
        {
            return context?.Request.Path;
        }

        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetQueryString(this HttpContext context)
        {
            return context?.Request.QueryString.ToString();
        }

        /// <summary>
        /// 获取body请求参数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetBody(this HttpContext context)
        {
            // Modify as needed for classic ASP.NET
            return string.Empty;
        }

        // /// <summary>
        // /// 获取浏览器信息
        // /// </summary>
        // /// <param name="context"></param>
        // /// <returns></returns>
        // public static ClientInfo GetClientInfo(this HttpContext context)
        // {
        //     var str = context.GetUserAgent();
        //     var uaParser = Parser.GetDefault();
        //     return uaParser.Parse(str);
        // }

        /// <summary>
        /// 根据IP获取地理位置
        /// </summary>
        /// <returns></returns>
        public static string GetIpInfo(string IP)
        {
            // Modify as needed for classic ASP.NET
            return string.Empty;
        }

        /// <summary>
        /// 设置请求参数
        /// </summary>
        /// <param name="reqMethod"></param>
        /// <param name="context"></param>
        public static string GetRequestValue(this HttpContext context, string reqMethod)
        {
            // Modify as needed for classic ASP.NET
            return string.Empty;
        }
    }

}
