const useSocketStore = defineStore('socket', {
  state: () => ({
    onlineNum: 0,
    onlineUsers: [],
    noticeList: [],
    noticeDot: false,
    readNoticeList: [],
    unreadNoticeList: [],
    readAimList: [],
    unreadAimList: []
  }),
  actions: {
    //更新在线人数
    setOnlineUserNum(num) {
      this.onlineNum = num
    },
    // 更新系统通知
    setNoticeList(data) {
      this.noticeList = data
      this.noticeDot = data.length > 0
    },
    setReadNoticeList(data) {
      this.readNoticeList = data
    },
    setUnreadNoticeList(data) {
      this.unreadNoticeList = data
      this.noticeDot = data.length > 0 || this.unreadAimList.length > 0
    },
    setReadAimList(data) {
      this.readAimList = data
    },
    setUnreadAimList(data) {
      this.unreadAimList = data
      this.noticeDot = data.length > 0 || this.unreadNoticeList.length > 0
    },
    // setOnlineUsers(data) {
    //   const { onlineNum, users } = data
    //   this.onlineUsers = users
    //   this.onlineNum = onlineNum
    // },
    sendChat(data) {
      const { proxy } = getCurrentInstance()
      console.log(data)
    }
  }
})
export default useSocketStore
