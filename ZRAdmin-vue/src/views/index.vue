<template>
  <div class="dashboard-editor-container home">
    <el-row :gutter="10">
      <!-- 左侧区域 -->
      <el-col :lg="16" :md="24">
        <el-card style="margin: 5px 0">
          <!-- 设置显示项目 -->
          <div style="position: relative; height: 45px; border-bottom: 1px solid var(--el-card-border-color)">
            <el-popover placement="bottom-end" trigger="click">
              <template #reference>
                <el-button circle title="设置显示项目" style="position: absolute; top: 0; right: 0">
                  <el-icon><Tools /></el-icon>
                </el-button>
              </template>
              <el-checkbox-group v-model="checkboxGroup">
                <el-checkbox v-for="(item, index) in checkBoxArray" :label="item.label" :key="index" />
              </el-checkbox-group>
            </el-popover>
          </div>
          <el-scrollbar view-style="overflow: hidden;" height="calc(100vh - 255px)">
            <!-- 业务日历 -->
            <el-card v-if="checkboxGroup.includes('业务日历')" class="no-padding-card" style="margin: 10px 0">
              <!-- 日历 -->
              <el-calendar ref="calendar" class="custom-calendar" v-model="calValue">
                <template #header="{ date }">
                  <b>业务日历</b>
                  <span>{{ date }}</span>
                  <el-button-group>
                    <el-button size="small" @click="selectDate('prev-month')"> 上个月 </el-button>
                    <el-button size="small" @click="selectDate('today')"> 今天 </el-button>
                    <el-button size="small" @click="selectDate('next-month')"> 下个月 </el-button>
                  </el-button-group>
                </template>
                <template #date-cell="{ data }">
                  <el-row class="day-block-wrapper">
                    <el-col :span="6" style="height: 100%">
                      {{ data.day.split('-')[2] }}
                    </el-col>
                    <!-- <el-col :span="18" style="height: 100%">
                      <div v-if="monthlyWorkCalendarDataPopover(data)">
                        <div v-for="item in monthlyWorkCalendarData" :key="item.theDay">
                          <div v-for="(workStatus, index) in item.workStatusListVos" :key="index">
                            <el-tag
                              effect="dark"
                              style="width: 100%; margin: 4px 0 0"
                              :type="computeWorkStatusTagType(workStatus.status)"
                              class="tag-cla"
                              v-if="item.theDay === data.day"
                              :key="workStatus.status">
                              {{ workStatus.statusName }} {{ workStatus.workItemNum }}
                            </el-tag>
                          </div>
                        </div>
                      </div>
                      <el-popover placement="right" :width="30" trigger="hover" v-else>
                        <template #reference>
                          <el-scrollbar height="70px">
                            <div v-for="(item, index) in monthlyWorkCalendarData" :key="item.theDay">
                              <div v-for="(workStatus, index) in item.workStatusListVos">
                                <el-tag
                                  effect="dark"
                                  style="width: 100%; margin: 4px 0 0"
                                  :type="computeWorkStatusTagType(workStatus.status)"
                                  class="tag-cla"
                                  v-if="item.theDay === data.day"
                                  :key="workStatus.status">
                                  {{ workStatus.statusName }} {{ workStatus.workItemNum }}
                                </el-tag>
                              </div>
                            </div>
                          </el-scrollbar>
                        </template>
                        <div v-for="(item, index) in monthlyWorkCalendarData" :key="item.theDay">
                          <div v-for="(workStatus, index) in item.workStatusListVos">
                            <el-tag
                              effect="dark"
                              style="width: 100%; margin: 4px 0 0"
                              :type="computeWorkStatusTagType(workStatus.status)"
                              class="tag-cla"
                              v-if="item.theDay === data.day"
                              :key="workStatus.status">
                              {{ workStatus.statusName }} {{ workStatus.workItemNum }}
                            </el-tag>
                          </div>
                        </div>
                      </el-popover>
                    </el-col> -->
                  </el-row>
                </template>
              </el-calendar>
            </el-card>
          </el-scrollbar>
        </el-card>
      </el-col>
      <!-- 右侧区域 -->
      <el-col :lg="8" :md="24">
        <el-row>
          <el-col>
            <el-card style="margin: 5px 0">
              <el-scrollbar view-style="overflow: hidden;" height="calc(100vh - 210px)">
                <el-descriptions :column="1" border>
                  <template #title>
                    <el-row>
                      <el-col>{{ userInfo.welcomeMessage }}</el-col>
                    </el-row>
                    <el-row>
                      <el-col>{{ userInfo.welcomeContent }}</el-col>
                    </el-row>
                  </template>
                  <template #extra>
                    <el-image :src="userInfo.avatar">
                      <template #error>
                        <div class="image-slot">
                          <el-icon :size="40"><Avatar /></el-icon>
                        </div>
                      </template>
                    </el-image>
                  </template>
                  <el-descriptions-item>
                    <template #label>
                      <div class="cell-item">
                        <el-icon>
                          <user />
                        </el-icon>
                        {{ $t('user.userName') }}
                      </div>
                    </template>
                    {{ userInfo.nickName }}
                  </el-descriptions-item>
                  <el-descriptions-item>
                    <template #label>
                      <div class="cell-item">
                        <el-icon><Calendar /></el-icon>
                        {{ $t('common.currentTime') }}
                      </div>
                    </template>

                    {{ nowTime }}</el-descriptions-item
                  >
                </el-descriptions>
                <!-- 待办事项 -->
                <el-divider content-position="left">
                  <b>待办事项</b>
                </el-divider>
                <div v-if="!toDoSumData.length" style="text-align: center">
                  <el-text type="info">暂无待办项</el-text>
                </div>
                <!-- 系统通知 -->
                <div style="display: flex">
                  <el-divider content-position="left">
                    <b>系统通知</b>
                  </el-divider>
                  <el-divider content-position="right">
                    <el-button type="primary" link icon="bell" @click="viewMoreNotice">前往通知中心</el-button>
                  </el-divider>
                </div>
                <div style="position: absolute; width: 100%">
                  <el-tabs v-model="activeNoticeTab">
                    <el-tab-pane name="notice">
                      <template #label>
                        <el-badge :value="unreadNoticeList.length" style="line-height: 18px" :hidden="unreadNoticeList.length === 0">
                          通知公告
                        </el-badge>
                      </template>
                      <el-empty :image-size="150" description="暂无未读通知公告" v-if="unreadNoticeList.length === 0" />
                      <el-table :data="unreadNoticeList" height="250" :show-header="false" v-else>
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
                      <el-empty :image-size="150" description="暂无未读业务信息" v-if="unreadAimList.length === 0" />
                      <el-table :data="unreadAimList" height="600" :show-header="false" v-else>
                        <el-table-column prop="noticeTitle" label="标题">
                          <template #default="{ row }">
                            <el-icon :size="20" color="#409EFF"><bell /></el-icon>
                            <el-link
                              :type="row.isRead ? 'primary' : 'danger'"
                              style="margin-left: 8px"
                              @click="viewNoticeDetail({ ...row, noticeType: NoticeType.aimMsg })"
                              >{{ row.noticeTitle }}</el-link
                            >
                          </template>
                        </el-table-column>
                        <el-table-column prop="create_time" label="时间" width="180">
                          <template #default="{ row }">
                            {{ formatDate(row.create_time) }}
                          </template>
                        </el-table-column>
                      </el-table>
                    </el-tab-pane>
                  </el-tabs>
                  <el-popconfirm title="是否确定把全部内容消息设为已读？" :teleported="false" :width="300" @confirm="onAllReadClick">
                    <template #reference>
                      <el-button style="position: absolute; top: 0; right: 0">全部标为已读</el-button>
                    </template>
                  </el-popconfirm>
                </div>
              </el-scrollbar>
            </el-card>
          </el-col>
        </el-row>
      </el-col>
    </el-row>
    <noticeDialog v-model="noticeDialogOpen" :notice-model="info" />
  </div>
</template>

<script setup lang="ts">
import noticeDialog from '@/components/Notice/noticeDialog/index.vue'
import signalR from '@/signalr'
import useSocketStore from '@/store/modules/socket'
import useUserStore from '@/store/modules/user'
const router = useRouter()
enum NoticeType {
  notice = 1,
  aimMsg = 2,
  chat = 3
}
const userInfo = computed<any>(() => {
  return useUserStore().userInfo
})
const checkboxGroup = ref(['业务日历'])
const checkBoxArray = ref(
  checkboxGroup.value.map((item) => ({
    label: item
  }))
)
const calendar = ref()
const calValue = ref(new Date())
const selectDate = (val: any) => {
  if (!calendar.value) return
  calendar.value.selectDate(val)
}
const activeNoticeTab = ref('notice')
const unreadNoticeList = computed(() => {
  return useSocketStore().unreadNoticeList
})
const unreadAimList = computed(() => {
  return useSocketStore().unreadAimList
})
const noticeDialogOpen = ref(false)
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
const viewNoticeDetail = (notice: noticeInfo) => {
  noticeDialogOpen.value = true
  info.value = notice
}
const viewMoreNotice = () => {
  router.push({ path: '/notice/center', query: { activeTab: activeNoticeTab.value } })
}
const onAllReadClick = () => {
  if (activeNoticeTab.value === 'notice') {
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
const nowTime = ref<string>('')
// 格式：XXXX年XX月XX日XX时XX分XX秒 星期X
const complement = function (value: any) {
  return value < 10 ? `0${value}` : value
}
const formateDate = (date: any) => {
  const time = new Date(date)
  const year = time.getFullYear()
  const month = complement(time.getMonth() + 1)
  const day = complement(time.getDate())
  const hour = complement(time.getHours())
  const minute = complement(time.getMinutes())
  const second = complement(time.getSeconds())
  const week = '星期' + '日一二三四五六'.charAt(time.getDay())
  return `${year}年${month}月${day}日 ${week} ${hour}:${minute}:${second}`
}
const toDoSumData = ref<any[]>([])
const buildToDoSumData = async () => {}
onMounted(() => {
  setInterval(() => {
    nowTime.value = formateDate(new Date())
  })
  buildToDoSumData()
})
</script>

<style scoped lang="scss">
:deep(.no-padding-card) > .el-card__body {
  padding: 0 0 10px 0;
  .el-calendar__header {
    padding: 18px 20px;
  }
  .el-calendar__body {
    padding: 12px 20px;
  }
}
:deep(.custom-calendar) {
  .el-calendar-day {
    padding: 0;
    height: 90px;
  }
  .day-block-wrapper {
    width: 100%;
    height: 100%;
    padding: 8px;
    box-sizing: border-box;
    border: 2px dashed #00000000;
  }
  .el-calendar-table td.is-selected .day-block-wrapper {
    border: 2px dashed var(--el-color-primary);
  }
}
:deep(.custom-carousel .el-carousel__indicator) {
  .el-carousel__button {
    background-color: #d3dce6;
    width: 25px;
    height: 15px;
  }
  .el-carousel__button .is-active {
    background-color: #99a9bf;
  }
}
.typography-1 {
  border-left: 4px solid var(--el-color-primary);
  padding-left: 5px;
  margin: 10px 0;
  position: relative;
}
.card-label {
  font-weight: bold;
  width: 90px;
  color: var(--el-text-color-regular);
  vertical-align: top;
}
.card-value {
  width: 180px;
  white-space: nowrap; /* 防止文字换行 */
  overflow: hidden; /* 文字溢出隐藏 */
  text-overflow: ellipsis; /* 出现省略号 */
}
.settings {
  display: flex;
  justify-content: flex-end;
}

.tag-name {
  min-width: 30px;
  margin: 0 5px 5px 0;
}
.tag-cla {
  min-width: 50px;
  margin: 0 0 5px 0;
}

.list-group-striped > .list-group-item {
  border-left: 0;
  border-right: 0;
  border-radius: 0;
  padding-left: 0;
  padding-right: 0;
}

.list-group {
  padding-left: 0px;
  list-style: none;
}

.list-group-item {
  border-bottom: 1px solid #e7eaec;
  border-top: 1px solid #e7eaec;
  margin-bottom: -1px;
  padding: 11px 0px;
  font-size: 13px;
}

.vertical-center {
  display: flex;
  justify-content: center;
  align-items: center;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.custom-tabs {
  margin-top: 10px;

  :deep(.el-tabs__item) {
    min-width: 111px;
    justify-content: center;
  }
}
.minibox {
  width: 200px;
  display: flex;
  box-sizing: border-box;
  .icon {
    margin: auto;
    text-align: center;
  }
  .word {
    width: 150px;
    line-height: 26px;
    margin: auto;
  }
}
.card:hover {
  border: 1px solid var(--el-color-primary);
  background-color: #f5f7fa;
  cursor: pointer;
  .icon {
    color: var(--el-color-primary);
  }
  .word {
    color: var(--el-color-primary);
  }
}

.nested-enter-active,
.nested-leave-active {
  transition: all 0.3s ease-in-out;
}
/* delay leave of parent element */
.nested-leave-active {
  transition-delay: 0.25s;
}

.nested-enter-from,
.nested-leave-to {
  transform: translateY(30px);
  opacity: 0;
}

/* we can also transition nested elements using nested selectors */
.nested-enter-active .inner,
.nested-leave-active .inner {
  transition: all 0.3s ease-in-out;
}
/* delay enter of nested element */
.nested-enter-active .inner {
  transition-delay: 0.25s;
}

.nested-enter-from .inner,
.nested-leave-to .inner {
  transform: translateX(30px);
  /*
  	Hack around a Chrome 96 bug in handling nested opacity transitions.
    This is not needed in other browsers or Chrome 99+ where the bug
    has been fixed.
  */
  opacity: 0.001;
}
</style>
