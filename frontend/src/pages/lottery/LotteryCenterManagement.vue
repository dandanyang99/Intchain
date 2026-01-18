<template>
  <view class="lottery-center-management">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索彩票中心名称"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 彩票中心列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="center in centerList" :key="center.id">
          <view class="center-item">
            <view class="center-info" @click="viewDetail(center)">
              <view class="center-header">
                <text class="center-name">{{ center.name }}</text>
                <view class="status-badge" :class="center.isActive ? 'status-active' : 'status-inactive'">
                  {{ center.isActive ? '启用' : '停用' }}
                </view>
              </view>
              <view class="info-row">
                <text class="label">联系人:</text>
                <text class="value">{{ center.contactPerson }}</text>
              </view>
              <view class="info-row">
                <text class="label">联系电话:</text>
                <text class="value">{{ center.contactPhone }}</text>
              </view>
              <view class="info-row">
                <text class="label">地址:</text>
                <text class="value">{{ center.address }}</text>
              </view>
            </view>

            <!-- 操作按钮 -->
            <view class="action-buttons">
              <u-button type="primary" size="small" @click.stop="handleEdit(center)">编辑</u-button>
              <u-button type="error" size="small" @click.stop="handleDelete(center)">删除</u-button>
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
      v-if="!loading && centerList.length === 0"
      text="暂无彩票中心"
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
        <view class="dialog-title">{{ isEdit ? '编辑彩票中心' : '添加彩票中心' }}</view>
        <u-form :model="centerForm" ref="centerFormRef">
          <u-form-item label="中心名称" required>
            <u-input v-model="centerForm.name" placeholder="请输入中心名称"></u-input>
          </u-form-item>
          <u-form-item label="联系人" required>
            <u-input v-model="centerForm.contactPerson" placeholder="请输入联系人"></u-input>
          </u-form-item>
          <u-form-item label="联系电话" required>
            <u-input v-model="centerForm.contactPhone" placeholder="请输入联系电话"></u-input>
          </u-form-item>
          <u-form-item label="地址" required>
            <u-textarea v-model="centerForm.address" placeholder="请输入地址"></u-textarea>
          </u-form-item>
          <u-form-item label="状态">
            <u-switch v-model="centerForm.isActive"></u-switch>
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
import { getLotteryCenterList, createLotteryCenter, updateLotteryCenter, deleteLotteryCenter } from '@/api/lotteryCenter.js'

export default {
  data() {
    return {
      searchKeyword: '',
      centerList: [],
      loading: false,
      noMore: false,
      page: 1,
      pageSize: 10,
      showAddDialog: false,
      isEdit: false,
      submitting: false,
      centerForm: {
        id: null,
        name: '',
        contactPerson: '',
        contactPhone: '',
        address: '',
        isActive: true
      }
    }
  },
  onLoad() {
    this.loadCenterList()
  },
  methods: {
    // 加载彩票中心列表
    async loadCenterList(isRefresh = false) {
      if (this.loading) return

      this.loading = true

      try {
        const params = {
          page: isRefresh ? 1 : this.page,
          pageSize: this.pageSize,
          keyword: this.searchKeyword
        }
        const res = await getLotteryCenterList(params)

        if (isRefresh) {
          this.centerList = res.data || []
          this.page = 1
        } else {
          this.centerList = [...this.centerList, ...(res.data || [])]
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

    loadMore() {
      if (this.noMore || this.loading) return
      this.page++
      this.loadCenterList()
    },

    handleSearch() {
      this.page = 1
      this.noMore = false
      this.loadCenterList(true)
    },

    handleClear() {
      this.searchKeyword = ''
      this.handleSearch()
    },

    viewDetail(center) {
      uni.showModal({
        title: center.name,
        content: `联系人: ${center.contactPerson}\n电话: ${center.contactPhone}\n地址: ${center.address}`,
        showCancel: false
      })
    },

    handleEdit(center) {
      this.isEdit = true
      this.centerForm = {
        id: center.id,
        name: center.name,
        contactPerson: center.contactPerson,
        contactPhone: center.contactPhone,
        address: center.address,
        isActive: center.isActive
      }
      this.showAddDialog = true
    },

    handleDelete(center) {
      uni.showModal({
        title: '提示',
        content: `确定要删除彩票中心"${center.name}"吗？`,
        success: async (res) => {
          if (res.confirm) {
            try {
              await deleteLotteryCenter(center.id)
              uni.showToast({
                title: '删除成功',
                icon: 'success'
              })
              this.loadCenterList(true)
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

    async handleSubmit() {
      if (!this.centerForm.name) {
        uni.showToast({
          title: '请输入中心名称',
          icon: 'none'
        })
        return
      }

      if (!this.centerForm.contactPerson) {
        uni.showToast({
          title: '请输入联系人',
          icon: 'none'
        })
        return
      }

      if (!this.centerForm.contactPhone) {
        uni.showToast({
          title: '请输入联系电话',
          icon: 'none'
        })
        return
      }

      if (!this.centerForm.address) {
        uni.showToast({
          title: '请输入地址',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        const data = {
          name: this.centerForm.name,
          contactPerson: this.centerForm.contactPerson,
          contactPhone: this.centerForm.contactPhone,
          address: this.centerForm.address,
          isActive: this.centerForm.isActive
        }

        if (this.isEdit) {
          await updateLotteryCenter(this.centerForm.id, data)
          uni.showToast({
            title: '更新成功',
            icon: 'success'
          })
        } else {
          await createLotteryCenter(data)
          uni.showToast({
            title: '添加成功',
            icon: 'success'
          })
        }

        this.showAddDialog = false
        this.resetForm()
        this.loadCenterList(true)
      } catch (error) {
        uni.showToast({
          title: error.message || '操作失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    resetForm() {
      this.isEdit = false
      this.centerForm = {
        id: null,
        name: '',
        contactPerson: '',
        contactPhone: '',
        address: '',
        isActive: true
      }
    }
  }
}
</script>

<style scoped>
.lottery-center-management {
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

.center-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.center-info {
  margin-bottom: 20rpx;
}

.center-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.center-name {
  font-size: 30rpx;
  font-weight: 600;
  color: #333;
}

.status-badge {
  padding: 6rpx 16rpx;
  border-radius: 20rpx;
  font-size: 24rpx;
}

.status-active {
  background-color: #f6ffed;
  color: #52c41a;
}

.status-inactive {
  background-color: #fff1f0;
  color: #ff4d4f;
}

.info-row {
  display: flex;
  padding: 8rpx 0;
}

.info-row .label {
  font-size: 26rpx;
  color: #999;
  width: 140rpx;
}

.info-row .value {
  flex: 1;
  font-size: 26rpx;
  color: #333;
}

.action-buttons {
  display: flex;
  gap: 20rpx;
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
