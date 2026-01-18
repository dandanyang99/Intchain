<template>
  <view class="register-container">
    <!-- 顶部装饰 -->
    <view class="header-bg"></view>

    <!-- 注册表单 -->
    <view class="register-card">
      <!-- Logo和标题 -->
      <view class="logo-section">
        <image class="logo" src="/static/logo.png" mode="aspectFit"></image>
        <text class="app-name">用户注册</text>
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
              placeholder="请输入密码（至少6位）"
              prefixIcon="lock"
              :clearable="true"
              :border="true"
            ></u-input>
          </u-form-item>

          <!-- 确认密码 -->
          <u-form-item prop="confirmPassword">
            <u-input
              v-model="formData.confirmPassword"
              type="password"
              placeholder="请再次输入密码"
              prefixIcon="lock"
              :clearable="true"
              :border="true"
            ></u-input>
          </u-form-item>

          <!-- 角色选择 -->
          <u-form-item prop="role">
            <u-input
              v-model="roleText"
              placeholder="请选择角色"
              prefixIcon="list"
              :disabled="true"
              :border="true"
              @click="showRolePicker = true"
            ></u-input>
          </u-form-item>
        </u-form>

        <!-- 微信绑定选项 -->
        <view class="wechat-bind-option">
          <u-checkbox v-model="bindWeChat" shape="circle">
            注册后自动绑定微信账号（推荐）
          </u-checkbox>
        </view>

        <!-- 注册按钮 -->
        <u-button
          type="primary"
          :loading="loading"
          :disabled="loading"
          @click="handleRegister"
          class="register-btn"
        >
          {{ loading ? '注册中...' : '注册' }}
        </u-button>

        <!-- 登录链接 -->
        <view class="login-link">
          <text>已有账号？</text>
          <text class="link-text" @click="goToLogin">立即登录</text>
        </view>
      </view>
    </view>

    <!-- 角色选择器 -->
    <u-picker
      :show="showRolePicker"
      :columns="roleColumns"
      @confirm="onRoleConfirm"
      @cancel="showRolePicker = false"
    ></u-picker>
  </view>
</template>

<script>
import { register, getOpenId } from '@/api/user.js'
import { setToken, setUserInfo } from '@/utils/auth.js'

export default {
  data() {
    return {
      formData: {
        username: '',
        password: '',
        confirmPassword: '',
        role: ''
      },
      loading: false,
      bindWeChat: true, // 默认勾选绑定微信
      wechatCode: '', // 微信登录code
      showRolePicker: false,
      roleColumns: [
        [
          { text: '销售网点', value: 'SalesOutlet' },
          { text: '彩票中心', value: 'LotteryCenter' },
          { text: '印刷厂', value: 'PrintingFactory' }
        ]
      ]
    }
  },

  computed: {
    roleText() {
      const roleMap = {
        'SalesOutlet': '销售网点',
        'LotteryCenter': '彩票中心',
        'PrintingFactory': '印刷厂'
      }
      return roleMap[this.formData.role] || ''
    }
  },

  mounted() {
    // 如果默认勾选了绑定微信，则获取微信code
    if (this.bindWeChat) {
      this.getWeChatCode()
    }
  },

  watch: {
    // 监听绑定微信选项的变化
    bindWeChat(newVal) {
      if (newVal && !this.wechatCode) {
        this.getWeChatCode()
      }
    }
  },

  methods: {
    // 获取微信code
    async getWeChatCode() {
      try {
        const loginRes = await uni.login({
          provider: 'weixin'
        })

        if (loginRes[1] && loginRes[1].code) {
          this.wechatCode = loginRes[1].code
          console.log('获取微信code成功:', this.wechatCode)
        }
      } catch (error) {
        console.error('获取微信code失败:', error)
      }
    },

    // 角色选择确认
    onRoleConfirm(e) {
      this.formData.role = e.value[0].value
      this.showRolePicker = false
    },

    // 表单验证
    validateForm() {
      if (!this.formData.username) {
        uni.showToast({
          title: '请输入用户名',
          icon: 'none'
        })
        return false
      }

      if (this.formData.username.length > 50) {
        uni.showToast({
          title: '用户名长度不能超过50个字符',
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

      if (!this.formData.confirmPassword) {
        uni.showToast({
          title: '请再次输入密码',
          icon: 'none'
        })
        return false
      }

      if (this.formData.password !== this.formData.confirmPassword) {
        uni.showToast({
          title: '两次输入的密码不一致',
          icon: 'none'
        })
        return false
      }

      if (!this.formData.role) {
        uni.showToast({
          title: '请选择角色',
          icon: 'none'
        })
        return false
      }

      return true
    },

    // 处理注册
    async handleRegister() {
      // 验证表单
      if (!this.validateForm()) {
        return
      }

      this.loading = true

      try {
        // 准备注册数据
        const registerData = {
          username: this.formData.username,
          password: this.formData.password,
          role: this.formData.role
        }

        // 如果勾选了绑定微信且有code，则需要先获取OpenId
        if (this.bindWeChat && this.wechatCode) {
          try {
            // 调用获取OpenId接口
            const openIdResponse = await getOpenId(this.wechatCode)

            // 检查OpenId是否已被其他用户绑定
            if (openIdResponse.isAlreadyBound) {
              uni.showToast({
                title: '该微信账号已绑定其他用户',
                icon: 'none',
                duration: 2000
              })
              this.loading = false
              return
            }

            // 将OpenId添加到注册数据中
            registerData.openId = openIdResponse.openId
            console.log('获取OpenId成功，将绑定到新用户')
          } catch (error) {
            console.error('获取OpenId失败:', error)
            // 获取OpenId失败，提示用户但继续注册（不绑定微信）
            uni.showToast({
              title: '获取微信信息失败，将不绑定微信账号',
              icon: 'none',
              duration: 2000
            })
          }
        }

        // 调用注册API
        const response = await register(registerData)

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
          title: '注册成功',
          icon: 'success'
        })

        // 延迟跳转到首页
        setTimeout(() => {
          uni.reLaunch({
            url: '/pages/index/index'
          })
        }, 1500)

      } catch (error) {
        console.error('注册失败:', error)
        uni.showToast({
          title: error.message || '注册失败，请重试',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    // 跳转到登录页面
    goToLogin() {
      uni.navigateBack()
    }
  }
}
</script>

<style lang="scss" scoped>
.register-container {
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

.register-card {
  position: relative;
  margin: 150rpx 60rpx 0;
  background: #ffffff;
  border-radius: 40rpx;
  padding: 60rpx 60rpx;
  box-shadow: 0 20rpx 60rpx rgba(0, 0, 0, 0.1);
}

.logo-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 60rpx;
}

.logo {
  width: 140rpx;
  height: 140rpx;
  margin-bottom: 20rpx;
}

.app-name {
  font-size: 36rpx;
  font-weight: bold;
  color: #333333;
}

.form-section {
  width: 100%;
}

.wechat-bind-option {
  margin: 30rpx 0;
  padding: 20rpx;
  background: #f8f9fa;
  border-radius: 10rpx;
}

.register-btn {
  width: 100%;
  margin-top: 40rpx;
  height: 90rpx;
  border-radius: 45rpx;
  font-size: 32rpx;
}

.login-link {
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
