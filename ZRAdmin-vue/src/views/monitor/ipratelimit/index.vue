<template>
  <div class="app-container">
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="plus" @click="handleAdd()">
          {{ $t('btn.add') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="edit" :disabled="single" @click="handleEdit()">
          {{ $t('btn.edit') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="delete" :disabled="single" @click="handleDelete()">
          {{ $t('btn.delete') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="warning" plain icon="Checked" @click="handleGoToLog()"> 日志 </el-button>
      </el-col>
      <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :search-btn="false"></right-toolbar>
    </el-row>

    <el-table v-loading="loading" :data="ipRateLimitPolicyList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="IP" align="center" prop="ip" width="150" :show-overflow-tooltip="true" />
      <el-table-column prop="flag" label="IP限制状态" align="center">
        <template #default="{ row }">
          <div @click="switchChange(row)" style="cursor: pointer">
            <el-switch
              v-model="row.flag"
              :loading="row.loading"
              active-value="1"
              inactive-value="0"
              active-text="启用"
              inactive-text="停用"
              size="small"
              style="pointer-events: none" />
          </div>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center">
        <template #default="{ row }">
          <el-button type="primary" text icon="View" @click="handleView(row)">查看</el-button>
          <el-button type="success" text icon="edit" @click="handleEdit(row)">编辑</el-button>
          <el-button type="danger" text icon="delete" @click="handleDelete(row)">删除</el-button>
          <el-button type="warning" text icon="Checked" @click="handleGoToLog(row.ip)">日志</el-button>
        </template>
      </el-table-column>
    </el-table>
    <pagination v-model:total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />

    <el-dialog v-model="dialogVisible" :title="dialogTitle" draggable width="800" @close="getList">
      <el-form ref="ipRateLimitPolicyRef" :model="ipRateLimitPolicyModel" :rules="ipRateLimitPolicyRules" label-position="left">
        <el-form-item label="IP" prop="ip">
          <el-input v-model="ipRateLimitPolicyModel.ip" placeholder="请输入" :disabled="!isEdit" />
        </el-form-item>
        <el-form-item prop="rules">
          <el-row :gutter="10" class="mb10">
            <el-col :span="1.5">
              <el-button plain icon="Plus" type="primary" @click="handleAddRule()" v-if="isEdit">新增规则</el-button>
            </el-col>
            <el-col :span="1.5">
              IP限制状态:
              <span>
                <el-tag type="success" v-if="ipRateLimitPolicyModel.flag === '1'">启用</el-tag>
                <el-tag type="danger" v-else>禁用</el-tag>
              </span>
            </el-col>
          </el-row>
          <el-table :data="ipRateLimitPolicyModel.rules" border height="300">
            <el-table-column label="终结点" prop="endpoint">
              <template #default="{ row, $index }">
                <span v-show="!row.show">{{ row.endpoint }}</span>
                <el-form-item :prop="`rules.${$index}.endpoint`" :rules="rules.endpoint">
                  <el-input v-model="row.endpoint" v-show="row.show"></el-input>
                </el-form-item>
              </template>
            </el-table-column>
            <el-table-column label="期间" prop="period">
              <template #default="{ row, $index }">
                <span v-show="!row.show">{{ row.period }}</span>
                <el-form-item :prop="`rules.${$index}.periodInputPart`" :rules="rules.periodInputPart">
                  <el-input v-model="row.periodInputPart" v-show="row.show">
                    <template #append>
                      <el-select v-model="row.periodSelectPart">
                        <el-option label="s" value="s" />
                        <el-option label="m" value="m" />
                        <el-option label="h" value="h" />
                        <el-option label="d" value="d" />
                      </el-select>
                    </template>
                  </el-input>
                </el-form-item>
              </template>
            </el-table-column>
            <el-table-column label="限制" prop="limit">
              <template #default="{ row, $index }">
                <span v-show="!row.show">{{ row.limit }}</span>
                <el-form-item :prop="`rules.${$index}.limit`" :rules="rules.limit">
                  <el-input-number v-model="row.limit" :min="0" :controls="false" v-show="row.show" style="width: 100%"></el-input-number>
                </el-form-item>
              </template>
            </el-table-column>
            <el-table-column label="规则限制状态" align="center">
              <template #default="{ row }">
                <!-- :style="row.id ? 'cursor: pointer' : 'pointer-events: none'" -->
                <div @click="switchChangeRule(row)" style="cursor: pointer" v-if="isEdit">
                  <!-- :disabled="!row.id" -->
                  <el-switch
                    v-model="row.flag"
                    :loading="row.loading"
                    active-value="1"
                    inactive-value="0"
                    active-text="启用"
                    inactive-text="停用"
                    size="small"
                    style="pointer-events: none" />
                </div>
                <span v-else>
                  <el-tag type="success" v-if="row.flag === '1'">启用</el-tag>
                  <el-tag type="danger" v-else>禁用</el-tag>
                </span>
              </template>
            </el-table-column>
            <el-table-column label="操作" align="center" v-if="isEdit">
              <template #default="scope">
                <el-button text icon="Edit" type="success" @click="handleEditRule(scope.row)" title="编辑规则" v-if="!scope.row.show"></el-button>
                <el-button
                  text
                  icon="RefreshLeft"
                  type="warning"
                  v-if="scope.row.id && scope.row.show"
                  @click="scope.row.show = false"
                  title="取消编辑"></el-button>
                <el-button text icon="Delete" type="danger" @click="handleDeleteRuleLine(scope.$index)" title="删除规则"></el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-form-item>
      </el-form>
      <template #footer v-if="isEdit">
        <el-row style="margin-top: -40px">
          <el-col :span="18" style="border: 1px solid var(--el-color-primary); border-radius: 2px; padding: 10px">
            <el-text class="text-left">
              <div><b>终结点：</b>{HTTP_Verb}:{PATH}，您可以使用asterix符号来定位任何HTTP谓词</div>
              <div>例如：*、*:/api/values、*:/api/values、((post)|(put)):/api/values</div>
              <div><b>期间：</b>{INT}{PERIOD_TYPE}，您可以使用以下期间类型之一：s(秒), m(分), h(时), d(天)</div>
              <div><b>限制：</b>{LONG}，单位时间内的允许访问的次数</div>
            </el-text>
          </el-col>
          <el-col :span="6" style="bottom: 2px" class="absolute right-1">
            <el-button @click="dialogVisible = false">{{ $t('btn.cancel') }}</el-button>
            <el-button type="primary" @click="handleConfirm">{{ $t('btn.submit') }}</el-button>
          </el-col>
        </el-row>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import Pagination from '@/components/Pagination/index.vue'
import {
  addIpRateLimitPolicy,
  checkIp,
  // changeRateLimitRuleFlag,
  deleteIpRateLimitPolicy,
  // deleteRateLimitRule,
  disableIpRateLimitPolicy,
  enableIpRateLimitPolicy,
  getIpRateLimitPolicyPage,
  updateIpRateLimitPolicy
} from '@/api/monitor/ipratelimit'
import modal from '@/plugins/modal'
import { FormInstance, FormRules } from 'element-plus'
import { deepClone } from '@/utils'
const router = useRouter()
onMounted(() => {
  getList()
})
const ipRateLimitPolicyRef = ref<FormInstance>()
const handleAdd = () => {
  dialogTitle.value = '新增限制规则'
  dialogVisible.value = true
  reset(false)
  isEdit.value = true
  ipRateLimitPolicyRef.value?.clearValidate()
}
const isEdit = ref(true)
const handleView = (row: any) => {
  reset(true, row)
  isEdit.value = false
  dialogTitle.value = '查看限制规则'
  dialogVisible.value = true
  ipRateLimitPolicyRef.value?.clearValidate()
}
const handleEdit = (row?: any) => {
  if (row) {
    // if (row.flag === '1') {
    //   modal.msgWarning('请先停用，才能修改')
    //   return
    // }
    reset(true, row)
  } else {
    // if (multipleSelection.value[0].flag === '1') {
    //   modal.msgWarning('请先停用，才能修改')
    //   return
    // }
    reset(true, multipleSelection.value[0])
  }
  dialogTitle.value = '编辑限制规则'
  dialogVisible.value = true
  isEdit.value = true
  ipRateLimitPolicyRef.value?.clearValidate()
}

const reset = (isUpdate: boolean, row?: any) => {
  if (isUpdate) {
    ipRateLimitPolicyModel.value = deepClone(row)
    ipRateLimitPolicyModel.value.rules = ipRateLimitPolicyModel.value.rules?.map((item: any) => ({
      ...item,
      periodInputPart: item.period.slice(0, -1),
      periodSelectPart: item.period.slice(-1)
    }))
    originIp.value = row.ip
  } else {
    ipRateLimitPolicyModel.value = {
      ip: '',
      flag: '0',
      rules: []
    }
    originIp.value = ''
  }
}
const handleDelete = async (row?: any) => {
  try {
    await modal.confirm('是否要删除?')
    try {
      if (row) {
        await deleteIpRateLimitPolicy(row.id)
      } else {
        await deleteIpRateLimitPolicy(ids.value[0])
      }
      await getList()
      modal.msgSuccess('删除成功')
    } catch (err) {
      console.error(err)
      modal.msgError('删除失败')
    }
  } catch (err) {}
}
// 遮罩层
const loading = ref(true)
// 选中数组
const ids = ref([])
// 非单个禁用
const single = ref(true)
// 非多个禁用
const multiple = ref(true)
// 显示搜索条件
const showSearch = ref(true)
const multipleSelection = ref<any[]>([])
// 总条数
const total = ref(0)
const queryParams = ref({
  pageNum: 1,
  pageSize: 10
})
interface ipRateLimitPolicy {
  id?: string
  ip: string
  flag: string
  rules?: Array<rateLimitRule>
}
interface rateLimitRule {
  id?: string
  ipRateLimitPolicyId: string
  endpoint: string
  period: string
  limit: number
  flag: string
  show?: boolean
  periodInputPart?: number
  periodSelectPart?: string
}
const ipRateLimitPolicyList = ref<ipRateLimitPolicy[]>([])
const getList = async () => {
  loading.value = true
  try {
    const { data } = await getIpRateLimitPolicyPage(queryParams.value)
    ipRateLimitPolicyList.value = data.result
    total.value = data.totalNum
    loading.value = false
  } catch (err) {
    console.error(err)
    loading.value = false
  }
}
const handleSelectionChange = (selection: any) => {
  ids.value = selection.map((item: any) => item.id)
  single.value = ids.value.length !== 1
  multiple.value = !ids.value.length
  multipleSelection.value = selection
}
const switchChange = async (row: any) => {
  row.loading = true
  // 停用
  if (row.flag === '1') {
    try {
      const { data } = await disableIpRateLimitPolicy(row.id)
      row.loading = false
      modal.msgSuccess('停用成功')
      row.flag = data
    } catch (err) {
      console.error(err)
    }
  } else {
    try {
      const { data } = await enableIpRateLimitPolicy(row.id)
      row.loading = false
      modal.msgSuccess('启用成功')
      row.flag = data
    } catch (err) {
      console.error(err)
    }
  }
}

const dialogVisible = ref(false)
const dialogTitle = ref('')
const originIp = ref('')
const ipRateLimitPolicyModel = ref<ipRateLimitPolicy>({
  ip: '',
  flag: '0'
})
const ipRateLimitPolicyRules = ref<FormRules>({
  ip: [
    {
      trigger: 'blur',
      validator: (rule: any, value: any, callback: any) => {
        const reg =
          /^(?:(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)|(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}|(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(?:\.(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)){3}(?:\/(?:3[0-2]|[1-2][0-9]|[0-9]))?)$/
        if (!reg.test(value)) {
          callback(new Error('请输入正确的IP'))
          return
        }
        if (originIp.value !== value) {
          setTimeout(async () => {
            const { data } = await checkIp(value)
            if (data) {
              callback(new Error('该IP已存在'))
            } else {
              callback()
            }
          }, 400)
        } else {
          callback()
        }
      }
    },
    {
      required: true,
      message: '请输入IP',
      trigger: 'blur'
    }
  ]
})
const rules = ref<FormRules>({
  endpoint: [
    {
      required: true,
      trigger: 'blur'
    }
  ],
  periodInputPart: [
    {
      required: true,
      trigger: 'blur'
    },
    {
      pattern: /^(([1-9]{1}\d*)|(0{1}))(\.\d{1,2})?$/
    }
  ],
  limit: [{ required: true, trigger: 'blur' }]
})
const handleConfirm = async () => {
  await ipRateLimitPolicyRef.value?.validate(async (valid) => {
    if (valid) {
      ipRateLimitPolicyModel.value.rules = ipRateLimitPolicyModel.value.rules?.map((item: any) => {
        item.period = item.periodInputPart + item.periodSelectPart
        return {
          endpoint: item.endpoint,
          flag: item.flag,
          id: item.id,
          ipRateLimitPolicyId: item.ipRateLimitPolicyId,
          limit: item.limit,
          period: item.period
        }
      })
      if (dialogTitle.value === '编辑限制规则') {
        try {
          const { data } = await updateIpRateLimitPolicy(ipRateLimitPolicyModel.value)
          if (data) {
            modal.msgSuccess('编辑成功')
            dialogVisible.value = false
          }
        } catch (err) {
          console.error(err)
          modal.msgError('编辑失败')
        }
      } else {
        try {
          const { data } = await addIpRateLimitPolicy(ipRateLimitPolicyModel.value)
          if (data) {
            modal.msgSuccess('新增成功')
            dialogVisible.value = false
          }
        } catch (err) {
          console.error(err)
          modal.msgError('新增失败')
        }
      }
    } else {
      modal.msgWarning('其中有数据未填写或格式不符合规范，请检查')
    }
  })
}

const switchChangeRule = async (row: any) => {
  row.loading = true
  try {
    row.loading = false
    row.flag = row.flag === '1' ? '0' : '1'
  } catch (err) {
    console.error(err)
  }
}

const handleAddRule = () => {
  const newRule: rateLimitRule = {
    ipRateLimitPolicyId: ipRateLimitPolicyModel.value.id!,
    endpoint: '',
    period: '',
    limit: 0,
    flag: '0',
    show: true,
    periodInputPart: 0,
    periodSelectPart: 's'
  }
  ipRateLimitPolicyModel.value.rules?.push(newRule)
}

const handleEditRule = (row: any) => {
  row.show = true
}

const handleDeleteRuleLine = (index: any) => {
  ipRateLimitPolicyModel.value.rules?.splice(index, 1)
}

const handleGoToLog = (ip?: string) => {
  router.push({ path: '/monitor/ipratelimitlog', query: { ip } })
}
</script>

<style scoped lang="scss"></style>
