<template>
  <view class="approval-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索订单号"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 待审批订单列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="order in orderList" :key="order.id">
          <view class="order-item">
            <!-- 订单信息 -->
            <view class="order-info" @click="goToDetail(order.id)">
              <view class="order-header">
                <text class="order-number">{{ order.orderNumber }}</text>
                <view class="status-badge">待审批</view>
              </view>
              <view class="info-row">
                <text class="label">产品ID:</text>
                <text class="value">{{ order.productId }}</text>
              </view>
              <view class="info-row">
                <text class="label">数量:</text>
                <text class="value">{{ order.quantity }}</text>
              </view>
              <view class="info-row">
                <text class="label">申请时间:</text>
                <text class="value">{{ formatTime(order.createdAt) }}</text>
              </view>
            </view>

            <!-- 操作按钮 -->
            <view class="action-buttons">
              <u-button
                type="success"
                size="small"
                @click.stop="handleQuickApprove(order)"
              >通过</u-button>
              <u-button
                type="error"
                size="small"
                @click.stop="handleQuickReject(order)"
              >拒绝</u-button>
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
      v-if="!loading && orderList.length === 0"
      text="暂无待审批订单"
      mode="list"
    ></u-empty>

    <!-- 快速审批通过对话框 -->
    <u-popup v-model="showApprovalDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">审批通过</view>
        <u-form :model="approvalForm">
          <u-form-item label="批准数量">
            <u-number-box
              v-model="approvalForm.approvedQuantity"
              :min="1"
              :max="currentOrder ? currentOrder.quantity : 999"
            ></u-number-box>
          </u-form-item>
          <u-form-item label="印刷厂ID">
            <u-input
              v-model="approvalForm.printingFactoryId"
              placeholder="请输入印刷厂ID"
              type="number"
            ></u-input>
          </u-form-item>
          <u-form-item label="审批意见">
            <u-textarea
              v-model="approvalForm.approvalRemarks"
              placeholder="请输入审批意见（可选）"
              :maxlength="200"
            ></u-textarea>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showApprovalDialog = false">取消</u-button>
          <u-button
            type="primary"
            @click="confirmApprove"
            :loading="submitting"
          >确定</u-button>
        </view>
      </view>
    </u-popup>

    <!-- 快速拒绝对话框 -->
    <u-popup v-model="showRejectDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">审批拒绝</view>
        <u-form :model="rejectForm">
          <u-form-item label="拒绝原因">
            <u-textarea
              v-model="rejectForm.rejectionReason"
              placeholder="请输入拒绝原因"
              :maxlength="200"
            ></u-textarea>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showRejectDialog = false">取消</u-button>
          <u-button
            type="error"
            @click="confirmReject"
            :loading="submitting"
          >确定</u-button>
        </view>
      </view>
    </u-popup>
  </view>
</template>

<script>
import {
  getApplicationOrdersByStatus,
  approveApplicationOrder,
  rejectApplicationOrder
} from '@/api/order.js'

export default {
  data() {
    return {
      searchKeyword: '',
      orderList: [],
      loading: false,
      noMore: false,
      currentOrder: null,
      showApprovalDialog: false,
      showRejectDialog: false,
      submitting: false,
      approvalForm: {
        approvedQuantity: 1,
        printingFactoryId: '',
        approvalRemarks: ''
      },
      rejectForm: {
        rejectionReason: ''
      }
    }
  },
  onLoad() {
    this.loadPendingOrders()
  },
  methods: {
    // 加载待审批订单
    async loadPendingOrders() {
      if (this.loading) return

      this.loading = true

      try {
        const res = await getApplicationOrdersByStatus('Pending')
        this.orderList = res.data || []

        // 如果有搜索关键词，进行过滤
        if (this.searchKeyword) {
          this.orderList = this.orderList.filter(order =>
            order.orderNumber.includes(this.searchKeyword)
          )
        }

        this.noMore = true
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    // 加载更多（当前实现为一次性加载所有）
    loadMore() {
      // 当前实现为一次性加载所有待审批订单
      // 如果需要分页，可以在这里实现
    },

    // 搜索
    handleSearch() {
      this.loadPendingOrders()
    },

    // 清除搜索
    handleClear() {
      this.searchKeyword = ''
      this.loadPendingOrders()
    },

    // 跳转到订单详情
    goToDetail(orderId) {
      uni.navigateTo({
        url: `/pages/order/OrderDetail?id=${orderId}`
      })
    },

    // 快速审批通过
    handleQuickApprove(order) {
      this.currentOrder = order
      this.approvalForm.approvedQuantity = order.quantity
      this.approvalForm.printingFactoryId = ''
      this.approvalForm.approvalRemarks = ''
      this.showApprovalDialog = true
    },

    // 快速拒绝
    handleQuickReject(order) {
      this.currentOrder = order
      this.rejectForm.rejectionReason = ''
      this.showRejectDialog = true
    },

    // 确认审批通过
    async confirmApprove() {
      if (!this.approvalForm.printingFactoryId) {
        uni.showToast({
          title: '请输入印刷厂ID',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        await approveApplicationOrder(this.currentOrder.id, {
          approvedQuantity: this.approvalForm.approvedQuantity,
          printingFactoryId: parseInt(this.approvalForm.printingFactoryId),
          approvalRemarks: this.approvalForm.approvalRemarks
        })

        uni.showToast({
          title: '审批成功',
          icon: 'success'
        })

        this.showApprovalDialog = false
        this.loadPendingOrders()
      } catch (error) {
        uni.showToast({
          title: error.message || '审批失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    // 确认拒绝
    async confirmReject() {
      if (!this.rejectForm.rejectionReason) {
        uni.showToast({
          title: '请输入拒绝原因',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        await rejectApplicationOrder(this.currentOrder.id, {
          rejectionReason: this.rejectForm.rejectionReason
        })

        uni.showToast({
          title: '已拒绝',
          icon: 'success'
        })

        this.showRejectDialog = false
        this.loadPendingOrders()
      } catch (error) {
        uni.showToast({
          title: error.message || '操作失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    // 格式化时间
    formatTime(time) {
      if (!time) return ''
      const date = new Date(time)
      return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`
    }
  }
}
</script>

<style scoped>
.approval-page {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 100rpx;
}

.search-bar {
  padding: 20rpx;
  background-color: #fff;
}

.list-container {
  padding: 20rpx;
}

.order-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.order-info {
  margin-bottom: 20rpx;
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.order-number {
  font-size: 28rpx;
  font-weight: 600;
  color: #333;
}

.status-badge {
  padding: 6rpx 16rpx;
  border-radius: 20rpx;
  font-size: 24rpx;
  background-color: #fff7e6;
  color: #faad14;
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
