using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using NBCZ.BLL.Services.IService;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using NBCZ.Web.Api.jwt;
using ZR.Model;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 数据字典信息
    /// </summary>
    [Verify]
    [RoutePrefix("system/dict/data")]
    public class SysDictDataController : BaseController
    {
        private readonly ISysDictDataService _sysDictDataService;
        private readonly ISysDictService _sysDictService;

        public SysDictDataController(ISysDictService sysDictService, ISysDictDataService sysDictDataService)
        {
            _sysDictService = sysDictService;
            _sysDictDataService = sysDictDataService;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pagerInfo"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:list")]
        [HttpGet, Route("list")]
        public IHttpActionResult List([FromUri] SysDictData dictData, [FromUri] PagerInfo pagerInfo)
        {
            var list = _sysDictDataService.SelectDictDataList(dictData, pagerInfo);

            if (dictData.DictType.StartsWith("sql_"))
            {
                var result = _sysDictService.SelectDictDataByCustomSql(dictData.DictType);

                list.Result.AddRange(result);
                list.TotalNum += result.Count;
            }
            return SUCCESS(list);
        }

        /// <summary>
        /// 根据字典类型查询字典数据信息
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("type/{dictType}")]
        public IHttpActionResult DictType(string dictType)
        {
            return SUCCESS(_sysDictDataService.SelectDictDataByType(dictType));
        }

        /// <summary>
        /// 根据字典类型查询字典数据信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("types")]
        public IHttpActionResult DictTypes([FromBody] List<SysDictDataDto> dto)
        {
            var list = _sysDictDataService.SelectDictDataByTypes(dto.Select(f => f.DictType).ToArray());
            List<SysDictDataDto> dataVos = new List<SysDictDataDto>();

            foreach (var dic in dto)
            {
                SysDictDataDto vo = new SysDictDataDto()
                {
                    DictType = dic.DictType,
                    ColumnName = dic.ColumnName,
                    List = list.FindAll(f => f.DictType == dic.DictType)
                };
                if (dic.DictType.StartsWith("cus_") || dic.DictType.StartsWith("sql_"))
                {
                    vo.List.AddRange(_sysDictService.SelectDictDataByCustomSql(dic.DictType));
                }
                dataVos.Add(vo);
            }
            return SUCCESS(dataVos);
        }

        /// <summary>
        /// 查询字典数据详细
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("info/{dictCode}")]
        public IHttpActionResult GetInfo(long dictCode)
        {
            return SUCCESS(_sysDictDataService.SelectDictDataById(dictCode));
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:add")]
        // [Log(Title = "字典数据", BusinessType = BusinessType.INSERT)]
        [HttpPost()]
        public IHttpActionResult Add([FromBody] SysDictData dict)
        {
            // dict.Create_by = HttpContext.GetUId();
            // dict.Create_name = HttpContext.GetNickName();
            dict.Create_time = DateTime.Now;
            return SUCCESS(_sysDictDataService.InsertDictData(dict));
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:edit")]
        // [Log(Title = "字典数据", BusinessType = BusinessType.UPDATE)]
        [HttpPut()]
        public IHttpActionResult Edit([FromBody] SysDictData dict)
        {
            // dict.Update_by = HttpContext.GetUId();
            // dict.Update_name = HttpContext.GetNickName();
            return SUCCESS(_sysDictDataService.UpdateDictData(dict));
        }

        /// <summary>
        /// 删除字典类型
        /// </summary>
        /// <param name="dictCode"></param>
        /// <returns></returns>
        // [ActionPermissionFilter(Permission = "system:dict:remove")]
        // [Log(Title = "字典类型", BusinessType = BusinessType.DELETE)]
        [HttpDelete, Route("{dictCode}")]
        public IHttpActionResult Remove(string dictCode)
        {
            long[] dictCodes = ZR.Common.Tools.SpitLongArrary(dictCode);

            return SUCCESS(_sysDictDataService.DeleteDictDataByIds(dictCodes));
        }
    }
}
