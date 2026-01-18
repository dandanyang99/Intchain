/**
 * 用户相关 API
 */

import { get, post, put, del } from '@/utils/request.js'

/**
 * 用户注册
 */
export const register = (data) => {
  return post('/api/auth/register', data)
}

/**
 * 用户登录
 */
export const login = (data) => {
  return post('/api/auth/login', data)
}

/**
 * 微信登录
 */
export const wxLogin = (code) => {
  return post('/api/auth/wechat-login', { code })
}

/**
 * 绑定微信账号
 */
export const bindWeChat = (code) => {
  return post('/api/auth/bind-wechat', { code })
}

/**
 * 获取微信OpenId（用于注册时绑定）
 */
export const getOpenId = (code) => {
  return post('/api/auth/get-openid', { code })
}

/**
 * 获取用户信息
 */
export const getUserInfo = () => {
  return get('/api/users/info')
}

/**
 * 更新用户信息
 */
export const updateUserInfo = (data) => {
  return put('/api/users/info', data)
}

/**
 * 获取用户列表（管理员）
 */
export const getUserList = (params) => {
  return get('/api/users/list', params)
}

/**
 * 获取用户详情
 */
export const getUserDetail = (id) => {
  return get(`/api/users/${id}`)
}

/**
 * 创建用户
 */
export const createUser = (data) => {
  return post('/api/users', data)
}

/**
 * 更新用户
 */
export const updateUser = (id, data) => {
  return put(`/api/users/${id}`, data)
}

/**
 * 删除用户
 */
export const deleteUser = (id) => {
  return del(`/api/users/${id}`)
}

/**
 * 重置密码
 */
export const resetPassword = (id, data) => {
  return post(`/api/users/${id}/reset-password`, data)
}
