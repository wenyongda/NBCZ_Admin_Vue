using NBCZ.Common;
using NBCZ.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Infrastructure.Extensions;
using NBCZ.Common.CustomException;
using Newtonsoft.Json.Serialization;
using WebApi.Jwt;

namespace NBCZ.Web.Api.Controllers
{
    public class BaseController : ApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            var res = new DataRes<string>();
            try
            {
              
                var data = controllerContext.Request.Content.ReadAsStringAsync().Result;
                var token = controllerContext.Request.Headers.Authorization;
                LogHelper.WriteRequestLog(LogLevel.Info, HttpContext.Current.Request.Url.AbsoluteUri +
                    "\r\ntoken:" + token + " \r\n data:" + data);

            }
            catch (Exception ex)
            {

                res.code = ResCode.Error;
                res.msg = "Base接口异常！异常信息：" + ex.Message;
                controllerContext.Request.CreateResponse(res);
                return;
            }

            finally 
            {
                base.Initialize(controllerContext);
            }

        }
     
        
        public static string TIME_FORMAT_FULL = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 返回成功封装
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeFormatStr"></param>
        /// <returns></returns>
        protected IHttpActionResult SUCCESS(object data, string timeFormatStr = "yyyy-MM-dd HH:mm:ss")
        {
            // string jsonStr = GetJsonStr(GetApiResult(data != null ? ResultCode.SUCCESS : ResultCode.NO_DATA, data), timeFormatStr);
            var jsonStr = GetApiResult(data != null ? ResultCode.SUCCESS : ResultCode.NO_DATA, data);
            return Ok(jsonStr);
        }

        // /// <summary>
        // /// 返回成功封装
        // /// </summary>
        // /// <param name="data"></param>
        // /// <param name="timeFormatStr"></param>
        // /// <returns></returns>
        // protected HttpResponseMessage SUCCESS(object data, bool isJson,string timeFormatStr = "yyyy-MM-dd HH:mm:ss")
        // {
        //     string jsonStr = GetJsonStr(GetApiResult(data != null ? ResultCode.SUCCESS : ResultCode.NO_DATA, data), timeFormatStr);
        //     // return Content(HttpStatusCode.OK, jsonStr, new JsonMediaTypeFormatter(), "application/json");
        //     return new HttpResponseMessage { Content = new StringContent(jsonStr, Encoding.UTF8, "application/json") };
        // }
        
        /// <summary>
        /// json输出带时间格式的
        /// </summary>
        /// <param name="apiResult"></param>
        /// <returns></returns>
        protected IHttpActionResult ToResponse(ApiResult apiResult)
        {
            // string jsonStr = GetJsonStr(apiResult, TIME_FORMAT_FULL);
            var jsonStr = apiResult;
            return Ok(jsonStr);
        }

        protected IHttpActionResult ToResponse(long rows, string timeFormatStr = "yyyy-MM-dd HH:mm:ss")
        {
            // string jsonStr = GetJsonStr(ToJson(rows), timeFormatStr);
            var jsonStr = ToJson(rows);
            return Ok(jsonStr);
        }

        protected IHttpActionResult ToResponse(ResultCode resultCode, string msg = "")
        {
            return ToResponse(new ApiResult((int)resultCode, msg));
        }
        
        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="apiResult"></param>
        // /// <param name="timeFormatStr"></param>
        // /// <returns></returns>
        // private static string GetJsonStr(ApiResult apiResult, string timeFormatStr)
        // {
        //     if (string.IsNullOrEmpty(timeFormatStr))
        //     {
        //         timeFormatStr = TIME_FORMAT_FULL;
        //     }
        //     var serializerSettings = new JsonSerializerSettings
        //     {
        //         // 设置为驼峰命名
        //         ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //         DateFormatString = timeFormatStr
        //     };
        //
        //     return JsonConvert.SerializeObject(apiResult, Formatting.Indented, serializerSettings);
        // }
        
        /// <summary>
        /// 响应返回结果
        /// </summary>
        /// <param name="rows">受影响行数</param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ApiResult ToJson(long rows, object? data = null)
        {
            return rows > 0 ? ApiResult.Success("success", data) : GetApiResult(ResultCode.FAIL);
        }
        
        /// <summary>
        /// 全局Code使用
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        protected ApiResult GetApiResult(ResultCode resultCode, object? data = null)
        {
            var msg = resultCode.GetDescription();

            return new ApiResult((int)resultCode, msg, data);
        }
    }
}
