<template>
  <div class="app-container" style="position: absolute; left: 10px; right: 10px">
    <el-tabs v-model="activeTab">
      <el-tab-pane name="notice">
        <template #label>
          <el-badge :value="unreadNoticeList.length" style="line-height: 18px" :hidden="unreadNoticeList.length === 0"> 通知公告 </el-badge>
        </template>
        <el-divider />
        <el-empty :image-size="150" description="暂无通知公告" v-if="noticeList.length === 0" />
        <el-table :data="noticeList" height="600" :show-header="false" :cell-style="{ height: '40px' }" v-else>
          <el-table-column prop="noticeTitle" label="标题">
            <template #default="{ row }">
              <el-icon :size="20" color="#409EFF"><bell /></el-icon>
              <el-link
                :type="row.isRead ? 'primary' : 'danger'"
                style="margin-left: 8px"
                @click="viewNoticeDetail({ ...row, noticeType: NoticeType.notice })"
                >{{ row.noticeTitle }}</el-link
              >
            </template>
          </el-table-column>
          <!-- <el-table-column prop="noticeContent" label="内容">
            <template #default="{ row }">
              <div v-html="row.noticeContent"></div>
            </template>
          </el-table-column> -->
          <el-table-column prop="create_time" label="时间" width="180">
            <template #default="{ row }">
              {{ formatDate(row.create_time) }}
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
      <el-tab-pane name="aimMsg">
        <template #label>
          <el-badge :value="unreadAimList.length" style="line-height: 18px" :hidden="unreadAimList.length === 0"> 业务信息 </el-badge>
        </template>
        <el-divider />
        <el-empty :image-size="150" description="暂无业务信息" v-if="aimList.length === 0" />
        <el-table :data="aimList" height="600" :show-header="false" :cell-style="{ height: '40px' }" v-else>
          <el-table-column prop="noticeTitle" label="标题">
            <template #default="{ row }">
              <el-icon :size="20" color="#409EFF"><bell /></el-icon>
              <el-link
                :type="row.isRead ? 'primary' : 'danger'"
                style="margin-left: 8px"
                @click="viewNoticeDetail({ ...row, noticeType: NoticeType.notice })"
                >{{ row.noticeTitle }}</el-link
              >
            </template>
          </el-table-column>
          <!-- <el-table-column prop="noticeContent" label="内容">
            <template #default="{ row }">
              <div v-html="row.noticeContent"></div>
            </template>
          </el-table-column> -->
          <el-table-column prop="create_time" label="时间" width="180">
            <template #default="{ row }">
              {{ formatDate(row.create_time) }}
            </template>
          </el-table-column>
        </el-table>
      </el-tab-pane>
    </el-tabs>
    <el-popconfirm title="是否确定把全部内容消息设为已读？" :width="300" @confirm="onAllReadClick">
      <template #reference>
        <el-button type="primary" link class="left-top-button">全部标为已读</el-button>
      </template>
    </el-popconfirm>
    <noticeDialog v-model="noticeDialogOpen" :notice-model="info" />
  </div>
</template>

<script setup lang="ts">
import useSocketStore from '@/store/modules/socket'
import signalR from '@/signalr'
import noticeDialog from '@/components/Notice/noticeDialog/index.vue'
import { ElTable } from 'element-plus'
const route = useRoute()
onMounted(() => {
  if (route.query.activeTab) {
    activeTab.value = route.query.activeTab as string
  }
})
enum NoticeType {
  notice = 1,
  aimMsg = 2,
  chat = 3
}
const activeTab = ref('notice')
const readNoticeList = computed(() => {
  return useSocketStore().readNoticeList
})
const unreadNoticeList = computed(() => {
  return useSocketStore().unreadNoticeList
})
const noticeList = computed(() => {
  return readNoticeList.value
    .map((item: any) => ({
      ...item,
      isRead: true
    }))
    .concat(unreadNoticeList.value.map((item: any) => ({ ...item, isRead: false })))
})

const readAimList = computed(() => {
  return useSocketStore().readAimList
})
const unreadAimList = computed(() => {
  return useSocketStore().unreadAimList
})
const aimList = computed(() => {
  return readAimList.value
    .map((item: any) => ({
      ...item,
      isRead: true
    }))
    .concat(unreadAimList.value.map((item: any) => ({ ...item, isRead: false })))
})
interface noticeInfo {
  noticeId: string
  noticeTitle: string
  noticeContent: string
  noticeType: NoticeType
  isRead: boolean
  create_name: string
  create_time: string
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

function onAllReadClick() {
  if (activeTab.value === 'notice') {
    signalR.AllReadNotice()
  } else {
    signalR.AllReadAimMsg()
  }
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

<style scoped lang="scss">
.content-box {
  font-size: 13px;

  .content-box-item {
    //padding-top: 12px;
    &:last-of-type {
      padding-bottom: 12px;
    }

    .content-box-msg {
      color: #999999;
      margin-top: 5px;
      margin-bottom: 5px;
    }

    .content-box-time {
      color: #999999;
    }
  }
}
.left-top-button {
  position: absolute;
  top: 0;
  right: 0;
}
</style>
