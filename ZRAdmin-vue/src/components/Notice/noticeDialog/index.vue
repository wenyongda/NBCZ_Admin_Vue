<template>
  <div>
    <el-dialog v-model="noticeDialogOpen" draggable :close-on-click-modal="false" append-to-body>
      <template #header>
        {{ noticeModel.noticeTitle }}
      </template>
      <!-- <Editor
        style="overflow-y: auto"
        v-model="valueHtml"
        :defaultConfig="editorConfig"
        mode="default"
        @onCreated="handleCreated"
        @onChange="handleChange" /> -->
      <div v-html="noticeModel.noticeContent"></div>
      <div class="n_right">
        {{ noticeModel.create_name }}
      </div>
      <div class="n_right">{{ formatDate(noticeModel.create_time) }}</div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import '@wangeditor/editor/dist/css/style.css' // 引入 css
import signalR from '@/signalr'
import { Editor } from '@wangeditor/editor-for-vue'
interface props {
  modelValue: boolean
  noticeModel: noticeInfo
}
const props = defineProps<props>()
const emit = defineEmits()
const valueHtml = ref(props.noticeModel.noticeContent)
const editorRef = shallowRef()
const editorConfig = {
  readOnly: true
}
const noticeDialogOpen = ref(props.modelValue)
watch(
  () => noticeDialogOpen.value,
  (val) => {
    emit('update:modelValue', val)
  }
)
watch(
  () => props.modelValue,
  (val) => {
    noticeDialogOpen.value = val
  }
)
onBeforeUnmount(() => {
  const editor = editorRef.value
  if (editor == null) return
  editor.destroy()
})
const handleCreated = (editor) => {
  editorRef.value = editor
}
const handleChange = (editor) => {
  emit('update:noticeModel.noticeContent', editor.getHtml())
}
watch(
  () => props.noticeModel,
  (value) => {
    const editor = editorRef.value
    if (value == undefined) {
      editor.clear()
      return
    }
    valueHtml.value = value.noticeContent
    viewNoticeDetail(value)
  }
)
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
const viewNoticeDetail = (notice: noticeInfo) => {
  if (!notice.isRead) {
    switch (notice.noticeType) {
      case NoticeType.notice:
        signalR.ReadNotice(notice.noticeId)
        break
      case NoticeType.aimMsg:
        signalR.ReadAimMsg(notice.noticeId)
        break
    }
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
.n_right {
  text-align: right;
  margin: 10px;
}
</style>
