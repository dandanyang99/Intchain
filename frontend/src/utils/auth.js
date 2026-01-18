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

/**
 * 解码 JWT token
 * @param {string} token - JWT token
 * @returns {object|null} 解码后的 payload，解码失败返回 null
 */
export const decodeToken = (token) => {
  try {
    if (!token || typeof token !== 'string') {
      return null
    }

    // JWT token 格式: header.payload.signature
    const parts = token.split('.')
    if (parts.length !== 3) {
      return null
    }

    // 解码 payload (base64url)
    const payload = parts[1]
    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )

    return JSON.parse(jsonPayload)
  } catch (error) {
    console.error('Token decode error:', error)
    return null
  }
}

/**
 * 检查 token 是否过期
 * @param {string} token - JWT token
 * @returns {boolean} true: 已过期, false: 未过期
 */
export const isTokenExpired = (token) => {
  const payload = decodeToken(token)
  if (!payload || !payload.exp) {
    return true
  }

  // exp 是秒级时间戳，需要转换为毫秒
  const expirationTime = payload.exp * 1000
  const currentTime = Date.now()

  return currentTime >= expirationTime
}

/**
 * 验证 token 是否有效
 * @param {string} token - JWT token
 * @returns {boolean} true: 有效, false: 无效
 */
export const isTokenValid = (token) => {
  if (!token) {
    return false
  }

  // 检查 token 格式
  const payload = decodeToken(token)
  if (!payload) {
    return false
  }

  // 检查 token 是否过期
  if (isTokenExpired(token)) {
    return false
  }

  return true
}
