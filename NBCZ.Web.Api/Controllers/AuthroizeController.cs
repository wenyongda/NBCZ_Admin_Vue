using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using NBCZ.BLL;
using NBCZ.BLL.Services.IService;
using NBCZ.Common;
using NBCZ.Model;
using SqlSugar;
using WebApi.Jwt;

namespace NBCZ.Web.Api.Controllers
{
    /// <summary>
    /// 认证
    /// </summary>
    // [Route("api/Authroize")]
    [JwtAuthentication]
    public class AuthroizeController : ApiController
    {
        private readonly ISqlSugarClient db;
        private readonly IPubUserService _pubUserService;
        public AuthroizeController(ISqlSugarClient db, IPubUserService pubUserService)
        {
            this.db = db;
            _pubUserService = pubUserService;
        }


        /// <summary>
        /// 登录获取token
        /// </summary>
        /// <param name="loginViewModel">登录实体信息</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("login")]
        public IHttpActionResult Login([FromBody]LoginViewModel loginViewModel)
        {
            var ob = db.Ado.SqlQuery<Pub_User>("select * from pub_user");
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var users = new Pub_UserBLL().GetList( 
                string.Format("StopFlag=0 AND UserName='{0}' AND UserPwd='{1}'",loginViewModel.Name,loginViewModel.Password), limits: 1);
            
            if (users.Count>0)
            {
                var user = users.First();
                //var userFunctions = new  Pub_UserFunctionBLL().GetList(string.Format("UserCode='{0}'",user.UserCode)).Select(p=>p.FunctionCode);
                //var roleFunctions = new Pub_RoleFunctionBLL().GetList(string.Format(" RoleCode IN(SELECT pur.RoleCode FROM Pub_UserRole AS pur WHERE pur.UserCode='{0}' )", user.UserCode)).Select(p => p.FunctionCode);
                //var functions = userFunctions.Concat(roleFunctions).Distinct();
                //var functionsStr = string.Join(",", functions);

                var token= JwtManager.GenerateToken(user);
            
                return Ok(new ResponseObj<dynamic>()
                {
                    Code = 200,
                    Message = "认证成功",
                    Data =new
                    {
                        Token=token.Item1 ,
                        Expires = TypeUtil.ConvertDateTimeInt((DateTime)token.Item2)
                    }
                });
            }

            return Ok(new ResponseObj<dynamic>()
            {
                Code = 0,
                Message = "用户名密码错误！"
            });
            //return BadRequest();
        }
        
        [AllowAnonymous]
        [HttpPost, Route("logout")]
        public IHttpActionResult Logout()
        {
            var user = User.GetNBCZUser();
            var userId = user.Id;
            var name = user.UserName;
            return Ok(new ResponseObj<dynamic>()
            {
                Code = 200,
                Message = "成功",
                Data = new
                {
                    name,
                    id = userId
                }
            });
        }
        
        [HttpGet, Route("getInfo")]
        public IHttpActionResult GetInfo()
        {
            var user = User.GetNBCZUser();
            var userId = user.Id;
            var name = user.UserName;
            var selectUserById = _pubUserService.SelectUserById(userId);
            return Ok(new ResponseObj<dynamic>()
            {
                Code = 200,
                Message = "成功",
                Data = new
                {
                    name,
                    id = userId
                }
            });
        }
        
        public class ResponseObj<T>
        {
            /// <summary>
            /// 0请求数据错误 1 成功 -1失败
            /// </summary>
            public int Code { get; set; }

            public string Message { get; set; }

            public T Data { get; set; }
        }

    }
}
