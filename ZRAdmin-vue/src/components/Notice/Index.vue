<template>
  <div>
    <el-popover placement="bottom" trigger="hover" width="400px" popper-class="el-popover-pupop-user-news">
      <template #reference>
        <el-badge :is-dot="noticeDot" style="line-height: 18px">
          <el-icon><bell /></el-icon>
        </el-badge>
      </template>
      <div class="layout-navbars-breadcrumb-user-news">
        <el-tabs v-model="activeTab">
          <el-tab-pane name="notice">
            <template #label>
              <el-badge :value="unreadNoticeList.length" style="line-height: 18px" :hidden="unreadNoticeList.length === 0"> 通知公告 </el-badge>
            </template>
            <div class="content-box">
              <template v-if="unreadNoticeList.length > 0">
                <div
                  class="content-box-item"
                  v-for="item in unreadNoticeList"
                  @click="viewNoticeDetail({ ...item, noticeType: NoticeType.notice })">
                  <el-icon :size="30" color="#409EFF"><bell /></el-icon>
                  <div class="content">
                    <div class="title">{{ item.noticeTitle }}</div>
                    <el-text class="content-box-time" type="info">{{ formatDate(item.create_time) }}</el-text>
                  </div>
                </div>
              </template>
              <el-empty v-if="unreadNoticeList.length <= 0" :image-size="60" description="暂无未读通知公告"></el-empty></div
          ></el-tab-pane>
          <el-tab-pane name="aimMsg">
            <template #label>
              <el-badge :value="unreadAimList.length" style="line-height: 18px" :hidden="unreadAimList.length === 0"> 业务信息 </el-badge>
            </template>
            <div class="content-box">
              <template v-if="unreadAimList.length > 0">
                <div class="content-box-item" v-for="item in unreadAimList" @click="viewNoticeDetail({ ...item, noticeType: NoticeType.aimMsg })">
                  <el-icon :size="30" color="#409EFF"><bell /></el-icon>
                  <div class="content">
                    <div class="title">{{ item.noticeTitle }}</div>
                    <el-text class="content-box-time" type="info">{{ formatDate(item.create_time) }}</el-text>
                  </div>
                </div>
              </template>
              <el-empty v-if="unreadAimList.length <= 0" :image-size="60" description="暂无未读业务信息"></el-empty>
            </div>
          </el-tab-pane>
        </el-tabs>
        <div class="foot-box">
          <el-popconfirm title="是否确定把全部内容消息设为已读？" :teleported="false" :width="300" @confirm="onAllReadClick">
            <template #reference>
              <el-button type="primary" link>全部标为已读</el-button>
            </template>
          </el-popconfirm>
          <el-link type="primary" @click="viewMoreNotice">
            查看更多<el-icon class="el-icon--right"><ArrowRight /></el-icon>
          </el-link>
        </div>
      </div>
    </el-popover>

    <noticeDialog v-model="noticeDialogOpen" :notice-model="info" />
  </div>
</template>

<script setup name="noticeIndex" lang="ts">
import useSocketStore from '@/store/modules/socket'
import signalR from '@/signalr'
import noticeDialog from './noticeDialog/index.vue'
const router = useRouter()
const activeTab = ref('notice')
// 小红点
const newsDot = ref(false)

enum NoticeType {
  notice = 1,
  aimMsg = 2,
  chat = 3
}
interface noticeInfo {
  noticeId: string
  noticeTitle: string
  noticeContent: string
  noticeType: NoticeType
  isRead: boolean
  create_name: string
  create_time: string
}
const noticeDot = computed(() => {
  return useSocketStore().noticeDot
})
const readNoticeList = computed(() => {
  return useSocketStore().readNoticeList
})
const unreadNoticeList = computed<any[]>(() => {
  return useSocketStore().unreadNoticeList
})
const noticeList = computed<any[]>(() => {
  return readNoticeList.value
    .map((item: any) => ({
      ...item,
      isRead: true
    }))
    .concat(unreadNoticeList.value.map((item) => ({ ...item, isRead: false })))
})
const readAimList = computed<any[]>(() => {
  return useSocketStore().readAimList
})
const unreadAimList = computed<any[]>(() => {
  return useSocketStore().unreadAimList
})
const aimList = computed<any[]>(() => {
  return readAimList.value
    .map((item: any) => ({
      ...item,
      isRead: true
    }))
    .concat(unreadAimList.value.map((item) => ({ ...item, isRead: false })))
})
// 全部已读点击
function onAllReadClick() {
  if (activeTab.value === 'notice') {
    signalR.AllReadNotice()
  } else {
    signalR.AllReadAimMsg()
  }
}
const info = ref<noticeInfo>({
  noticeId: '',
  noticeTitle: '',
  noticeContent: '',
  noticeType: NoticeType.notice,
  isRead: false,
  create_name: '',
  create_time: ''
})
const noticeDialogOpen = ref(false)
const viewNoticeDetail = (notice: noticeInfo) => {
  noticeDialogOpen.value = true
  info.value = notice
}
// 前往通知中心点击
const viewMoreNotice = () => {
  router.push({ path: '/notice/center', query: { activeTab: activeTab.value } })
}
const formatDate = (dateTime) => {
  const dateObj = new Date(dateTime)
  const options: any = {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
    // hour: '2-digit',
    // minute: '2-digit',
    // second: '2-digit'
  }
  return dateObj.toLocaleDateString('zh-CN', options)
}
</script>

<style lang="scss">
.head-box {
  display: flex;
  border-bottom: 1px solid #ebeef5;
  box-sizing: border-box;
  color: #333333;
  justify-content: space-between;
  height: 35px;
  align-items: center;
  .head-box-btn {
    color: #1890ff;
    font-size: 13px;
    cursor: pointer;
    opacity: 0.8;
    &:hover {
      opacity: 1;
    }
  }
}
.content-box {
  font-size: 13px;
  min-height: 160px;
  max-height: 230px;
  overflow: auto;

  .content-box-item {
    display: flex;
    margin-bottom: 20px;
    cursor: pointer;
    &:hover {
      color: var(--el-color-primary);
    }

    &:last-of-type {
      padding-bottom: 12px;
    }
    .content {
      margin-left: 8px;
      width: 100%;
      .name {
        color: var(--el-color-primary);
      }
    }
    .icon {
      width: 30px;
      height: 30px;
      margin: 2px 10px 0 0;
    }
    .content-box-time {
      margin-top: 3px;
      text-align: right;
      display: block;
    }
  }
  .content-box-empty {
    height: 260px;
    display: flex;
    .content-box-empty-margin {
      margin: auto;
      text-align: center;
      i {
        font-size: 60px;
      }
    }
  }
}
//.foot-box {
//  height: 35px;
//  color: #1890ff;
//  font-size: 13px;
//  cursor: pointer;
//  opacity: 0.8;
//  display: flex;
//  align-items: center;
//  justify-content: center;
//  border-top: 1px solid #ebeef5;
//  &:hover {
//    opacity: 1;
//  }
//}
.foot-box {
  display: flex;
  justify-content: space-between;
}
:deep(.el-empty__description p) {
  font-size: 13px;
}
.head-box-title {
  color: var(--base-color-white);
}
</style>
