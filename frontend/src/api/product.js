/**
 * 产品相关 API
 */

import { get, post, put, del } from '@/utils/request.js'

/**
 * 获取产品列表
 */
export const getProductList = (params) => {
  return get('/api/products', params)
}

/**
 * 获取产品详情
 */
export const getProductDetail = (id) => {
  return get(`/api/products/${id}`)
}

/**
 * 创建产品（彩票中心发布产品）
 */
export const createProduct = (data) => {
  return post('/api/products', data)
}

/**
 * 更新产品信息
 */
export const updateProduct = (id, data) => {
  return put(`/api/products/${id}`, data)
}

/**
 * 删除产品
 */
export const deleteProduct = (id) => {
  return del(`/api/products/${id}`)
}

/**
 * 获取产品库存统计
 */
export const getInventoryStats = (productId) => {
  return get(`/api/inventory/stats/${productId}`)
}
