<template>
  <view class="outlet-management">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索网点名称"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 销售网点列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="outlet in outletList" :key="outlet.id">
          <view class="outlet-item">
            <view class="outlet-info" @click="viewDetail(outlet)">
              <view class="outlet-header">
                <text class="outlet-name">{{ outlet.name }}</text>
                <view class="status-badge" :class="outlet.isActive ? 'status-active' : 'status-inactive'">
                  {{ outlet.isActive ? '启用' : '停用' }}
                </view>
              </view>
              <view class="info-row">
                <text class="label">联系人:</text>
                <text class="value">{{ outlet.contactPerson }}</text>
              </view>
              <view class="info-row">
                <text class="label">联系电话:</text>
                <text class="value">{{ outlet.contactPhone }}</text>
              </view>
              <view class="info-row">
                <text class="label">地址:</text>
                <text class="value">{{ outlet.address }}</text>
              </view>
            </view>

            <!-- 操作按钮 -->
            <view class="action-buttons">
              <u-button type="primary" size="small" @click.stop="handleEdit(outlet)">编辑</u-button>
              <u-button type="error" size="small" @click.stop="handleDelete(outlet)">删除</u-button>
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
      v-if="!loading && outletList.length === 0"
      text="暂无销售网点"
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
        <view class="dialog-title">{{ isEdit ? '编辑网点' : '添加网点' }}</view>
        <u-form :model="outletForm" ref="outletFormRef">
          <u-form-item label="网点名称" required>
            <u-input v-model="outletForm.name" placeholder="请输入网点名称"></u-input>
          </u-form-item>
          <u-form-item label="联系人" required>
            <u-input v-model="outletForm.contactPerson" placeholder="请输入联系人"></u-input>
          </u-form-item>
          <u-form-item label="联系电话" required>
            <u-input v-model="outletForm.contactPhone" placeholder="请输入联系电话"></u-input>
          </u-form-item>
          <u-form-item label="地址" required>
            <u-textarea v-model="outletForm.address" placeholder="请输入地址"></u-textarea>
          </u-form-item>
          <u-form-item label="状态">
            <u-switch v-model="outletForm.isActive"></u-switch>
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
import { getOutletList, createOutlet, updateOutlet, deleteOutlet } from '@/api/outlet.js'

export default {
  data() {
    return {
      searchKeyword: '',
      outletList: [],
      loading: false,
      noMore: false,
      page: 1,
      pageSize: 10,
      showAddDialog: false,
      isEdit: false,
      submitting: false,
      outletForm: {
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
    this.loadOutletList()
  },
  methods: {
    // 加载销售网点列表
    async loadOutletList(isRefresh = false) {
      if (this.loading) return

      this.loading = true

      try {
        const params = {
          page: isRefresh ? 1 : this.page,
          pageSize: this.pageSize,
          keyword: this.searchKeyword
        }
        const res = await getOutletList(params)

        if (isRefresh) {
          this.outletList = res.data || []
          this.page = 1
        } else {
          this.outletList = [...this.outletList, ...(res.data || [])]
        }

        // 判断是否还有更多数据
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
      this.loadOutletList()
    },

    // 搜索
    handleSearch() {
      this.page = 1
      this.noMore = false
      this.loadOutletList(true)
    },

    // 清除搜索
    handleClear() {
      this.searchKeyword = ''
      this.handleSearch()
    },

    // 查看详情
    viewDetail(outlet) {
      uni.showModal({
        title: outlet.name,
        content: `联系人: ${outlet.contactPerson}\n电话: ${outlet.contactPhone}\n地址: ${outlet.address}`,
        showCancel: false
      })
    },

    // 编辑网点
    handleEdit(outlet) {
      this.isEdit = true
      this.outletForm = {
        id: outlet.id,
        name: outlet.name,
        contactPerson: outlet.contactPerson,
        contactPhone: outlet.contactPhone,
        address: outlet.address,
        isActive: outlet.isActive
      }
      this.showAddDialog = true
    },

    // 删除网点
    handleDelete(outlet) {
      uni.showModal({
        title: '提示',
        content: `确定要删除网点"${outlet.name}"吗？`,
        success: async (res) => {
          if (res.confirm) {
            try {
              await deleteOutlet(outlet.id)
              uni.showToast({
                title: '删除成功',
                icon: 'success'
              })
              this.loadOutletList(true)
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

    // 提交表单
    async handleSubmit() {
      // 表单验证
      if (!this.outletForm.name) {
        uni.showToast({
          title: '请输入网点名称',
          icon: 'none'
        })
        return
      }

      if (!this.outletForm.contactPerson) {
        uni.showToast({
          title: '请输入联系人',
          icon: 'none'
        })
        return
      }

      if (!this.outletForm.contactPhone) {
        uni.showToast({
          title: '请输入联系电话',
          icon: 'none'
        })
        return
      }

      if (!this.outletForm.address) {
        uni.showToast({
          title: '请输入地址',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        const userInfo = uni.getStorageSync('userInfo')
        const data = {
          name: this.outletForm.name,
          contactPerson: this.outletForm.contactPerson,
          contactPhone: this.outletForm.contactPhone,
          address: this.outletForm.address,
          isActive: this.outletForm.isActive,
          lotteryCenterId: userInfo.lotteryCenterId
        }

        if (this.isEdit) {
          await updateOutlet(this.outletForm.id, data)
          uni.showToast({
            title: '更新成功',
            icon: 'success'
          })
        } else {
          await createOutlet(data)
          uni.showToast({
            title: '添加成功',
            icon: 'success'
          })
        }

        this.showAddDialog = false
        this.resetForm()
        this.loadOutletList(true)
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
      this.outletForm = {
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
.outlet-management {
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

.outlet-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.outlet-info {
  margin-bottom: 20rpx;
}

.outlet-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.outlet-name {
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
