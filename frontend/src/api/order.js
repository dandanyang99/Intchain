/**
 * 订单相关 API
 */

import { get, post, put, del } from '@/utils/request.js'

/**
 * 创建申请订单（销售网点申请彩票）
 */
export const createApplicationOrder = (data) => {
  return post('/api/applicationorders', data)
}

/**
 * 获取申请订单列表
 */
export const getApplicationOrderList = (params) => {
  return get('/api/applicationorders', params)
}

/**
 * 获取申请订单详情
 */
export const getApplicationOrderDetail = (id) => {
  return get(`/api/applicationorders/${id}`)
}

/**
 * 根据订单号获取申请订单
 */
export const getApplicationOrderByNumber = (orderNumber) => {
  return get(`/api/applicationorders/number/${orderNumber}`)
}

/**
 * 根据状态获取申请订单列表
 */
export const getApplicationOrdersByStatus = (status) => {
  return get(`/api/applicationorders/status/${status}`)
}

/**
 * 审批通过申请订单
 */
export const approveApplicationOrder = (id, data) => {
  return post(`/api/applicationorders/${id}/approve`, data)
}

/**
 * 拒绝申请订单
 */
export const rejectApplicationOrder = (id, data) => {
  return post(`/api/applicationorders/${id}/reject`, data)
}

/**
 * 更新订单状态为待发货
 */
export const updateToWaitingShipment = (id) => {
  return post(`/api/applicationorders/${id}/waiting-shipment`)
}

/**
 * 更新订单状态为已发货
 */
export const updateToShipped = (id) => {
  return post(`/api/applicationorders/${id}/shipped`)
}

/**
 * 更新订单状态为运输中
 */
export const updateToInTransit = (id) => {
  return post(`/api/applicationorders/${id}/in-transit`)
}

/**
 * 完成申请订单
 */
export const completeApplicationOrder = (id) => {
  return post(`/api/applicationorders/${id}/complete`)
}

/**
 * 获取印刷订单列表
 */
export const getPrintingOrderList = (params) => {
  return get('/api/printingorders', params)
}

/**
 * 获取印刷订单详情
 */
export const getPrintingOrderDetail = (id) => {
  return get(`/api/printingorders/${id}`)
}

/**
 * 根据状态获取印刷订单列表
 */
export const getPrintingOrdersByStatus = (status) => {
  return get(`/api/printingorders/status/${status}`)
}

/**
 * 接受印刷订单
 */
export const acceptPrintingOrder = (id) => {
  return post(`/api/printingorders/${id}/accept`)
}

/**
 * 更新印刷订单状态为待发货
 */
export const updatePrintingToWaitingShipment = (id) => {
  return post(`/api/printingorders/${id}/waiting-shipment`)
}

/**
 * 更新印刷订单状态为已发货
 */
export const updatePrintingToShipped = (id) => {
  return post(`/api/printingorders/${id}/shipped`)
}

/**
 * 完成印刷订单
 */
export const completePrintingOrder = (id) => {
  return post(`/api/printingorders/${id}/complete`)
}
