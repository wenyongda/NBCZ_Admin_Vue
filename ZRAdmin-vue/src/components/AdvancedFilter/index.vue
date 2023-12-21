<template>
  <div>
    <el-form inline>
      <el-form-item class="search-form-item" :label="currFilter.label" v-for="(currFilter, index) in currFilters" :key="index">
        <template #label>
          <div class="search-form-item-label-wrapper">
            <span class="search-form-item-label">{{ currFilter.label }}</span>
            <el-icon class="search-form-item-remove-btn" @click="remove(index)">
              <CircleClose />
            </el-icon>
          </div>
        </template>
        <!-- 单选 -->
        <el-select v-if="currFilter.type === 'singleSelect'" v-model="currFilter.value" placeholder="请选择" filterable clearable>
          <el-option v-for="item in currFilter.options" :key="item.value" :label="item.label" :value="item.value">
            <span class="text-left">
              {{ item.label }}
            </span>
            <span class="text-right">
              {{ item.name }}
            </span>
          </el-option>
        </el-select>
        <!-- 多选 -->
        <el-select
          v-else-if="currFilter.type === 'multipleSelect'"
          v-model="currFilter.value"
          placeholder="请选择"
          multiple
          collapse-tags
          filterable
          clearable>
          <el-option v-for="item in currFilter.options" :key="item.value" :label="item.label" :value="item.value">
            <span class="text-left">
              {{ item.label }}
            </span>
            <span class="text-right">
              {{ item.name }}
            </span>
          </el-option>
        </el-select>
        <!-- 日期 -->
        <el-date-picker
          v-else-if="currFilter.type === ('date' || 'daterange')"
          :type="currFilter.type"
          v-model="currFilter.value"
          start-placeholder="开始日期"
          end-placeholder="结束日期" />
        <!-- 数字 -->
        <el-input-number v-else-if="currFilter.type === 'number'" v-model="currFilter.value" />
        <!-- 文本 -->
        <el-input v-else v-model="currFilter.value" placeholder="请输入" clearable />
      </el-form-item>
      <el-form-item class="add-condition-wrapper">
        <template v-if="currFilters.length !== searchableItems.length">
          <div v-if="showHeadSelect">
            <el-select
              ref="tableHeaderSelect"
              v-model="conditionValue"
              placeholder="请选择筛选条件"
              @change="selectFilterItem"
              @visible-change="(val) => (showHeadSelect = val)">
              <el-option
                v-for="item in searchableItems"
                :key="item.key"
                :label="item.label"
                :value="item.key"
                :disabled="!!currFilters.find((c) => c.key === item.key)" />
            </el-select>
          </div>
          <template v-else>
            <div @click="handleAddCondition" class="dashed-wrapper">
              <el-icon>
                <Plus />
              </el-icon>
              添加筛选条件
            </div>
          </template>
        </template>
        <el-button plain type="success" style="margin-left: 15px" @click="handleSearch()">搜索</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>
<script setup name="AdvancedFilter" lang="ts">
import type { PropType } from 'vue'
interface searchableItemOption {
  label: string
  value: string
  name?: string
}
export interface searchableItem {
  // 筛选类型
  type: 'string' | 'number' | 'date' | 'singleSelect' | 'multipleSelect' | 'daterange'
  // 可筛选项名称
  label: string
  // 可筛选项属性键
  key: string | Array<string>
  // 可筛选项属性值
  value?: any
  // 此项的可选项集合
  options?: searchableItemOption[]
}
const props = defineProps({
  searchableItems: {
    type: Array as PropType<searchableItem[]>,
    default: () => [],
    require: true
  }
})
const emit = defineEmits()
const tableHeaderSelect = ref()
const showHeadSelect = ref(false)
const currFilters = ref<searchableItem[]>([])
const conditionValue = ref()
// 添加条件
function handleAddCondition() {
  showHeadSelect.value = true
  nextTick(() => {
    tableHeaderSelect.value.toggleMenu()
  })
}
// 选中过滤项
function selectFilterItem(val) {
  const item: searchableItem | undefined = props.searchableItems.find((v) => v.key === val)
  if (item) {
    item.value = null // 筛选项值重置
    currFilters.value.push(item)
  }
  conditionValue.value = null // 条件筛选框可选项值重置
  showHeadSelect.value = false
}
// 移除
function remove(index) {
  currFilters.value.splice(index, 1)
}
// 搜索
function handleSearch() {
  let params = {}
  currFilters.value.map((v) => {
    if (Array.isArray(v.key)) {
      // 起止区间格式参数
      params[v.key[0]] = v.value[0]
      params[v.key[1]] = v.value[1]
    } else {
      params[v.key] = v.value
    }
  })
  emit('query', params)
}
function initFilterValue() {
  currFilters.value = []
  props.searchableItems.map((v) => {
    if (v.value) {
      currFilters.value.push(v)
    }
  })
}
onMounted(() => {
  initFilterValue()
})
defineExpose({
  handleSearch
})
</script>
<style lang="scss" scoped>
.dashed-wrapper {
  border: 1px dashed #ccc;
  border-radius: 4px;
  display: inline-block;
  color: var(--el-text-color-placeholder);
  width: 220px;
  height: 33px;
  text-align: center;
  cursor: pointer;
  &:hover {
    background-color: rgba(222, 222, 222, 0.2);
  }
}
.search-form-item-label-wrapper {
  position: relative;
  cursor: pointer;
  .search-form-item-label {
    margin-right: 10px;
  }
  .search-form-item-remove-btn {
    position: absolute;
    top: 5px;
    right: 0;
    margin-right: -5px;
    margin-top: -5px;
    font-size: 16px;
    color: var(--el-text-color-placeholder);
    display: none;
    &:hover {
      color: var(--el-color-danger);
    }
    &:active {
      transform: scale(0.8);
    }
  }
}
.search-form-item {
  &:hover .search-form-item-remove-btn {
    display: inline-block;
  }
}
.add-condition-wrapper {
  min-width: 340px;
}
</style>
