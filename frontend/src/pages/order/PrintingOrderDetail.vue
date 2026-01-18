<template>
  <view class="printing-order-detail">
    <!-- 加载状态 -->
    <view v-if="loading" class="loading-container">
      <u-loading-icon mode="circle"></u-loading-icon>
      <text class="loading-text">加载中...</text>
    </view>

    <!-- 订单详情 -->
    <view v-else-if="orderDetail" class="detail-container">
      <!-- 订单状态卡片 -->
      <view class="status-card">
        <view class="status-header">
          <text class="order-number">{{ orderDetail.orderNumber }}</text>
          <view class="status-badge" :class="getStatusClass(orderDetail.status)">
            {{ getStatusText(orderDetail.status) }}
          </view>
        </view>
        <view class="status-time">
          <text class="time-label">创建时间：</text>
          <text class="time-value">{{ formatTime(orderDetail.createdAt) }}</text>
        </view>
      </view>

      <!-- 产品信息卡片 -->
      <view class="info-card">
        <view class="card-title">产品信息</view>
        <view class="info-row">
          <text class="label">产品ID：</text>
          <text class="value">{{ orderDetail.productId }}</text>
        </view>
        <view class="info-row">
          <text class="label">申请订单ID：</text>
          <text class="value">{{ orderDetail.applicationOrderId }}</text>
        </view>
        <view class="info-row">
          <text class="label">印刷数量：</text>
          <text class="value highlight">{{ orderDetail.quantity }}</text>
        </view>
        <view v-if="orderDetail.remarks" class="info-row">
          <text class="label">备注：</text>
          <text class="value">{{ orderDetail.remarks }}</text>
        </view>
      </view>

      <!-- 时间信息卡片 -->
      <view class="info-card">
        <view class="card-title">时间信息</view>
        <view class="info-row">
          <text class="label">创建时间：</text>
          <text class="value">{{ formatTime(orderDetail.createdAt) }}</text>
        </view>
        <view v-if="orderDetail.acceptedAt" class="info-row">
          <text class="label">接单时间：</text>
          <text class="value">{{ formatTime(orderDetail.acceptedAt) }}</text>
        </view>
        <view v-if="orderDetail.completedAt" class="info-row">
          <text class="label">完成时间：</text>
          <text class="value">{{ formatTime(orderDetail.completedAt) }}</text>
        </view>
        <view class="info-row">
          <text class="label">更新时间：</text>
          <text class="value">{{ formatTime(orderDetail.updatedAt) }}</text>
        </view>
      </view>

      <!-- 操作按钮 -->
      <view class="action-section">
        <!-- 待接单状态 -->
        <u-button
          v-if="orderDetail.status === 'Pending'"
          type="primary"
          @click="handleAccept"
          :loading="submitting"
          block
        >
          接单
        </u-button>

        <!-- 生产中状态 -->
        <u-button
          v-if="orderDetail.status === 'InProduction'"
          type="success"
          @click="handleCompleteProduction"
          :loading="submitting"
          block
        >
          完成生产
        </u-button>

        <!-- 待发货状态 -->
        <u-button
          v-if="orderDetail.status === 'WaitingShipment'"
          type="warning"
          @click="handleShip"
          :loading="submitting"
          block
        >
          发货
        </u-button>

        <!-- 已发货状态 -->
        <u-button
          v-if="orderDetail.status === 'Shipped'"
          type="info"
          @click="handleComplete"
          :loading="submitting"
          block
        >
          完成订单
        </u-button>

        <!-- 已完成状态 -->
        <view v-if="orderDetail.status === 'Completed'" class="completed-tip">
          <u-icon name="checkmark-circle-fill" size="40" color="#52c41a"></u-icon>
          <text class="tip-text">订单已完成</text>
        </view>
      </view>
    </view>

    <!-- 空状态 -->
    <u-empty v-else text="订单不存在" mode="data"></u-empty>
  </view>
</template>

<script>
import {
  getPrintingOrderDetail,
  acceptPrintingOrder,
  updatePrintingToWaitingShipment,
  updatePrintingToShipped,
  completePrintingOrder
} from '@/api/order.js'

export default {
  data() {
    return {
      orderId: null,
      orderDetail: null,
      loading: false,
      submitting: false
    }
  },
  onLoad(options) {
    if (options.id) {
      this.orderId = options.id
      this.loadOrderDetail()
    }
  },
  methods: {
    // 加载订单详情
    async loadOrderDetail() {
      if (!this.orderId) return

      this.loading = true

      try {
        const res = await getPrintingOrderDetail(this.orderId)
        this.orderDetail = res.data
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    // 接单
    handleAccept() {
      uni.showModal({
        title: '确认接单',
        content: '确定要接受这个印刷订单吗？',
        success: async (res) => {
          if (res.confirm) {
            this.submitting = true
            try {
              await acceptPrintingOrder(this.orderId)
              uni.showToast({
                title: '接单成功',
                icon: 'success'
              })
              this.loadOrderDetail()
            } catch (error) {
              uni.showToast({
                title: error.message || '接单失败',
                icon: 'none'
              })
            } finally {
              this.submitting = false
            }
          }
        }
      })
    },

    // 完成生产
    handleCompleteProduction() {
      uni.showModal({
        title: '完成生产',
        content: '确认已完成生产，准备发货？',
        success: async (res) => {
          if (res.confirm) {
            this.submitting = true
            try {
              await updatePrintingToWaitingShipment(this.orderId)
              uni.showToast({
                title: '生产完成',
                icon: 'success'
              })
              this.loadOrderDetail()
            } catch (error) {
              uni.showToast({
                title: error.message || '操作失败',
                icon: 'none'
              })
            } finally {
              this.submitting = false
            }
          }
        }
      })
    },

    // 发货
    handleShip() {
      uni.showModal({
        title: '确认发货',
        content: '确认已发货？',
        success: async (res) => {
          if (res.confirm) {
            this.submitting = true
            try {
              await updatePrintingToShipped(this.orderId)
              uni.showToast({
                title: '发货成功',
                icon: 'success'
              })
              this.loadOrderDetail()
            } catch (error) {
              uni.showToast({
                title: error.message || '发货失败',
                icon: 'none'
              })
            } finally {
              this.submitting = false
            }
          }
        }
      })
    },

    // 完成订单
    handleComplete() {
      uni.showModal({
        title: '完成订单',
        content: '确认完成这个订单？',
        success: async (res) => {
          if (res.confirm) {
            this.submitting = true
            try {
              await completePrintingOrder(this.orderId)
              uni.showToast({
                title: '订单已完成',
                icon: 'success'
              })
              this.loadOrderDetail()
            } catch (error) {
              uni.showToast({
                title: error.message || '操作失败',
                icon: 'none'
              })
            } finally {
              this.submitting = false
            }
          }
        }
      })
    },

    // 获取状态样式类
    getStatusClass(status) {
      const classMap = {
        'Pending': 'status-pending',
        'InProduction': 'status-production',
        'WaitingShipment': 'status-waiting',
        'Shipped': 'status-shipped',
        'Completed': 'status-completed'
      }
      return classMap[status] || ''
    },

    // 获取状态文本
    getStatusText(status) {
      const textMap = {
        'Pending': '待接单',
        'InProduction': '生产中',
        'WaitingShipment': '待发货',
        'Shipped': '已发货',
        'Completed': '已完成'
      }
      return textMap[status] || status
    },

    // 格式化时间
    formatTime(time) {
      if (!time) return ''
      const date = new Date(time)
      return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`
    }
  }
}</script>

<style scoped>
.printing-order-detail {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 100rpx;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 200rpx 0;
}

.loading-text {
  margin-top: 20rpx;
  font-size: 28rpx;
  color: #999;
}

.detail-container {
  padding: 20rpx;
}

.status-card {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.status-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
}

.order-number {
  font-size: 32rpx;
  font-weight: 600;
  color: #333;
}

.status-badge {
  padding: 6rpx 16rpx;
  border-radius: 20rpx;
  font-size: 24rpx;
}

.status-pending {
  background-color: #fff7e6;
  color: #faad14;
}

.status-production {
  background-color: #e6f7ff;
  color: #1890ff;
}

.status-waiting {
  background-color: #f0f5ff;
  color: #597ef7;
}

.status-shipped {
  background-color: #f9f0ff;
  color: #9254de;
}

.status-completed {
  background-color: #f6ffed;
  color: #52c41a;
}

.status-time {
  display: flex;
  align-items: center;
}

.time-label {
  font-size: 26rpx;
  color: #999;
}

.time-value {
  font-size: 26rpx;
  color: #666;
}

.info-card {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.card-title {
  font-size: 30rpx;
  font-weight: 600;
  color: #333;
  margin-bottom: 20rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.info-row {
  display: flex;
  padding: 12rpx 0;
}

.info-row .label {
  font-size: 28rpx;
  color: #999;
  width: 180rpx;
}

.info-row .value {
  flex: 1;
  font-size: 28rpx;
  color: #333;
}

.info-row .value.highlight {
  color: #ff9900;
  font-weight: 600;
}

.action-section {
  padding: 20rpx;
}

.completed-tip {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40rpx 0;
}

.tip-text {
  margin-top: 16rpx;
  font-size: 28rpx;
  color: #52c41a;
}
</style>
