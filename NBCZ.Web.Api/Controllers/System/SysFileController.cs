using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
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
    /// 文件存储Controller
    /// </summary>
    [Verify]
    [RoutePrefix("tool/file")]
    public class SysFileController : BaseController
    {
        /// <summary>
        /// 文件存储接口
        /// </summary>
        private readonly ISysFileService _sysFileService;

        public SysFileController(ISysFileService sysFileService)
        {
            _sysFileService = sysFileService;
        }

        /// <summary>
        /// 查询文件存储列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet, Route("list")]
        public IHttpActionResult QuerySysFile([FromUri] SysFileQueryDto parm)
        {
            var predicate = Expressionable.Create<SysFile>();

            predicate = predicate.AndIF(parm.BeginCreate_time != null, it => it.Create_time >= parm.BeginCreate_time);
            predicate = predicate.AndIF(parm.EndCreate_time != null, it => it.Create_time <= parm.EndCreate_time);
            predicate = predicate.AndIF(parm.StoreType != null, m => m.StoreType == parm.StoreType);
            predicate = predicate.AndIF(parm.FileId != null, m => m.Id == parm.FileId);

            var response = _sysFileService.GetPages(predicate.ToExpression(), parm, x => x.Id, OrderByType.Desc);
            return SUCCESS(response);
        }

        /// <summary>
        /// 查询文件存储详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet, Route("{Id}")]
        // [ActionPermissionFilter(Permission = "tool:file:query")]
        public IHttpActionResult GetSysFile(long Id)
        {
            var response = _sysFileService.GetFirst(x => x.Id == Id);

            return SUCCESS(response);
        }

        /// <summary>
        /// 删除文件存储
        /// </summary>
        /// <returns></returns>
        [HttpDelete, Route("{ids}")]
        // [ActionPermissionFilter(Permission = "tool:file:delete")]
        // [Log(Title = "文件存储", BusinessType = BusinessType.DELETE)]
        public async Task<IHttpActionResult> DeleteSysFile(string ids)
        {
            long[] idsArr = Tools.SpitLongArrary(ids);
            if (idsArr.Length <= 0) { return ToResponse(ApiResult.Error($"删除失败Id 不能为空")); }

            var response = await _sysFileService.DeleteSysFileAsync(idsArr);

            return ToResponse(response);
        }

        // /// <summary>
        // /// 文件存储导出
        // /// </summary>
        // /// <returns></returns>
        // // [Log(BusinessType = BusinessType.EXPORT, IsSaveResponseData = false, Title = "文件存储")]
        // [HttpGet, Route("export")]
        // // [ActionPermissionFilter(Permission = "tool:file:export")]
        // public IHttpActionResult Export()
        // {
        //     var list = _sysFileService.GetAll();
        //
        //     string sFileName = ExportExcel(list, "SysFile", "文件存储");
        //     return SUCCESS(new { path = "/export/" + sFileName, fileName = sFileName });
        // }
        
        // /// <summary>
        // /// 通过文件ID下载文件
        // /// </summary>
        // /// <param name="file"></param>
        // /// <returns></returns>
        // [HttpPost, Route("download")]
        // public IHttpActionResult Download([FromBody] SysFile file)
        // {
        //     var sysFile = _sysFileService.Queryable().Where(it => it.Id == file.Id).First();
        //     if (sysFile == null)
        //     {
        //         return NoContent();
        //     }
        //
        //     Stream? stream = null;
        //     switch ((StoreType)Enum.Parse(typeof(StoreType), sysFile.StoreType.ToString() ?? string.Empty))
        //     {
        //         case StoreType.LOCAL:
        //             stream = sysFile.IsEncrypted == "1" ? _sysFileService.DecryptSysFileStream(sysFile.FileUrl) 
        //                 : new FileStream(sysFile.FileUrl, FileMode.Open);
        //             break;
        //         case StoreType.ALIYUN:
        //             stream = AliyunOssHelper.DownloadFile(sysFile.FileUrl, "", sysFile.IsEncrypted == "1");
        //             break;
        //         default:
        //             throw new CustomException("不支持的存储类型");
        //     }
        //     if (stream != null)
        //     {
        //         return new FileStreamResult(stream, "application/octet-stream")
        //         {
        //             FileDownloadName = sysFile.RealName
        //         };
        //     }
        //     else
        //     {
        //         return NoContent();
        //     }
        // }
    }
}