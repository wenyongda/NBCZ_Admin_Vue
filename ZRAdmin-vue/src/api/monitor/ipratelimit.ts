import request from '@/utils/request'

const url = '/ip/route/limit/'

export const addIpRateLimitPolicy = (data: any) => {
  return request({
    url: url + 'addIpRateLimitPolicy',
    method: 'POST',
    data
  })
}

export const updateIpRateLimitPolicy = (data: any) => {
  return request({
    url: url + 'updateIpRateLimitPolicy',
    method: 'PUT',
    data
  })
}

export const getIpRateLimitPolicyPage = (params: any) => {
  return request({
    url: url + 'getIpRateLimitPolicyPage',
    method: 'get',
    params
  })
}

export const enableIpRateLimitPolicy = (id: string) => {
  return request({
    url: url + 'enableIpRateLimitPolicy' + '/' + id,
    method: 'patch'
  })
}

export const disableIpRateLimitPolicy = (id: string) => {
  return request({
    url: url + 'disableIpRateLimitPolicy' + '/' + id,
    method: 'patch'
  })
}

export const deleteIpRateLimitPolicy = (id: string) => {
  return request({
    url: url + 'deleteIpRateLimitPolicy' + '/' + id,
    method: 'DELETE'
  })
}

// export const deleteRateLimitRule = (id: string) => {
//   return request({
//     url: url + 'deleteRateLimitRule' + '/' + id,
//     method: 'delete'
//   })
// }

// export const changeRateLimitRuleFlag = (id: string) => {
//   return request({
//     url: url + 'changeRateLimitRuleFlag' + '/' + id,
//     method: 'patch'
//   })
// }

export const getIpRateLimitLogPage = (params: any) => {
  return request({
    url: url + 'getIpRateLimitLogPage',
    method: 'get',
    params
  })
}

export const checkIp = (ip: string) => {
  return request({
    url: url + 'checkIp' + '/' + ip,
    method: 'get'
  })
}
