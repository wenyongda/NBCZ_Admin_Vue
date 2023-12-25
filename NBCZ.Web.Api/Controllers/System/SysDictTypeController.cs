using System;
using System.Web;
using System.Web.Http;
using Mapster;
using NBCZ.BLL.Services.IService;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;
using ZR.Common;
using ZR.Model;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 数据字典信息
    /// </summary>
    [Verify]
    [RoutePrefix("system/dict/type")]
    // [ApiExplorerSettings(GroupName = "sys")]
    public class SysDictTypeController : BaseController
    {
        private readonly ISysDictService _sysDictService;

        public SysDictTypeController(ISysDictService sysDictService)
        {
            _sysDictService = sysDictService;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pagerInfo"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:list")]
        [HttpGet, Route("list")]
        public IHttpActionResult List([FromUri] SysDictType dict, [FromUri] PagerInfo pagerInfo)
        {
            var list = _sysDictService.SelectDictTypeList(dict, pagerInfo);

            return SUCCESS(list, TIME_FORMAT_FULL);
        }

        /// <summary>
        /// 查询字典类型详细
        /// </summary>
        /// <param name="dictId"></param>
        /// <returns></returns>
        [HttpGet, Route("{dictId}")]
        // [ActionPermissionFilter(Permission = "system:dict:query")]
        public IHttpActionResult GetInfo(long dictId = 0)
        {
            return SUCCESS(_sysDictService.GetInfo(dictId));
        }

        /// <summary>
        /// 添加字典类型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:add")]
        // [Log(Title = "字典操作", BusinessType = BusinessType.INSERT)]
        [HttpPost, Route("edit")]
        public IHttpActionResult Add([FromBody] SysDictTypeDto dto)
        {
            SysDictType dict = dto.Adapt<SysDictType>();
            if (UserConstants.NOT_UNIQUE.Equals(_sysDictService.CheckDictTypeUnique(dict)))
            {
                return ToResponse(ApiResult.Error($"新增字典'{dict.DictName}'失败，字典类型已存在"));
            }
            // dict.Create_by = HttpContext.GetUId();
            // dict.Create_name = HttpContext.GetNickName();
            dict.Create_time = DateTime.Now;
            return SUCCESS(_sysDictService.InsertDictType(dict));
        }

        /// <summary>
        /// 修改字典类型
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:edit")]
        // [Log(Title = "字典操作", BusinessType = BusinessType.UPDATE)]
        [Route("edit")]
        [HttpPut]
        public IHttpActionResult Edit([FromBody] SysDictTypeDto dto)
        {
            SysDictType dict = dto.Adapt<SysDictType>();
            if (UserConstants.NOT_UNIQUE.Equals(_sysDictService.CheckDictTypeUnique(dict)))
            {
                return ToResponse(ApiResult.Error($"修改字典'{dict.DictName}'失败，字典类型已存在"));
            }
            //设置添加人
            // dict.Update_by = HttpContext.GetUId();
            // dict.Update_name = HttpContext.GetNickName();
            return SUCCESS(_sysDictService.UpdateDictType(dict));
        }

        /// <summary>
        /// 删除字典类型
        /// </summary>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:remove")]
        // [Log(Title = "删除字典类型", BusinessType = BusinessType.DELETE)]
        [HttpDelete, Route("{ids}")]
        public IHttpActionResult Remove(string ids)
        {
            long[] idss = Tools.SpitLongArrary(ids);

            return SUCCESS(_sysDictService.DeleteDictTypeByIds(idss));
        }

        /// <summary>
        /// 字典导出
        /// </summary>
        /// <returns></returns>
        /// [ActionPermissionFilter(Permission = "system:dict:export")]
        // [Log(BusinessType = BusinessType.EXPORT, IsSaveResponseData = false, Title = "字典导出")]
        [HttpGet, Route("export")]
        public IHttpActionResult Export()
        {
            var list = _sysDictService.GetAll();

            // string sFileName = ExportExcel(list, "sysdictType", "字典");
            // return SUCCESS(new { path = "/export/" + sFileName, fileName = sFileName });
            return SUCCESS(null);
        }
    }
}
