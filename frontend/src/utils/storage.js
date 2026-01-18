/**
 * 本地存储封装
 * 统一管理本地存储，支持同步和异步操作
 */

/**
 * 同步设置存储
 */
export const setStorageSync = (key, value) => {
  try {
    uni.setStorageSync(key, value)
    return true
  } catch (e) {
    console.error('setStorageSync error:', e)
    return false
  }
}

/**
 * 同步获取存储
 */
export const getStorageSync = (key) => {
  try {
    return uni.getStorageSync(key)
  } catch (e) {
    console.error('getStorageSync error:', e)
    return null
  }
}

/**
 * 同步移除存储
 */
export const removeStorageSync = (key) => {
  try {
    uni.removeStorageSync(key)
    return true
  } catch (e) {
    console.error('removeStorageSync error:', e)
    return false
  }
}

/**
 * 异步设置存储
 */
export const setStorage = (key, value) => {
  return new Promise((resolve, reject) => {
    uni.setStorage({
      key,
      data: value,
      success: () => resolve(true),
      fail: (err) => reject(err)
    })
  })
}

/**
 * 异步获取存储
 */
export const getStorage = (key) => {
  return new Promise((resolve, reject) => {
    uni.getStorage({
      key,
      success: (res) => resolve(res.data),
      fail: (err) => reject(err)
    })
  })
}

/**
 * 异步移除存储
 */
export const removeStorage = (key) => {
  return new Promise((resolve, reject) => {
    uni.removeStorage({
      key,
      success: () => resolve(true),
      fail: (err) => reject(err)
    })
  })
}

/**
 * 清空所有存储
 */
export const clearStorage = () => {
  return new Promise((resolve, reject) => {
    uni.clearStorage({
      success: () => resolve(true),
      fail: (err) => reject(err)
    })
  })
}
