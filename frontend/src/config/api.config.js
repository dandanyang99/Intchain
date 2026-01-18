/**
 * API 配置文件
 * 配置 API 网关地址和相关参数
 */

// 开发环境配置
const devConfig = {
  // API 网关地址
  baseURL: 'http://localhost:5199',
  // 请求超时时间（毫秒）
  timeout: 30000,
  // 是否显示请求日志
  showLog: true
}

// 生产环境配置
const prodConfig = {
  // API 网关地址（生产环境需要替换为实际地址）
  baseURL: 'https://api.intchain.com',
  // 请求超时时间（毫秒）
  timeout: 30000,
  // 是否显示请求日志
  showLog: false
}

// 根据环境变量选择配置
const config = process.env.NODE_ENV === 'production' ? prodConfig : devConfig

export default config
