<template>
  <view class="user-management">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索用户名"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 用户列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="user in userList" :key="user.id">
          <view class="user-item">
            <view class="user-info" @click="viewDetail(user)">
              <view class="user-header">
                <text class="username">{{ user.username }}</text>
                <view class="role-badge" :class="getRoleClass(user.role)">
                  {{ getRoleText(user.role) }}
                </view>
              </view>
              <view class="info-row">
                <text class="label">邮箱:</text>
                <text class="value">{{ user.email || '未设置' }}</text>
              </view>
              <view class="info-row">
                <text class="label">电话:</text>
                <text class="value">{{ user.phoneNumber || '未设置' }}</text>
              </view>
            </view>

            <!-- 操作按钮 -->
            <view class="action-buttons">
              <u-button type="primary" size="small" @click.stop="handleEdit(user)">编辑</u-button>
              <u-button type="warning" size="small" @click.stop="handleResetPassword(user)">重置密码</u-button>
              <u-button type="error" size="small" @click.stop="handleDelete(user)">删除</u-button>
            </view>
          </view>
        </u-list-item>
      </u-list>

      <!-- 加载状态 -->
      <view class="loading-status">
        <u-loading-icon v-if="loading" mode="circle"></u-loading-icon>
        <text v-else-if="noMore" class="no-more">没有更多数据了</text>
      </view>
    </view>

    <!-- 空状态 -->
    <u-empty
      v-if="!loading && userList.length === 0"
      text="暂无用户"
      mode="list"
    ></u-empty>

    <!-- 添加按钮 -->
    <view class="fab-button">
      <u-button
        type="primary"
        shape="circle"
        icon="plus"
        @click="showAddDialog = true"
      ></u-button>
    </view>

    <!-- 添加/编辑对话框 -->
    <u-popup v-model="showAddDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">{{ isEdit ? '编辑用户' : '添加用户' }}</view>
        <u-form :model="userForm" ref="userFormRef">
          <u-form-item label="用户名" required>
            <u-input v-model="userForm.username" placeholder="请输入用户名"></u-input>
          </u-form-item>
          <u-form-item label="密码" :required="!isEdit">
            <u-input v-model="userForm.password" type="password" placeholder="请输入密码"></u-input>
          </u-form-item>
          <u-form-item label="邮箱">
            <u-input v-model="userForm.email" placeholder="请输入邮箱"></u-input>
          </u-form-item>
          <u-form-item label="电话">
            <u-input v-model="userForm.phoneNumber" placeholder="请输入电话"></u-input>
          </u-form-item>
          <u-form-item label="角色" required>
            <u-input v-model="userForm.role" placeholder="请输入角色" disabled></u-input>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showAddDialog = false">取消</u-button>
          <u-button type="primary" @click="handleSubmit" :loading="submitting">确定</u-button>
        </view>
      </view>
    </u-popup>
  </view>
</template>

<script>
import { getUserList, createUser, updateUser, deleteUser, resetPassword } from '@/api/user.js'

export default {
  data() {
    return {
      searchKeyword: '',
      userList: [],
      loading: false,
      noMore: false,
      page: 1,
      pageSize: 10,
      showAddDialog: false,
      isEdit: false,
      submitting: false,
      userForm: {
        id: null,
        username: '',
        password: '',
        email: '',
        phoneNumber: '',
        role: 'SalesOutlet'
      }
    }
  },
  onLoad() {
    this.loadUserList()
  },
  methods: {
    // 加载用户列表
    async loadUserList(isRefresh = false) {
      if (this.loading) return

      this.loading = true

      try {
        const params = {
          page: isRefresh ? 1 : this.page,
          pageSize: this.pageSize,
          keyword: this.searchKeyword
        }
        const res = await getUserList(params)

        if (isRefresh) {
          this.userList = res.data || []
          this.page = 1
        } else {
          this.userList = [...this.userList, ...(res.data || [])]
        }

        if (!res.data || res.data.length < this.pageSize) {
          this.noMore = true
        } else {
          this.noMore = false
        }
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    // 加载更多
    loadMore() {
      if (this.noMore || this.loading) return
      this.page++
      this.loadUserList()
    },

    // 搜索
    handleSearch() {
      this.page = 1
      this.noMore = false
      this.loadUserList(true)
    },

    // 清除搜索
    handleClear() {
      this.searchKeyword = ''
      this.handleSearch()
    },

    // 查看详情
    viewDetail(user) {
      uni.showModal({
        title: user.username,
        content: `角色: ${this.getRoleText(user.role)}\n邮箱: ${user.email || '未设置'}\n电话: ${user.phoneNumber || '未设置'}`,
        showCancel: false
      })
    },

    // 编辑用户
    handleEdit(user) {
      this.isEdit = true
      this.userForm = {
        id: user.id,
        username: user.username,
        password: '',
        email: user.email,
        phoneNumber: user.phoneNumber,
        role: user.role
      }
      this.showAddDialog = true
    },

    // 删除用户
    handleDelete(user) {
      uni.showModal({
        title: '提示',
        content: `确定要删除用户"${user.username}"吗？`,
        success: async (res) => {
          if (res.confirm) {
            try {
              await deleteUser(user.id)
              uni.showToast({
                title: '删除成功',
                icon: 'success'
              })
              this.loadUserList(true)
            } catch (error) {
              uni.showToast({
                title: error.message || '删除失败',
                icon: 'none'
              })
            }
          }
        }
      })
    },

    // 重置密码
    handleResetPassword(user) {
      uni.showModal({
        title: '重置密码',
        content: `确定要重置用户"${user.username}"的密码吗？`,
        success: async (res) => {
          if (res.confirm) {
            try {
              await resetPassword(user.id, { newPassword: '123456' })
              uni.showToast({
                title: '密码已重置为123456',
                icon: 'success',
                duration: 3000
              })
            } catch (error) {
              uni.showToast({
                title: error.message || '重置失败',
                icon: 'none'
              })
            }
          }
        }
      })
    },

    // 提交表单
    async handleSubmit() {
      if (!this.userForm.username) {
        uni.showToast({
          title: '请输入用户名',
          icon: 'none'
        })
        return
      }

      if (!this.isEdit && !this.userForm.password) {
        uni.showToast({
          title: '请输入密码',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        const data = {
          username: this.userForm.username,
          email: this.userForm.email,
          phoneNumber: this.userForm.phoneNumber,
          role: this.userForm.role
        }

        if (!this.isEdit) {
          data.password = this.userForm.password
        }

        if (this.isEdit) {
          await updateUser(this.userForm.id, data)
          uni.showToast({
            title: '更新成功',
            icon: 'success'
          })
        } else {
          await createUser(data)
          uni.showToast({
            title: '添加成功',
            icon: 'success'
          })
        }

        this.showAddDialog = false
        this.resetForm()
        this.loadUserList(true)
      } catch (error) {
        uni.showToast({
          title: error.message || '操作失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    // 重置表单
    resetForm() {
      this.isEdit = false
      this.userForm = {
        id: null,
        username: '',
        password: '',
        email: '',
        phoneNumber: '',
        role: 'SalesOutlet'
      }
    },

    // 获取角色样式类
    getRoleClass(role) {
      const classMap = {
        'Admin': 'role-admin',
        'LotteryCenter': 'role-lottery',
        'PrintingFactory': 'role-factory',
        'SalesOutlet': 'role-outlet'
      }
      return classMap[role] || ''
    },

    // 获取角色文本
    getRoleText(role) {
      const textMap = {
        'Admin': '管理员',
        'LotteryCenter': '彩票中心',
        'PrintingFactory': '印刷厂',
        'SalesOutlet': '销售网点'
      }
      return textMap[role] || role
    }
  }
}
</script>

<style scoped>
.user-management {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 100rpx;
}

.search-bar {
  padding: 20rpx;
  background-color: #fff;
  margin-bottom: 20rpx;
}

.list-container {
  padding: 20rpx;
}

.user-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.user-info {
  margin-bottom: 20rpx;
}

.user-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.username {
  font-size: 30rpx;
  font-weight: 600;
  color: #333;
}

.role-badge {
  padding: 6rpx 16rpx;
  border-radius: 20rpx;
  font-size: 24rpx;
}

.role-admin {
  background-color: #fff1f0;
  color: #ff4d4f;
}

.role-lottery {
  background-color: #e6f7ff;
  color: #1890ff;
}

.role-factory {
  background-color: #fff7e6;
  color: #faad14;
}

.role-outlet {
  background-color: #f6ffed;
  color: #52c41a;
}

.info-row {
  display: flex;
  padding: 8rpx 0;
}

.info-row .label {
  font-size: 26rpx;
  color: #999;
  width: 120rpx;
}

.info-row .value {
  flex: 1;
  font-size: 26rpx;
  color: #333;
}

.action-buttons {
  display: flex;
  gap: 15rpx;
}

.action-buttons button {
  flex: 1;
}

.loading-status {
  display: flex;
  justify-content: center;
  padding: 40rpx 0;
}

.no-more {
  font-size: 28rpx;
  color: #999;
}

.fab-button {
  position: fixed;
  right: 40rpx;
  bottom: 100rpx;
  z-index: 999;
}

.dialog-content {
  padding: 40rpx;
  width: 600rpx;
}

.dialog-title {
  font-size: 36rpx;
  font-weight: 600;
  color: #333;
  text-align: center;
  margin-bottom: 30rpx;
}

.dialog-buttons {
  display: flex;
  justify-content: space-between;
  margin-top: 30rpx;
  gap: 20rpx;
}

.dialog-buttons button {
  flex: 1;
}
</style>
