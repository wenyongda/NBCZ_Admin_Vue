using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Infrastructure.Extensions;
using Mapster;
using NBCZ.BLL.Services.IService;
using NBCZ.Common.CustomException;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using SqlSugar;
using WebGrease;
using ZR.Common;
using ZR.Model;

namespace NBCZ.Web.Api.Controllers.System
{
    /// <summary>
    /// 系统通知
    /// </summary>
    // [Verify]
    [Route("system/notice")]
    public class SysNoticeController : BaseController
    {
        /// <summary>
        /// 通知公告表接口
        /// </summary>
        private readonly ISysNoticeService _sysNoticeService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ISysNoticeLogService _sysNoticeLogService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public SysNoticeController(ISysNoticeService sysNoticeService, IHubContext<MessageHub> hubContext, ISysNoticeLogService sysNoticeLogService, IWebHostEnvironment webHostEnvironment)
        {
            _sysNoticeService = sysNoticeService;
            _hubContext = hubContext;
            _sysNoticeLogService = sysNoticeLogService;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// 查询通知公告表列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("queryNotice")]
        public IHttpActionResult QueryNotice([FromQuery] SysNoticeQueryDto parm)
        {
            var predicate = Expressionable.Create<SysNotice>();

            predicate = predicate.And(m => m.Status == 0);
            var response = _sysNoticeService.GetPages(predicate.ToExpression(), parm);
            return SUCCESS(response);
        }

        /// <summary>
        /// 查询通知公告表列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        [ActionPermissionFilter(Permission = "system:notice:list")]
        public IHttpActionResult QuerySysNotice([FromQuery] SysNoticeQueryDto parm)
        {
            PagedInfo<SysNotice> response = _sysNoticeService.GetPageList(parm);
            return SUCCESS(response);
        }

        /// <summary>
        /// 查询通知公告表详情
        /// </summary>
        /// <param name="noticeId"></param>
        /// <returns></returns>
        [HttpGet("{noticeId}")]
        public IHttpActionResult GetSysNotice(long noticeId)
        {
            var response = _sysNoticeService.GetFirst(x => x.NoticeId == noticeId);

            return SUCCESS(response);
        }

        /// <summary>
        /// 添加通知公告表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionPermissionFilter(Permission = "system:notice:add")]
        [Log(Title = "发布通告", BusinessType = BusinessType.INSERT)]
        public async Task<IHttpActionResult> AddSysNotice([FromBody] SysNoticeDto parm)
        {
            var modal = parm.Adapt<SysNotice>().ToCreate(HttpContext);
            modal.Create_by = HttpContext.GetUId();
            modal.Create_name = HttpContext.GetNickName();
            modal.Create_time = DateTime.Now;
            
            // int result = _SysNoticeService.Insert(modal, it => new
            // {
            //     it.NoticeTitle,
            //     it.NoticeType,
            //     it.NoticeContent,
            //     it.Status,
            //     it.Remark,
            //     it.Create_by,
            //     it.Create_time
            // });
            
            var result = await _sysNoticeService
                .Insertable(modal)
                .ExecuteReturnSnowflakeIdAsync();
            var scheme = HttpContext.Request.Scheme + "://";
            var serverIP = HttpContext.Request.Host.Value;
            if (_webHostEnvironment.IsProduction())
            {
                var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
                var ip = host.AddressList
                    .FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork);
                serverIP = ip + ":" + Request.HttpContext.Connection.LocalPort;//获取服务器IP
            }
            var url = scheme + serverIP;
            var hubConnection = new HubConnectionBuilder().WithUrl(url + "/msghub").Build();
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("SendNoticeToOnlineUsers", result, true);
            await hubConnection.StopAsync();
            return SUCCESS(result);
        }

        /// <summary>
        /// 更新通知公告表
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ActionPermissionFilter(Permission = "system:notice:update")]
        [Log(Title = "修改公告", BusinessType = BusinessType.UPDATE)]
        public async Task<IHttpActionResult> UpdateSysNotice([FromBody] SysNoticeDto parm)
        {
            var config = new TypeAdapterConfig();
            config.ForType<SysNoticeDto, SysNotice>()
                .Map(dest => dest.NoticeId, src => src.NoticeId.ParseToLong());
            var model = parm.Adapt<SysNotice>(config).ToUpdate(HttpContext);
            var nowDate = DateTime.Now;
            var response = _sysNoticeService.Update(w => w.NoticeId == model.NoticeId, it => new SysNotice()
            {
                NoticeTitle = model.NoticeTitle,
                NoticeType = model.NoticeType,
                NoticeContent = model.NoticeContent,
                Status = model.Status,
                Remark = model.Remark,
                Update_by = HttpContext.GetUId(),
                Update_name = HttpContext.GetNickName(),
                Update_time = nowDate,
                Create_time = nowDate
            });
            
            var scheme = HttpContext.Request.Scheme + "://";
            var serverIP = HttpContext.Request.Host.Value;
            if (_webHostEnvironment.IsProduction())
            {
                var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
                var ip = host.AddressList
                    .FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork);
                serverIP = ip + ":" + Request.HttpContext.Connection.LocalPort;//获取服务器IP
            }
            var url = scheme + serverIP;
            var hubConnection = new HubConnectionBuilder().WithUrl(url + "/msghub").Build();
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("SendNoticeToOnlineUsers", model.NoticeId, true);
            await hubConnection.StopAsync();
            return SUCCESS(response);
        }
        /// <summary>
        /// 发送通知公告表
        /// </summary>
        /// <returns></returns>
        [HttpPut("send/{noticeId}")]
        [ActionPermissionFilter(Permission = "system:notice:update")]
        [Log(Title = "发送通知公告", BusinessType = BusinessType.OTHER)]
        public IHttpActionResult SendNotice(long noticeId = 0)
        {
            if (noticeId <= 0)
            {
                throw new CustomException("请求实体不能为空");
            }
            var response = _sysNoticeService.GetFirst(x => x.NoticeId == noticeId);
            if (response != null && response.Status == 0)
            {
                _hubContext.Clients.All.SendAsync(HubsConstant.ReceiveNotice, response.NoticeTitle, response.NoticeContent);
            }
            return SUCCESS(response);
        }

        /// <summary>
        /// 删除通知公告表
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{ids}")]
        [ActionPermissionFilter(Permission = "system:notice:delete")]
        [Log(Title = "通知公告", BusinessType = BusinessType.DELETE)]
        public async Task<IHttpActionResult> DeleteSysNotice(string ids)
        {
            var idsArr = Tools.SpitLongArrary(ids);
            if (idsArr.Length <= 0) { return ToResponse(ApiResult.Error($"删除失败Id 不能为空")); }

            var response = _sysNoticeService.Delete(idsArr);
            await _sysNoticeLogService.Deleteable()
                .Where(it => idsArr.Contains(it.NoticeId))
                .ExecuteCommandAsync();
            
            var scheme = HttpContext.Request.Scheme + "://";
            var serverIP = HttpContext.Request.Host.Value;
            if (_webHostEnvironment.IsProduction())
            {
                var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
                var ip = host.AddressList
                    .FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork);
                serverIP = ip + ":" + Request.HttpContext.Connection.LocalPort;//获取服务器IP
            }
            var url = scheme + serverIP;
            var hubConnection = new HubConnectionBuilder().WithUrl(url + "/msghub").Build();
            await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("SendNoticeToOnlineUsers", null, false);
            await hubConnection.StopAsync();
            return SUCCESS(response);
        }
    }
}