<template>
  <div class="app-container">
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="plus" @click="handleAdd(ruleFormRef)">
          {{ $t('btn.add') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="edit" :disabled="single" @click="handleEdit(ruleFormRef)">
          {{ $t('btn.edit') }}
        </el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="delete" :disabled="multiple" @click="handleDelete()">
          {{ $t('btn.delete') }}
        </el-button>
      </el-col>
      <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
    </el-row>

    <el-table v-loading="loading" :data="codeRoleList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="代码" align="center" prop="code" width="150" :show-overflow-tooltip="true" />
      <el-table-column label="名称" align="center" prop="name" width="200" :show-overflow-tooltip="true" />
      <el-table-column label="类型" align="center" prop="type" width="180" :show-overflow-tooltip="true" />
      <el-table-column label="前缀" align="center" prop="prefix" :show-overflow-tooltip="true" />
      <el-table-column label="宽度" align="center" prop="width" :show-overflow-tooltip="true" />
      <el-table-column label="初始值" align="center" prop="initVal" :show-overflow-tooltip="true" />
      <el-table-column label="增量" align="center" prop="step" width="180" :show-overflow-tooltip="true" />
      <el-table-column label="终止值" align="center" prop="finishVal" :show-overflow-tooltip="true" />
      <el-table-column label="循环" align="center" prop="cycle" :show-overflow-tooltip="true" />
      <el-table-column label="后缀" align="center" prop="sufix" :show-overflow-tooltip="true" />
      <el-table-column label="分隔符" align="center" prop="joinChar" :show-overflow-tooltip="true" />
      <el-table-column label="填充符" align="center" prop="fillChar" :show-overflow-tooltip="true" />
      <el-table-column label="类型值" align="center" prop="typeVal" :show-overflow-tooltip="true" />
      <el-table-column label="循环号" align="center" prop="cycleVal" :show-overflow-tooltip="true" />
      <el-table-column label="当前值" align="center" prop="currVal" width="180" :show-overflow-tooltip="true" />
      <el-table-column label="版本号" align="center" prop="version" width="180" :show-overflow-tooltip="true" />
    </el-table>
    <pagination v-model:total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />
    <!-- 新增/编辑 -->
    <el-dialog v-model="dialogVisible" v-if="dialogVisible" :title="dialogTitle" draggable>
      <el-form ref="ruleFormRef" :model="codeRuleForm" :rules="rules" status-icon label-width="90px">
        <el-row :gutter="10">
          <el-col :span="12">
            <el-form-item label="代码" prop="code">
              <el-input v-model="codeRuleForm.code" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="名称" prop="name">
              <el-input v-model="codeRuleForm.name" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="类型" prop="type">
              <el-input v-model="codeRuleForm.type" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="前缀" prop="prefix">
              <el-input v-model="codeRuleForm.prefix" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="宽度" prop="width">
              <el-input v-model="codeRuleForm.width" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="初始值" prop="initVal">
              <el-input v-model="codeRuleForm.initVal" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="增量" prop="step">
              <el-input v-model="codeRuleForm.step" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="终止值" prop="finishVal">
              <el-input v-model="codeRuleForm.finishVal" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="循环" prop="cycle">
              <el-input v-model="codeRuleForm.cycle" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="后缀" prop="sufix">
              <el-input v-model="codeRuleForm.sufix" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="分隔符" prop="joinChar">
              <el-input v-model="codeRuleForm.joinChar" placeholder="请输入" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="填充符" prop="fillChar">
              <el-input v-model="codeRuleForm.fillChar" placeholder="请输入" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <template #footer>
        <span>
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitForm(ruleFormRef)">确认</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { addBaseCodeRule, deleteBaseCodeRule, updateBaseCodeRule, getBaseCodeRuleList } from '@/api/system/baseCodeRole'
import type { FormInstance, FormRules } from 'element-plus'
import useCurrentInstance from '@/utils/useCurrentInstance'
const { globalProperties } = useCurrentInstance()
const dialogVisible = ref(false)
const dialogTitle = ref('')
const ruleFormRef = ref<FormInstance>()
const multipleSelection = ref([])
let codeRuleForm: any = reactive({
  code: null,
  name: null,
  type: null,
  prefix: null,
  width: null,
  initVal: null,
  step: null,
  finishVal: null,
  cycle: null,
  sufix: null,
  joinChar: null,
  fillChar: null
})
let rules = reactive<FormRules>({
  code: [{ required: true, message: '不可为空', trigger: 'blur' }],
  name: [{ required: true, message: '不可为空', trigger: 'blur' }]
})
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
// 业务维护编码表格数据
let codeRoleList: any[] = reactive([])
const queryParams = reactive({
  pageNum: 1,
  pageSize: 10
})
// 总条数
const total = ref(0)
const getList = () => {
  loading.value = true
  getBaseCodeRuleList(queryParams).then((res) => {
    codeRoleList = res.data.result
    total.value = res.data.totalNum
    loading.value = false
  })
}
const handleSelectionChange = (selection) => {
  ids.value = selection.map((item) => item.code)
  single.value = ids.value.length !== 1
  multiple.value = !ids.value.length
  multipleSelection.value = selection
}
const resetCodeRuleForm = (isUpdate?: boolean, row?) => {
  if (isUpdate) {
    codeRuleForm = reactive({
      code: row.code,
      name: row.name,
      type: row.type,
      prefix: row.prefix,
      width: row.width,
      initVal: row.initVal,
      step: row.step,
      finishVal: row.finishVal,
      cycle: row.cycle,
      sufix: row.sufix,
      joinChar: row.joinChar,
      fillChar: row.fillChar
    })
  } else {
    codeRuleForm = reactive({
      code: null,
      name: null,
      type: null,
      prefix: null,
      width: null,
      initVal: null,
      step: null,
      finishVal: null,
      cycle: null,
      sufix: null,
      joinChar: null,
      fillChar: null
    })
  }
}
const handleAdd = (formEl: FormInstance | undefined) => {
  dialogTitle.value = '新增规则'
  dialogVisible.value = true
  resetCodeRuleForm(false)
  formEl && formEl.clearValidate()
}
const handleEdit = (formEl: FormInstance | undefined) => {
  dialogTitle.value = '编辑规则'
  dialogVisible.value = true
  resetCodeRuleForm(true, multipleSelection.value[0])
  formEl && formEl.clearValidate()
}
/** 删除按钮操作 */
const handleDelete = async () => {
  const Ids = ids.value
  globalProperties
    .$confirm('是否确认删除参数编号为"' + Ids + '"的数据项？')
    .then(async function () {
      return await deleteBaseCodeRule(Ids)
    })
    .then(() => {
      getList()
      globalProperties.$modal.msgSuccess('删除成功')
    })
    .catch(() => {})
}
const submitForm = (formEl: FormInstance | undefined) => {
  if (!formEl) return
  formEl.validate((valid) => {
    if (valid) {
      // 提交
      if (dialogTitle.value === '新增规则') {
        addBaseCodeRule(codeRuleForm).then((res) => {
          getList()
          dialogVisible.value = false
        })
      } else {
        updateBaseCodeRule(codeRuleForm).then((res) => {
          getList()
          dialogVisible.value = false
        })
      }
    }
  })
}
onMounted(() => {
  getList()
})
</script>

<style scoped></style>
