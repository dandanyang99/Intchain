import { get, post, put } from '@/utils/request.js'

/**
 * 库存管理 API
 */

// 获取库存列表
export const getInventoryList = (params) => {
  return get('/api/inventory', params)
}

// 获取库存详情
export const getInventoryDetail = (id) => {
  return get(`/api/inventory/${id}`)
}

// 更新库存
export const updateInventory = (id, data) => {
  return put(`/api/inventory/${id}`, data)
}

// 根据产品ID获取库存
export const getInventoryByProductId = (productId) => {
  return get(`/api/inventory/product/${productId}`)
}

// 库存入库
export const inventoryIn = (data) => {
  return post('/api/inventory/in', data)
}

// 库存出库
export const inventoryOut = (data) => {
  return post('/api/inventory/out', data)
}
