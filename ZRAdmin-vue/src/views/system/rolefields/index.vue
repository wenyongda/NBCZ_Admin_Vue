<template>
  <div class="app-container">
    <el-card v-loading="cardLoading">
      <el-page-header @back="goBack">
        <template #content>
          <el-row :gutter="10">
            <el-col :span="1.5">
              <el-button type="default" plain icon="Refresh" @click="handleInit" :loading="initBtnLoading"> {{ $t('btn.init') }} </el-button>
            </el-col>
          </el-row>
        </template>
      </el-page-header>
      <el-divider />
      <el-row :gutter="10">
        <el-col :span="10">
          <el-card>
            <template #header>
              <b>模型</b>
            </template>
            <el-table ref="modelTable" :data="modelList" border highlight-current-row @current-change="handleCurrentChange">
              <el-table-column prop="fullName" label="模型" />
            </el-table>
            <pagination
              v-model:total="total"
              v-model:page="queryParams.pageNum"
              v-model:limit="queryParams.pageSize"
              :layout="'sizes, pager, jumper'"
              @pagination="getList" />
          </el-card>
        </el-col>
        <el-col :span="14">
          <el-card>
            <template #header>
              <div class="flex justify-between items-center">
                <b>数据字段</b>
                <el-button type="primary" plain @click="handleSave" :loading="saveBtnLoading">{{ $t('btn.save') }}</el-button>
              </div>
            </template>
            <el-table v-loading="fieldListLoading" :data="fieldList" height="580" border>
              <el-table-column prop="fieldName" label="字段名称" />
              <el-table-column prop="fieldType" label="字段类型" />
              <el-table-column prop="isPermission" label="是否授权" align="center">
                <template #default="scope">
                  <el-switch v-model="scope.row.isPermission" class="ml-2" style="--el-switch-on-color: #13ce66; --el-switch-off-color: #ff4949" />
                </template>
              </el-table-column>
            </el-table>
          </el-card>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import useTagsViewStore from '@/store/modules/tagsView.js'
import { getModelList, initFields, getFields, addOrUpdateSysRoleField } from '@/api/system/field'
import modal from '@/plugins/modal'
import { ElTable } from 'element-plus'
const router = useRouter()
const route = useRoute()
const cardLoading = ref(false)
const goBack = () => {
  useTagsViewStore().delView(router.currentRoute.value)
  router.push('/system/role')
}
const modelTable = ref<InstanceType<typeof ElTable>>()
const initBtnLoading = ref(false)
const total = ref(0)
const queryParams = ref({
  pageNum: 1,
  pageSize: 10
})
const handleInit = async () => {
  initBtnLoading.value = true
  saveBtnLoading.value = true
  try {
    await initFields()
    modal.msgSuccess('初始化完成')
  } catch (err) {
    console.error(err)
  }
  initBtnLoading.value = false
  saveBtnLoading.value = false
  getList()
}
interface model {
  fullName: string
  // properties: field[]
}
interface field {
  id: number
  fieldName: string
  fieldType: string
  isPermission: boolean
}
const modelList = ref<model[]>([])
const fieldList = ref<field[]>([])
const getList = async () => {
  cardLoading.value = true
  try {
    const { data } = await getModelList(queryParams.value)
    modelList.value = data.result
    total.value = data.totalNum
  } catch (err) {
    console.error(err)
  }
  cardLoading.value = false
}

const currentRow = ref()
const handleCurrentChange = async (val: any) => {
  if (val) {
    currentRow.value = val
    handleLoadFieldList()
  } else {
    fieldList.value = []
  }
}
const handleLoadFieldList = async () => {
  fieldListLoading.value = true
  try {
    const val = currentRow.value
    const { data } = await getFields({ fullName: val.fullName, roleId: route.query.roleId })
    fieldList.value = data
  } catch (e) {
    console.error(e)
  }
  fieldListLoading.value = false
}
const fieldListLoading = ref(false)
const saveBtnLoading = ref(false)
const handleSave = async () => {
  saveBtnLoading.value = true
  try {
    await addOrUpdateSysRoleField(route.query.roleId, fieldList.value)
    modal.msgSuccess('保存成功')
    handleLoadFieldList()
  } catch (e) {
    console.error(e)
  }
  saveBtnLoading.value = false
}
onMounted(() => {
  getList()
})
</script>

<style scoped lang="scss">
.flex {
  display: flex;
}
.justify-between {
  justify-content: space-between;
}
.items-center {
  align-items: center;
}
</style>
