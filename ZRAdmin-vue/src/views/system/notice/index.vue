<template>
  <div class="app-container">
    <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch" label-width="68px">
      <el-form-item label="公告标题" prop="noticeTitle">
        <el-input v-model="queryParams.noticeTitle" placeholder="请输入公告标题" clearable @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="操作人员" prop="createBy">
        <el-input v-model="queryParams.createBy" placeholder="请输入操作人员" clearable @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="类型" prop="noticeType">
        <el-select v-model="queryParams.noticeType" placeholder="公告类型" clearable>
          <el-option v-for="dict in typeOptions" :key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" icon="Search" @click="handleQuery">{{ $t('btn.search') }}</el-button>
        <el-button icon="Refresh" @click="resetQuery">{{ $t('btn.reset') }}</el-button>
      </el-form-item>
    </el-form>

    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['system:notice:add']"> {{ $t('btn.add') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['system:notice:edit']">
          {{ $t('btn.edit') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['system:notice:remove']">
          {{ $t('btn.delete') }}
        </el-button>
      </el-col>
      <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <el-table v-loading="loading" :data="noticeList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="序号" align="center" prop="noticeId" width="100" />
      <el-table-column label="公告标题" align="center" prop="noticeTitle" :show-overflow-tooltip="true" />
      <el-table-column label="内容" align="center" prop="noticeContent" :show-overflow-tooltip="true" />
      <el-table-column label="公告类型" align="center" prop="noticeType" width="100">
        <template #default="scope">
          <dict-tag :options="typeOptions" :value="scope.row.noticeType" />
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" prop="status" width="100">
        <template #default="scope">
          <dict-tag :options="statusOptions" :value="scope.row.status" />
        </template>
      </el-table-column>
      <el-table-column label="创建者" align="center" prop="createBy" width="100" />
      <el-table-column label="创建时间" align="center" prop="createTime" width="100">
        <template #default="scope">
          <span>{{ parseTime(scope.row.createTime, 'YYYY-MM-DD') }}</span>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button text icon="bell" @click="handleNotice(scope.row)" v-hasPermi="['system:notice:edit']"> 通知</el-button>
          <el-button text icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['system:notice:edit']"> 修改</el-button>
          <el-button text icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['system:notice:remove']"> 删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <pagination :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />

    <!-- 添加或修改公告对话框 -->
    <el-dialog :title="title" v-model="open" width="780px" append-to-body>
      <el-form ref="noticeRef" :model="form" :rules="rules" label-width="80px">
        <el-row>
          <el-col :lg="24">
            <el-form-item label="公告标题" prop="noticeTitle">
              <el-input v-model="form.noticeTitle" placeholder="请输入公告标题" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="公告类型" prop="noticeType">
              <!-- <el-select v-model="form.noticeType" placeholder="请选择公告类型">
                <el-option v-for="dict in typeOptions" :key="dict.dictValue" :label="dict.dictLabel" :value="dict.dictValue"></el-option>
              </el-select> -->
              <el-radio-group v-model="form.noticeType">
                <el-radio v-for="dict in typeOptions" :key="dict.dictValue" :label="parseInt(dict.dictValue)">{{ dict.dictLabel }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item label="状态">
              <el-radio-group v-model="form.status">
                <el-radio v-for="dict in statusOptions" :key="dict.dictValue" :label="parseInt(dict.dictValue)">{{ dict.dictLabel }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item label="内容" prop="noticeContent">
              <editor v-model="form.noticeContent" :min-height="196" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <el-button text @click="cancel">{{ $t('btn.cancel') }}</el-button>
        <el-button type="primary" @click="submitForm" :loading="submitBtnLoading">{{ $t('btn.submit') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="notice">
// 富文本组件
import Editor from '@/components/Editor'
import { listNotice, getNotice, delNotice, addNotice, updateNotice, sendNotice } from '@/api/system/notice'
import { getCurrentInstance } from 'vue'

const { proxy } = getCurrentInstance()
const noticeList = ref([])
const open = ref(false)
const loading = ref(true)
const showSearch = ref(true)
const ids = ref([])
const single = ref(true)
const multiple = ref(true)
const total = ref(0)
const title = ref('')
const noticeRef = ref()
const submitBtnLoading = ref(false)

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    noticeTitle: undefined,
    createBy: undefined,
    status: undefined
  },
  rules: {
    noticeTitle: [{ required: true, message: '公告标题不能为空', trigger: 'blur' }],
    noticeType: [{ required: true, message: '公告类型不能为空', trigger: 'change' }]
  }
})
const statusOptions = ref([])
const typeOptions = ref([])
proxy.getDicts('sys_notice_status').then((response) => {
  statusOptions.value = response.data
})
proxy.getDicts('sys_notice_type').then((response) => {
  typeOptions.value = response.data
})

const { queryParams, form, rules } = toRefs(data)

/** 查询公告列表 */
function getList() {
  loading.value = true
  listNotice(queryParams.value).then((response) => {
    noticeList.value = response.data.result
    total.value = response.data.totalNum
    loading.value = false
  })
}
/** 取消按钮 */
function cancel() {
  open.value = false
  reset()
}
/** 表单重置 */
function reset() {
  form.value = {
    noticeId: undefined,
    noticeTitle: undefined,
    noticeType: 1,
    noticeContent: undefined,
    status: 0
  }
  proxy.resetForm('noticeRef')
}
/** 搜索按钮操作 */
function handleQuery() {
  queryParams.value.pageNum = 1
  getList()
}
/** 重置按钮操作 */
function resetQuery() {
  proxy.resetForm('queryRef')
  handleQuery()
}
/** 多选框选中数据 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.noticeId)
  single.value = selection.length != 1
  multiple.value = !selection.length
}
/** 新增按钮操作 */
function handleAdd() {
  reset()
  submitBtnLoading.value = false
  open.value = true
  title.value = '添加公告'
}
/**修改按钮操作 */
function handleUpdate(row) {
  reset()
  const noticeId = row.noticeId || ids.value
  getNotice(noticeId).then((response) => {
    form.value = response.data
    submitBtnLoading.value = false
    open.value = true
    title.value = '修改公告'
  })
}
/** 提交按钮 */
function submitForm() {
  proxy.$refs['noticeRef'].validate((valid) => {
    if (valid) {
      submitBtnLoading.value = true
      if (form.value.noticeId != undefined) {
        updateNotice(form.value).then((response) => {
          proxy.$modal.msgSuccess('修改成功')
          open.value = false
          getList()
        })
      } else {
        addNotice(form.value).then((response) => {
          proxy.$modal.msgSuccess('新增成功')
          open.value = false
          getList()
        })
      }
    }
  })
}
/** 删除按钮操作 */
function handleDelete(row) {
  const noticeIds = row.noticeId || ids.value
  proxy.$modal
    .confirm('是否确认删除公告编号为"' + noticeIds + '"的数据项？')
    .then(function () {
      return delNotice(noticeIds)
    })
    .then(() => {
      getList()
      proxy.$modal.msgSuccess('删除成功')
    })
    .catch(() => {})
}
// 发送通知
function handleNotice(row) {
  const noticeId = row.noticeId || ids.value
  sendNotice(noticeId).then((res) => {
    proxy.$modal.msgSuccess('发送通知成功')
  })
}
getList()
</script>
