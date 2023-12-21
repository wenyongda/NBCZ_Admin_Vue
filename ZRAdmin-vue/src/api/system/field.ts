import request from '@/utils/request'

const url = '/system/field/'
export const getModelList = (params: { pageNum: number; pageSize: number }) => {
  return request({
    url: url + 'getModelList',
    method: 'get',
    params
  })
}

export const getFields = (params: { fullName: string; roleId: any }) => {
  return request({
    url: url + 'getFields',
    method: 'get',
    params
  })
}

export const initFields = () => {
  return request({
    url: url + 'initFields',
    method: 'post'
  })
}

export const addOrUpdateSysRoleField = (roleId: any, data: any) => {
  return request({
    url: url + 'addOrUpdateSysRoleField' + '/' + roleId,
    method: 'put',
    data
  })
}

export const fieldDisplay = (params: { queryKey: string }) => {
  return request({
    url: url + 'fieldDisplay',
    method: 'post',
    params
  })
}
