using System.Linq;
using System.Web.Http;
using Infrastructure.Extensions;
using Mapster;
using NBCZ.BLL.Services.IService;
using NBCZ.Common.CustomException;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;
using SqlSugar;
using ZR.Common;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 参数配置Controller
    /// </summary>
    [Verify]
    [RoutePrefix("system/config")]
    public class SysConfigController : BaseController
    {
        /// <summary>
        /// 参数配置接口
        /// </summary>
        private readonly ISysConfigService _sysConfigService;

        public SysConfigController(ISysConfigService sysConfigService)
        {
            _sysConfigService = sysConfigService;
        }

        /// <summary>
        /// 查询参数配置列表
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("list")]
        // [ActionPermissionFilter(Permission = "system:config:list")]
        public IHttpActionResult QuerySysConfig([FromUri] SysConfigQueryDto parm)
        {
            var predicate = Expressionable.Create<SysConfig>();

            predicate = predicate.AndIF(!parm.ConfigType.IsEmpty(), m => m.ConfigType == parm.ConfigType);
            predicate = predicate.AndIF(!parm.ConfigName.IsEmpty(), m => m.ConfigName.Contains(parm.ConfigType));
            predicate = predicate.AndIF(!parm.ConfigKey.IsEmpty(), m => m.ConfigKey.Contains(parm.ConfigKey));
            predicate = predicate.AndIF(!parm.BeginTime.IsEmpty(), m => m.Create_time >= parm.BeginTime);
            predicate = predicate.AndIF(!parm.BeginTime.IsEmpty(), m => m.Create_time <= parm.EndTime);

            var response = _sysConfigService.GetPages(predicate.ToExpression(), parm);

            return SUCCESS(response);
        }

        /// <summary>
        /// 查询参数配置详情
        /// </summary>
        /// <param name="ConfigId"></param>
        /// <returns></returns>
        [HttpGet, Route("{ConfigId}")]
        // [ActionPermissionFilter(Permission = "system:config:query")]
        public IHttpActionResult GetSysConfig(int ConfigId)
        {
            var response = _sysConfigService.GetId(ConfigId);

            return SUCCESS(response);
        }

        /// <summary>
        /// 根据参数键名查询参数值
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        [HttpGet, Route("configKey/{configKey}")]
        [AllowAnonymous]
        public IHttpActionResult GetConfigKey(string configKey)
        {
            var response = _sysConfigService.Queryable().First(f => f.ConfigKey == configKey);

            return SUCCESS(response?.ConfigValue);
        }

        /// <summary>
        /// 添加参数配置
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        // [ActionPermissionFilter(Permission = "system:config:add")]
        // [Log(Title = "参数配置添加", BusinessType = BusinessType.INSERT)]
        public IHttpActionResult AddSysConfig([FromBody] SysConfigDto parm)
        {
            if (parm == null)
            {
                throw new CustomException("请求参数错误");
            }
            // var model = parm.Adapt<SysConfig>().ToCreate(HttpContext);
            var model = parm.Adapt<SysConfig>();
            
            return SUCCESS(_sysConfigService.Insert(model, it => new
            {
                it.ConfigName,
                it.ConfigKey,
                it.ConfigValue,
                it.ConfigType,
                it.Create_by,
                it.Create_time,
                it.Remark,
            }));
        }

        /// <summary>
        /// 更新参数配置
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        // [ActionPermissionFilter(Permission = "system:config:update")]
        // [Log(Title = "参数配置修改", BusinessType = BusinessType.UPDATE)]
        public IHttpActionResult UpdateSysConfig([FromBody] SysConfigDto parm)
        {
            if (parm == null)
            {
                throw new CustomException("请求实体不能为空");
            }
            // var model = parm.Adapt<SysConfig>().ToUpdate(HttpContext);
            var model = parm.Adapt<SysConfig>();
            
            var response = _sysConfigService.Update(w => w.ConfigId == model.ConfigId, it => new SysConfig()
            {
                ConfigName = model.ConfigName,
                ConfigKey = model.ConfigKey,
                ConfigValue = model.ConfigValue,
                ConfigType = model.ConfigType,
                Update_by = model.Update_by,
                Update_time = model.Update_time,
                Remark = model.Remark
            });

            return SUCCESS(response);
        }

        /// <summary>
        /// 删除参数配置
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("{ids}")]
        // [ActionPermissionFilter(Permission = "system:config:remove")]
        // [Log(Title = "参数配置删除", BusinessType = BusinessType.DELETE)]
        public IHttpActionResult DeleteSysConfig(string ids)
        {
            int[] idsArr = Tools.SpitIntArrary(ids);
            if (idsArr.Length <= 0) { return ToResponse(ApiResult.Error($"删除失败Id 不能为空")); }
            int sysCount = _sysConfigService.Count(s => s.ConfigType == "Y" && idsArr.Contains(s.ConfigId));
            if (sysCount > 0) { return ToResponse(ApiResult.Error($"删除失败Id： 系统内置参数不能删除！")); }
            var response = _sysConfigService.Delete(idsArr);

            return SUCCESS(response);
        }
    }
}