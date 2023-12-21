import request from '@/utils/request'

export const addBaseCodeRule = (data: any) => {
  return request({
    url: 'base/codeRule/addBaseCodeRule',
    method: 'post',
    data
  })
}

export const deleteBaseCodeRule = (code: any) => {
  return request({
    url: 'base/codeRule/deleteBaseCodeRule' + '/' + code,
    method: 'delete'
  })
}

export const getBaseCodeRuleList = (params: any) => {
  return request({
    url: 'base/codeRule/getBaseCodeRuleList',
    method: 'get',
    params
  })
}

export const updateBaseCodeRule = (data: any) => {
  return request({
    url: 'base/codeRule/updateBaseCodeRule',
    method: 'put',
    data
  })
}
