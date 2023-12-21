import { ElNotification, ElMessageBox } from 'element-plus'
import useSocketStore from '@/store/modules/socket'
import useUserStore from '@/store/modules/user'
import { webNotify } from '@/utils/index'
import notificationBellIcon from '@/components/Notice/bellIcon/index.vue'
export default {
  onMessage(connection) {
    connection.on(MsgType.M001, (data) => {
      useSocketStore().setOnlineUserNum(data)
    })

    connection.on(MsgType.M002, (data) => {})
    // 接受后台手动推送消息
    connection.on(MsgType.M003, (title, data) => {
      ElNotification({
        type: 'info',
        title: title,
        message: data,
        dangerouslyUseHTMLString: true,
        duration: 0
      })
      webNotify({ title: title, body: data })
    })
    // 接受系统通知/公告
    connection.on(MsgType.M004, (data) => {
      if (data.code == 200) {
        useSocketStore().setUnreadNoticeList(data.data.unReadNotifications)
        useSocketStore().setReadNoticeList(data.data.readNotifications)
        if (data.data.unReadNotifications.length > 0) {
          ElNotification({
            title: import.meta.env.VITE_APP_TITLE,
            message: `你有${data.data.unReadNotifications.length}条通知公告待查看`,
            icon: notificationBellIcon,
            duration: 0
          })
        }
      }
    })
    // 接受聊天数据
    connection.on(MsgType.M005, (data) => {
      const { fromUser, message } = data

      useSocketStore().setChat(data)

      if (data.userid != useUserStore().userId) {
        ElNotification({
          title: fromUser.nickName,
          message: message,
          type: 'success',
          duration: 3000
        })
      }
      webNotify({ title: fromUser.nickName, body: message })
    })
  },
  AllReadNotice(connection) {
    connection.invoke('AllReadNotice')
  },
  ReadNotice(connection, noticeId) {
    connection.invoke('ReadNotice', noticeId.toString())
  }
}
const MsgType = {
  M001: 'onlineNum',
  M002: 'connId',
  M003: 'receiveNotice',
  M004: 'moreNotice',
  M005: 'receiveChat'
}
