import { get, post, put, del } from '@/utils/request.js'

/**
 * 彩票中心管理 API
 */

// 获取彩票中心列表
export const getLotteryCenterList = (params) => {
  return get('/api/lotterycenters', params)
}

// 获取彩票中心详情
export const getLotteryCenterDetail = (id) => {
  return get(`/api/lotterycenters/${id}`)
}

// 创建彩票中心
export const createLotteryCenter = (data) => {
  return post('/api/lotterycenters', data)
}

// 更新彩票中心
export const updateLotteryCenter = (id, data) => {
  return put(`/api/lotterycenters/${id}`, data)
}

// 删除彩票中心
export const deleteLotteryCenter = (id) => {
  return del(`/api/lotterycenters/${id}`)
}
