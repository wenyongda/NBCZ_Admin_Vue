using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Infrastructure.Extensions;
using Mapster;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using NBCZ.BLL.Services.IService;
using NBCZ.Common;
using NBCZ.Model;
using NBCZ.Model.System;
using NBCZ.Model.System.Dto;
using ZR.ServiceCore.Signalr;

namespace NBCZ.BLL.Signalr
{
    /// <summary>
    /// msghub
    /// </summary>
    public class MessageHub : Hub
    {
        //创建用户集合，用于存储所有链接的用户数据
        public static readonly List<OnlineUsers> OnlineClients = new List<OnlineUsers>();
        public static List<OnlineUsers> Users = new List<OnlineUsers>();
        //private readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly ISysNoticeService _sysNoticeService;
        private readonly ISysNoticeLogService _sysNoticeLogService;

        public MessageHub(ISysNoticeService noticeService, ISysNoticeLogService sysNoticeLogService)
        {
            _sysNoticeService = noticeService;
            _sysNoticeLogService = sysNoticeLogService;
        }

        private ApiResult SendNotice()
        {
            var result = _sysNoticeService.GetSysNotices();

            return new ApiResult(200, "success", result);
        }
        
        private ApiResult SendNotice(object notice)
        {
            return new ApiResult(200, "success", notice);
        }

        #region 客户端连接

        /// <summary>
        /// 客户端连接的时候调用
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnected()
        {
            var context = Context.Request.GetHttpContext().ApplicationInstance.Context;
            var name = HttpContextExtension.GetName(context);
            // var ip = HttpContextExtension.GetClientUserIp(context);
            // var ip_info = IpTool.Search(ip);
            // ClientInfo clientInfo = HttpContextExtension.GetClientInfo(context);
            // string device = clientInfo.ToString();
            string qs = HttpContextExtension.GetQueryString(context);
            var query = HttpUtility.ParseQueryString(qs);
            string from = query.Get("from") ?? "web";
            string clientId = query.Get("clientId");

            long userid = HttpContextExtension.GetUId(context);
            // string uuid = device + userid + ip;
            string uuid = userid.ToString();
            var user = OnlineClients.Any(u => u.ConnnectionId == Context.ConnectionId);
            var user2 = OnlineClients.Any(u => u.Uuid == uuid);

            //判断用户是否存在，否则添加集合!user2 && !user && 
            if (!user2 && !user && Context.User.Identity.IsAuthenticated)
            {
                OnlineUsers onlineUser = new OnlineUsers(Context.ConnectionId, name, userid)
                {
                    // Location = ip_info.City,
                    Uuid = uuid,
                    Platform = from,
                    ClientId = clientId ?? Context.ConnectionId
                };
                OnlineClients.Add(onlineUser);
                // Log.WriteLine(msg: $"{name},{Context.ConnectionId}连接服务端success，当前已连接{OnlineClients.Count}个");
                //Clients.All.SendAsync("welcome", $"欢迎您：{name},当前时间：{DateTime.Now}");
                
                var noticeRes = (List<SysNotice>) SendNotice()[ApiResult.DATA_TAG];
                // 获取当前在线用户的通知日志记录id
                var unreadAndReadNoticeIds = await _sysNoticeLogService.Queryable()
                    .Where(it => it.UserId == userid)
                    .Select(it => it.NoticeId)
                    .ToListAsync();
                
                // 当前在线用户的日志通知记录里如果有不存在的通知记录则添加未读记录
                foreach (var notice in noticeRes.Select(it => it.NoticeId).ToList().Except(unreadAndReadNoticeIds))
                {
                    await _sysNoticeLogService.Insertable(new SysNoticeLog
                    {
                        NoticeId = notice,
                        UserId = userid,
                        Status = SysNoticeLogStatus.Unread
                    }).ExecuteReturnSnowflakeIdAsync();
                }
                // 当前在线用户的日志通知记录里如果有冗余的通知记录则删除已读记录
                // foreach (var notice in unreadAndReadNoticeIds.Except(noticeRes.Select(it => it.NoticeId).ToList()))
                // {
                //     _sysNoticeLogService.Deleteable()
                //         .Where(it => it.UserId == userid && it.NoticeId == notice)
                //         .ExecuteCommand();
                // }
                var newUnReadNotificationIdsAndReadNotifications = await _sysNoticeLogService.Queryable()
                    .Where(it => it.UserId == userid)
                    .ToListAsync();
                var newUnReadNotificationIds = newUnReadNotificationIdsAndReadNotifications
                    .Where(it => it.Status == SysNoticeLogStatus.Unread)
                    .Select(it => it.NoticeId)
                    .ToList();
                var newReadNotificationIds = newUnReadNotificationIdsAndReadNotifications
                    .Where(it => it.Status == SysNoticeLogStatus.Read)
                    .Select(it => it.NoticeId)
                    .ToList();
                var config = new TypeAdapterConfig();
                config.ForType<SysNotice, SysNoticeDto>()
                    .Map(dest => dest.NoticeId, src => src.NoticeId.ToString());
                var newUnReadNotifications = noticeRes.Where(it => 
                    newUnReadNotificationIds.Contains(it.NoticeId)).ToList().Adapt<List<SysNoticeDto>>(config);
                var newReadNotifications = noticeRes.Where(it => 
                    newReadNotificationIds.Contains(it.NoticeId)).ToList().Adapt<List<SysNoticeDto>>(config);
                await Clients.Caller.SendAsync(HubsConstant.MoreNotice, SendNotice(new
                {
                    unReadNotifications = newUnReadNotifications,
                    readNotifications = newReadNotifications
                }));
                // Clients.Caller.SendAsync(HubsConstant.MoreNotice, SendNotice());
                // Clients.Caller.SendAsync(HubsConstant.ConnId, onlineUser.ConnnectionId);
            }
            OnlineUsers userInfo = GetUserById(userid);
            if (userInfo == null)
            {
                userInfo = new OnlineUsers() { Userid = userid, Name = name, LoginTime = DateTime.Now };
                Users.Add(userInfo);
            }
            else
            {
                if (userInfo.LoginTime <= Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                {
                    userInfo.LoginTime = DateTime.Now;
                    userInfo.TodayOnlineTime = 0;
                }
                var clientUser = OnlineClients.Find(x => x.Userid == userid);
                userInfo.TodayOnlineTime += Math.Round(clientUser?.OnlineTime ?? 0, 2);
            }
            //给当前所有登录当前账号的用户下发登录时长
            var connIds = OnlineClients.Where(f => f.Userid == userid).ToList();
            userInfo.ClientNum = connIds.Count;

            // Clients.Clients(connIds.Select(f => f.ConnnectionId)).SendAsync("onlineInfo", userInfo);

            // Log.WriteLine(ConsoleColor.Blue, msg: $"用户{name}已连接，今日已在线{userInfo?.TodayOnlineTime}分钟，当前已连接{OnlineClients.Count}个");
            //给所有用户更新在线人数
            await Clients.All.SendAsync(HubsConstant.OnlineNum, new
            {
                num = OnlineClients.Count, onlineClients = OnlineClients
            });
            await base.OnConnected();
        }
        
        /// <summary>
        /// 连接终止时调用。
        /// </summary>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            var user = OnlineClients.Where(p => p.ConnnectionId == Context.ConnectionId).FirstOrDefault();
            if (user != null)
            {
                OnlineClients.Remove(user);
                //给所有用户更新在线人数
                Clients.All.SendAsync(HubsConstant.OnlineNum, new
                {
                    num = OnlineClients.Count,
                    onlineClients = OnlineClients,
                    leaveUser = user
                });

                //累计用户时长
                OnlineUsers userInfo = GetUserById(user.Userid);
                if (userInfo != null)
                {
                    userInfo.TodayOnlineTime += user?.OnlineTime ?? 0;
                }
                // Log.WriteLine(ConsoleColor.Red, msg: $"用户{user?.Name}离开了,已在线{userInfo?.TodayOnlineTime}分，当前已连接{OnlineClients.Count}个");
            }
            return base.OnDisconnected(stopCalled);
        }

        #endregion

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="toConnectId">对方链接id</param>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HubMethodName("sendMessage")]
        public async Task SendMessage(string toConnectId, long toUserId, string message)
        {
            var context = Context.Request.GetHttpContext().ApplicationInstance.Context;
            var userName = HttpContextExtension.GetName(context);
            long userid = HttpContextExtension.GetUId(context);
            var toUserList = OnlineClients.Where(p => p.Userid == toUserId);
            var toUserInfo = toUserList.FirstOrDefault();
            IList<string> sendToUser = toUserList.Select(x => x.ConnnectionId).ToList();
            sendToUser.Add(GetConnectId());
            if (toUserInfo != null)
            {
                await Clients.Clients(sendToUser)
                    .SendAsync("receiveChat", new
                    {
                        msgType = 0,//文本
                        chatid = Guid.NewGuid().ToString(),
                        userName,
                        userid,
                        toUserName = toUserInfo.Name,
                        toUserid = toUserInfo.Userid,
                        message,
                        chatTime = DateTime.Now
                    });
            }
            else
            {
                //TODO 存储离线消息
                Console.WriteLine($"{toUserId}不在线");
            }

            Console.WriteLine($"用户{userName}对{toConnectId}-{toUserId}说：{message}");
        }

        /// <summary>
        /// 对当前在线用户累加新通知公告
        /// </summary>
        /// <param name="noticeId"></param>
        /// <param name="enableDelete"></param>
        public async Task SendNoticeToOnlineUsers(long? noticeId, bool enableDelete = true)
        {
            if (enableDelete)
            {
                // 防止日志记录有冗余数据
                await _sysNoticeLogService.Deleteable()
                    .Where(it => it.NoticeId == noticeId)
                    .ExecuteCommandAsync();
                // 获取所有通知
                var noticeRes = (List<SysNotice>) SendNotice()[ApiResult.DATA_TAG];
                
                foreach (var onlineUser in OnlineClients)
                {
                    var userid = onlineUser.Userid;
                    
                    var sysNoticeLogStore = await _sysNoticeLogService
                        .Storageable(new SysNoticeLog(noticeId!.Value, onlineUser.Userid, SysNoticeLogStatus.Unread))
                        .WhereColumns(it => new { it.NoticeId, it.UserId })
                        .ToStorageAsync();
                    await sysNoticeLogStore
                        .AsInsertable
                        .ExecuteReturnSnowflakeIdAsync();
                    await sysNoticeLogStore
                        .AsUpdateable
                        .ExecuteCommandAsync();
                    // 获取当前在线用户的通知日志记录id
                    var unreadAndReadNotices = await _sysNoticeLogService.Queryable()
                        .Where(it => it.UserId == userid)
                        .ToListAsync();
                    var config = new TypeAdapterConfig();
                    config.ForType<SysNotice, SysNoticeDto>()
                        .Map(dest => dest.NoticeId, src => src.NoticeId.ToString());
                    var unReadNotifications = noticeRes.Where(it => 
                            unreadAndReadNotices.Where(o => o.Status == SysNoticeLogStatus.Unread)
                            .Select(o => o.NoticeId).Contains(it.NoticeId)).ToList()
                            .Adapt<List<SysNoticeDto>>(config);
                    var readNotifications = noticeRes.Where(it => 
                            unreadAndReadNotices.Where(o => o.Status == SysNoticeLogStatus.Read)
                            .Select(o => o.NoticeId).Contains(it.NoticeId)).ToList()
                            .Adapt<List<SysNoticeDto>>(config);
                    
                    await Clients.Client(onlineUser.ConnnectionId).SendAsync(HubsConstant.MoreNotice, 
                        SendNotice(new
                        {
                            unReadNotifications,
                            readNotifications
                        }));
                }
            }
            else
            {
                // 获取所有通知
                var noticeRes = (List<SysNotice>) SendNotice()[ApiResult.DATA_TAG];
                foreach (var onlineUser in OnlineClients)
                {
                    var userid = onlineUser.Userid;
                    // 获取当前在线用户的通知日志记录id
                    var unreadAndReadNotices = await _sysNoticeLogService.Queryable()
                        .Where(it => it.UserId == userid)
                        .ToListAsync();
                    var config = new TypeAdapterConfig();
                    config.ForType<SysNotice, SysNoticeDto>()
                        .Map(dest => dest.NoticeId, src => src.NoticeId.ToString());
                    var unReadNotifications = noticeRes.Where(it => 
                            unreadAndReadNotices.Where(o => o.Status == SysNoticeLogStatus.Unread)
                            .Select(o => o.NoticeId).Contains(it.NoticeId)).ToList()
                            .Adapt<List<SysNoticeDto>>(config);
                    var readNotifications = noticeRes.Where(it => 
                            unreadAndReadNotices.Where(o => o.Status == SysNoticeLogStatus.Read)
                            .Select(o => o.NoticeId).Contains(it.NoticeId)).ToList()
                            .Adapt<List<SysNoticeDto>>(config);
                    await Clients.Client(onlineUser.ConnnectionId).SendAsync(HubsConstant.MoreNotice, 
                        SendNotice(new
                        {
                            unReadNotifications,
                            readNotifications
                        }));
                }
            }

            // var onlineUsers = ClientUsers.Where(it => it.ConnnectionId != Context.ConnectionId);
        }
        
        /// <summary>
        /// 全部标为已读
        /// </summary>
        [HubMethodName("AllReadNotice")]
        public async Task AllReadNotice()
        {
            var context = Context.Request.GetHttpContext().ApplicationInstance.Context;
            var userId = HttpContextExtension.GetUId(context);
            var unreadNotificationIds = await _sysNoticeLogService.Queryable()
                .Where(it => it.Status == SysNoticeLogStatus.Unread && it.UserId == userId)
                .Select(it => it.NoticeId)
                .ToListAsync();
            foreach (var notice in unreadNotificationIds)
            {
                await _sysNoticeLogService.Updateable(new SysNoticeLog
                    {
                        Status = SysNoticeLogStatus.Read
                    })
                    .IgnoreColumns(it => new { it.NoticeId, it.UserId })
                    .Where(it => it.UserId == userId && it.NoticeId == notice
                                                       && it.Status == SysNoticeLogStatus.Unread)
                    .ExecuteCommandAsync();
            }
            var newNotifications = await _sysNoticeLogService.Queryable()
                .Where(it => it.UserId == userId)
                .ToListAsync();
            var newUnReadNotificationIds = newNotifications
                .Where(it => it.Status == SysNoticeLogStatus.Unread)
                .Select(it => it.NoticeId)
                .ToList();
            var newReadNotificationIds = newNotifications
                .Where(it => it.Status == SysNoticeLogStatus.Read)
                .Select(it => it.NoticeId)
                .ToList();
            var noticeRes = (List<SysNotice>) SendNotice()[ApiResult.DATA_TAG];
            var newUnReadNotifications = noticeRes.Where(it => 
                newUnReadNotificationIds.Contains(it.NoticeId)).ToList();
            var newReadNotifications = noticeRes.Where(it => 
                newReadNotificationIds.Contains(it.NoticeId)).ToList();
            await Clients.Caller.SendAsync(HubsConstant.MoreNotice, SendNotice(new
            {
                unReadNotifications = newUnReadNotifications,
                readNotifications = newReadNotifications
            }));
        }

        /// <summary>
        /// 标记已读
        /// </summary>
        /// <param name="noticeId"></param>
        [HubMethodName("ReadNotice")]
        public async Task ReadNotice(string noticeId)
        {
            var context = Context.Request.GetHttpContext().ApplicationInstance.Context;
            var userid = HttpContextExtension.GetUId(context);
            await _sysNoticeLogService.Updateable(new SysNoticeLog
                {
                    Status = SysNoticeLogStatus.Read
                })
                .IgnoreColumns(it => new { it.NoticeId, it.UserId })
                .Where(it => it.NoticeId == noticeId.ParseToLong() && it.UserId == userid
                                                        && it.Status == SysNoticeLogStatus.Unread)
                .ExecuteCommandAsync();
            var newNotifications = await _sysNoticeLogService.Queryable()
                .Where(it => it.UserId == userid)
                .ToListAsync();
            var newUnReadNotificationIds = newNotifications
                .Where(it => it.Status == SysNoticeLogStatus.Unread)
                .Select(it => it.NoticeId)
                .ToList();
            var newReadNotificationIds = newNotifications
                .Where(it => it.Status == SysNoticeLogStatus.Read)
                .Select(it => it.NoticeId)
                .ToList();
            var noticeRes = (List<SysNotice>) SendNotice()[ApiResult.DATA_TAG];
            var newUnReadNotifications = noticeRes.Where(it => 
                newUnReadNotificationIds.Contains(it.NoticeId)).ToList();
            var newReadNotifications = noticeRes.Where(it => 
                newReadNotificationIds.Contains(it.NoticeId)).ToList();
            await Clients.Caller.SendAsync(HubsConstant.MoreNotice, SendNotice(new
            {
                unReadNotifications = newUnReadNotifications,
                readNotifications = newReadNotifications
            }));
        }
        
        private OnlineUsers GetUserByConnId(string connId)
        {
            return OnlineClients.Where(p => p.ConnnectionId == connId).FirstOrDefault();
        }
        private static OnlineUsers GetUserById(long userid)
        {
            return Users.Where(f => f.Userid == userid).FirstOrDefault();
        }

        /// <summary>
        /// 移动端使用获取链接id
        /// </summary>
        /// <returns></returns>
        [HubMethodName("getConnId")]
        public string GetConnectId()
        {
            return Context.ConnectionId;
        }

        // /// <summary>
        // /// 退出其他设备登录
        // /// </summary>
        // /// <returns></returns>
        // [HubMethodName("logOut")]
        // public async Task LogOut()
        // {
        //     var singleLogin = AppSettings.Get<bool>("singleLogin");
        //     long userid = HttpContextExtension.GetUId(App.HttpContext);
        //     if (singleLogin)
        //     {
        //         var onlineUsers = OnlineClients.Where(p => p.ConnnectionId != Context.ConnectionId && p.Userid == userid);
        //         await Clients.Clients(onlineUsers.Select(x => x.ConnnectionId).ToList())
        //             .SendAsync("logOut");
        //     }
        // }
    }
}
