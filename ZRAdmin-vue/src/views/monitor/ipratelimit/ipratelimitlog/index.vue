<template>
  <div class="app-container">
    <el-card v-loading="cardLoading">
      <el-page-header @back="goBack">
        <template #content></template>
      </el-page-header>
      <el-divider />
      <el-table v-loading="loading" :data="ipRateLimitLogList" height="600" border>
        <el-table-column label="请求方式" prop="httpVerb" width="90" show-overflow-tooltip />
        <el-table-column label="路径" prop="path" width="250" show-overflow-tooltip />
        <el-table-column label="客户端IP" prop="clientIp" show-overflow-tooltip />
        <el-table-column label="限制" prop="limit" show-overflow-tooltip />
        <el-table-column label="期间" prop="period" show-overflow-tooltip />
        <el-table-column label="超出次数" prop="exceeded" show-overflow-tooltip />
        <el-table-column label="终结点" prop="endpoint" show-overflow-tooltip />
        <el-table-column label="时间" prop="createTime" show-overflow-tooltip />
      </el-table>
      <pagination v-model:total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import useTagsViewStore from '@/store/modules/tagsView'
import Pagination from '@/components/Pagination/index.vue'
import { getIpRateLimitLogPage } from '@/api/monitor/ipratelimit'
onMounted(() => {
  getList()
})
const route = useRoute()
const router = useRouter()
const cardLoading = ref(false)
const goBack = () => {
  useTagsViewStore().delView(router.currentRoute.value)
  router.push('/monitor/ipratelimit')
}
// 遮罩层
const loading = ref(true)
// 总条数
const total = ref(0)
const queryParams = ref({
  pageNum: 1,
  pageSize: 10,
  clientIp: route.query.ip
})
interface ipRateLimitLog {
  id: string
  httpVerb: string
  path: string
  clientIp: string
  limit: number
  period: string
  exceeded: number
  endpoint: string
  createTime: string
}
const ipRateLimitLogList = ref<ipRateLimitLog[]>([])
const getList = async () => {
  loading.value = true
  try {
    const { data } = await getIpRateLimitLogPage(queryParams.value)
    ipRateLimitLogList.value = data.result
    total.value = data.totalNum
    loading.value = false
  } catch (err) {
    console.error(err)
    loading.value = false
  }
}
</script>

<style scoped lang="scss"></style>
