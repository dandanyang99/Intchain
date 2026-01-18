<template>
  <view class="login-container">
    <!-- 顶部装饰 -->
    <view class="header-bg"></view>

    <!-- 登录表单 -->
    <view class="login-card">
      <!-- Logo和标题 -->
      <view class="logo-section">
        <image class="logo" src="/static/logo.png" mode="aspectFit"></image>
        <text class="app-name">彩票物流管理系统</text>
      </view>

      <!-- 表单 -->
      <view class="form-section">
        <u-form :model="formData" ref="formRef">
          <!-- 用户名 -->
          <u-form-item prop="username">
            <u-input
              v-model="formData.username"
              placeholder="请输入用户名"
              prefixIcon="account"
              :clearable="true"
              :border="true"
            ></u-input>
          </u-form-item>

          <!-- 密码 -->
          <u-form-item prop="password">
            <u-input
              v-model="formData.password"
              type="password"
              placeholder="请输入密码"
              prefixIcon="lock"
              :clearable="true"
              :border="true"
            ></u-input>
          </u-form-item>
        </u-form>

        <!-- 登录按钮 -->
        <u-button
          type="primary"
          :loading="loading"
          :disabled="loading"
          @click="handleLogin"
          class="login-btn"
        >
          {{ loading ? '登录中...' : '登录' }}
        </u-button>

        <!-- 注册链接 -->
        <view class="register-link">
          <text>还没有账号？</text>
          <text class="link-text" @click="goToRegister">立即注册</text>
        </view>
      </view>
    </view>
  </view>
</template>

<script>
import { login } from '@/api/user.js'
import { setToken, setUserInfo } from '@/utils/auth.js'

export default {
  data() {
    return {
      formData: {
        username: '',
        password: ''
      },
      loading: false
    }
  },

  methods: {
    // 表单验证
    validateForm() {
      if (!this.formData.username) {
        uni.showToast({
          title: '请输入用户名',
          icon: 'none'
        })
        return false
      }

      if (!this.formData.password) {
        uni.showToast({
          title: '请输入密码',
          icon: 'none'
        })
        return false
      }

      if (this.formData.password.length < 6) {
        uni.showToast({
          title: '密码长度不能少于6位',
          icon: 'none'
        })
        return false
      }

      return true
    },

    // 处理登录
    async handleLogin() {
      // 验证表单
      if (!this.validateForm()) {
        return
      }

      this.loading = true

      try {
        // 调用登录API
        const response = await login({
          username: this.formData.username,
          password: this.formData.password
        })

        // 保存token
        if (response.token) {
          setToken(response.token)
        }

        // 保存用户信息
        const userInfo = {
          userId: response.userId,
          username: response.username,
          role: response.role,
          entityId: response.entityId
        }
        setUserInfo(userInfo)

        // 显示成功提示
        uni.showToast({
          title: '登录成功',
          icon: 'success'
        })

        // 延迟跳转到首页
        setTimeout(() => {
          uni.reLaunch({
            url: '/pages/index/index'
          })
        }, 1500)

      } catch (error) {
        console.error('登录失败:', error)
        uni.showToast({
          title: error.message || '登录失败，请重试',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    // 跳转到注册页面
    goToRegister() {
      uni.navigateTo({
        url: '/pages/register/register'
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.login-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  position: relative;
  overflow: hidden;
}

.header-bg {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 400rpx;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 0 0 50% 50%;
}

.login-card {
  position: relative;
  margin: 200rpx 60rpx 0;
  background: #ffffff;
  border-radius: 40rpx;
  padding: 80rpx 60rpx;
  box-shadow: 0 20rpx 60rpx rgba(0, 0, 0, 0.1);
}

.logo-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 80rpx;
}

.logo {
  width: 160rpx;
  height: 160rpx;
  margin-bottom: 30rpx;
}

.app-name {
  font-size: 40rpx;
  font-weight: bold;
  color: #333333;
}

.form-section {
  width: 100%;
}

.login-btn {
  width: 100%;
  margin-top: 60rpx;
  height: 90rpx;
  border-radius: 45rpx;
  font-size: 32rpx;
}

.register-link {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 40rpx;
  font-size: 28rpx;
  color: #999999;
}

.link-text {
  color: #667eea;
  margin-left: 10rpx;
}
</style>
