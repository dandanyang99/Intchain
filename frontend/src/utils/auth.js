/**
 * 认证相关工具函数
 */

const TOKEN_KEY = 'intchain_token'
const USER_INFO_KEY = 'intchain_user_info'

/**
 * 获取 token
 */
export const getToken = () => {
  return uni.getStorageSync(TOKEN_KEY)
}

/**
 * 设置 token
 */
export const setToken = (token) => {
  return uni.setStorageSync(TOKEN_KEY, token)
}

/**
 * 移除 token
 */
export const removeToken = () => {
  return uni.removeStorageSync(TOKEN_KEY)
}

/**
 * 获取用户信息
 */
export const getUserInfo = () => {
  const userInfo = uni.getStorageSync(USER_INFO_KEY)
  return userInfo ? JSON.parse(userInfo) : null
}

/**
 * 设置用户信息
 */
export const setUserInfo = (userInfo) => {
  return uni.setStorageSync(USER_INFO_KEY, JSON.stringify(userInfo))
}

/**
 * 移除用户信息
 */
export const removeUserInfo = () => {
  return uni.removeStorageSync(USER_INFO_KEY)
}

/**
 * 清除所有认证信息
 */
export const clearAuth = () => {
  removeToken()
  removeUserInfo()
}
