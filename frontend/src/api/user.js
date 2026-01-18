/**
 * 用户相关 API
 */

import { get, post, put } from '@/utils/request.js'

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
  return post('/api/users/wx-login', { code })
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
