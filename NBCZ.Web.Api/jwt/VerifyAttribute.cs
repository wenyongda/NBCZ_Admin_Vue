using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Infrastructure.Model;
using Microsoft.IdentityModel.Tokens;
using NBCZ.Common;
using NBCZ.Common.CustomException;
using NBCZ.DBUtility;
using NBCZ.Model;

//本命名空间暂时先不改，改动比较大2023年9月2日
namespace NBCZ.Web.Api.jwt
{
    /// <summary>
    /// 授权校验访问
    /// 如果跳过授权登录在Action 或controller加上 AllowAnonymousAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class VerifyAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var noNeedCheck = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (noNeedCheck) return;
            // byte[] symmetricKey = System.Text.Encoding.UTF8.GetBytes(Secret);
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var validateParameter = JwtUtil.ValidParameters();
            //
            // var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
            // actionContext.RequestContext.Principal = 
            TokenModel loginUser = JwtUtil.GetLoginUser(HttpContext.Current);
            // string ip = HttpContextExtension.GetClientUserIp(actionContext.Request.Properties);
            string url = actionContext.Request.RequestUri.AbsolutePath;
            var isAuthed = actionContext.RequestContext.Principal.Identity.IsAuthenticated;
            // string osType = actionContext.Request.Headers.GetValues("os")?.FirstOrDefault();
            
            if (loginUser != null)
            {
                var nowTime = DateTime.UtcNow;
                TimeSpan ts = loginUser.ExpireTime - nowTime;

                var CK = "token_" + loginUser.UserId;
                if (!CacheHelper.Exists(CK) && ts.TotalMinutes < 5)
                {
                    var newToken = JwtUtil.GenerateJwtToken(JwtUtil.AddClaims(loginUser));
                    
                    CacheHelper.SetCache(CK, CK, 1);

                    actionContext.Response.Headers.Add("X-Refresh-Token", newToken);
                }
            }

            if (loginUser == null || !isAuthed)
            {
                string msg = $"请求访问[{url}]失败，无法访问系统资源";
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, ApiResult.Error(ResultCode.DENY, msg));
            }
        }
    }
}
