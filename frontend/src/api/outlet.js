import { get, post, put, del } from '@/utils/request.js'

/**
 * 销售网点管理 API
 */

// 获取销售网点列表
export const getOutletList = (params) => {
  return get('/api/salesoutlets', params)
}

// 获取销售网点详情
export const getOutletDetail = (id) => {
  return get(`/api/salesoutlets/${id}`)
}

// 创建销售网点
export const createOutlet = (data) => {
  return post('/api/salesoutlets', data)
}

// 更新销售网点
export const updateOutlet = (id, data) => {
  return put(`/api/salesoutlets/${id}`, data)
}

// 删除销售网点
export const deleteOutlet = (id) => {
  return del(`/api/salesoutlets/${id}`)
}

// 根据彩票中心ID获取销售网点列表
export const getOutletsByLotteryCenterId = (lotteryCenterId) => {
  return get(`/api/salesoutlets/lottery-center/${lotteryCenterId}`)
}
