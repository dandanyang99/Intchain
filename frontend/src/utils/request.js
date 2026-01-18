/**
 * HTTP 请求封装
 * 基于 uni.request 封装，支持拦截器、token 自动添加等功能
 */

import apiConfig from '@/config/api.config.js'
import { getToken, removeToken, isTokenValid, clearAuth } from './auth.js'

/**
 * 请求拦截器
 */
const requestInterceptor = (config) => {
  // 检查是否是认证接口（register/login），这些接口不需要 token
  const isAuthEndpoint = config.url.includes('/api/auth/')

  // 获取 token
  const token = getToken()

  // 如果不是认证接口且有 token，则验证 token
  if (!isAuthEndpoint && token) {
    // 验证 token 是否有效
    if (!isTokenValid(token)) {
      // token 无效或已过期，清除认证信息
      clearAuth()
      uni.showToast({
        title: '登录已过期，请重新登录',
        icon: 'none'
      })
      // 跳转到登录页
      setTimeout(() => {
        uni.reLaunch({
          url: '/pages/login/login'
        })
      }, 1500)
      // 返回 null 表示请求被拦截
      return null
    }

    // token 有效，添加到请求头
    config.header = config.header || {}
    config.header['Authorization'] = `Bearer ${token}`
  }

  // 打印请求日志
  if (apiConfig.showLog) {
    console.log('【请求】', config.url, config)
  }

  return config
}

/**
 * 响应拦截器
 */
const responseInterceptor = (response) => {
  // 打印响应日志
  if (apiConfig.showLog) {
    console.log('【响应】', response.config.url, response)
  }

  const { statusCode, data } = response

  // HTTP 状态码检查
  if (statusCode !== 200) {
    uni.showToast({
      title: `请求失败: ${statusCode}`,
      icon: 'none'
    })
    return Promise.reject(response)
  }

  // 业务状态码检查
  if (data.code !== 200) {
    // token 过期或无效
    if (data.code === 401) {
      removeToken()
      uni.showToast({
        title: '登录已过期，请重新登录',
        icon: 'none'
      })
      // 跳转到登录页
      setTimeout(() => {
        uni.reLaunch({
          url: '/pages/login/login'
        })
      }, 1500)
      return Promise.reject(data)
    }

    // 其他业务错误
    uni.showToast({
      title: data.message || '请求失败',
      icon: 'none'
    })
    return Promise.reject(data)
  }

  return data.data
}

/**
 * 发起请求
 */
const request = (options) => {
  // 合并配置
  const config = {
    url: apiConfig.baseURL + options.url,
    method: options.method || 'GET',
    data: options.data || {},
    header: {
      'Content-Type': 'application/json',
      ...options.header
    },
    timeout: options.timeout || apiConfig.timeout
  }

  // 请求拦截
  const interceptedConfig = requestInterceptor(config)

  // 如果拦截器返回 null，说明请求被拦截（如 token 无效）
  if (!interceptedConfig) {
    return Promise.reject({
      code: 401,
      message: '认证失败，请重新登录'
    })
  }

  // 发起请求
  return new Promise((resolve, reject) => {
    uni.request({
      ...interceptedConfig,
      success: (res) => {
        responseInterceptor(res)
          .then(resolve)
          .catch(reject)
      },
      fail: (err) => {
        console.error('【请求失败】', err)
        uni.showToast({
          title: '网络请求失败',
          icon: 'none'
        })
        reject(err)
      }
    })
  })
}

/**
 * GET 请求
 */
export const get = (url, data, options = {}) => {
  return request({
    url,
    method: 'GET',
    data,
    ...options
  })
}

/**
 * POST 请求
 */
export const post = (url, data, options = {}) => {
  return request({
    url,
    method: 'POST',
    data,
    ...options
  })
}

/**
 * PUT 请求
 */
export const put = (url, data, options = {}) => {
  return request({
    url,
    method: 'PUT',
    data,
    ...options
  })
}

/**
 * DELETE 请求
 */
export const del = (url, data, options = {}) => {
  return request({
    url,
    method: 'DELETE',
    data,
    ...options
  })
}

export default request
