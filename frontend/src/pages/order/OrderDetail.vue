<template>
  <view class="order-detail">
    <view v-if="loading" class="loading-container">
      <u-loading-icon mode="circle"></u-loading-icon>
    </view>

    <view v-else-if="order" class="detail-container">
      <!-- 状态卡片 -->
      <view class="status-card">
        <view class="status-badge" :class="getStatusClass(order.status)">
          {{ getStatusText(order.status) }}
        </view>
        <text class="order-number">{{ order.orderNumber }}</text>
      </view>

      <!-- 订单信息 -->
      <view class="info-card">
        <view class="card-title">订单信息</view>
        <view class="info-item">
          <text class="label">产品ID</text>
          <text class="value">{{ order.productId }}</text>
        </view>
        <view class="info-item">
          <text class="label">申请数量</text>
          <text class="value">{{ order.quantity }}</text>
        </view>
        <view class="info-item">
          <text class="label">销售网点ID</text>
          <text class="value">{{ order.salesOutletId }}</text>
        </view>
        <view class="info-item">
          <text class="label">彩票中心ID</text>
          <text class="value">{{ order.lotteryCenterId }}</text>
        </view>
        <view class="info-item" v-if="order.remarks">
          <text class="label">备注</text>
          <text class="value">{{ order.remarks }}</text>
        </view>
      </view>

      <!-- 时间信息 -->
      <view class="info-card">
        <view class="card-title">时间信息</view>
        <view class="info-item">
          <text class="label">创建时间</text>
          <text class="value">{{ formatTime(order.createdAt) }}</text>
        </view>
        <view class="info-item">
          <text class="label">更新时间</text>
          <text class="value">{{ formatTime(order.updatedAt) }}</text>
        </view>
      </view>

      <!-- 操作按钮 -->
      <view class="action-buttons" v-if="showActions">
        <!-- 彩票中心审批按钮 -->
        <template v-if="userRole === 'LotteryCenter' && order.status === 'Pending'">
          <u-button type="success" @click="showApprovalDialog = true" block>审批通过</u-button>
          <u-button type="error" @click="showRejectDialog = true" block style="margin-top: 20rpx;">审批拒绝</u-button>
        </template>

        <!-- 其他状态操作按钮 -->
        <template v-if="canUpdateStatus">
          <u-button type="primary" @click="handleStatusUpdate" block>{{ getActionText() }}</u-button>
        </template>
      </view>
    </view>

    <!-- 审批通过对话框 -->
    <u-popup v-model="showApprovalDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">审批通过</view>
        <u-form :model="approvalForm" ref="approvalFormRef">
          <u-form-item label="批准数量" required>
            <u-number-box v-model="approvalForm.approvedQuantity" :min="1" :max="order.quantity"></u-number-box>
          </u-form-item>
          <u-form-item label="印刷厂ID" required>
            <u-input v-model="approvalForm.printingFactoryId" placeholder="请输入印刷厂ID"></u-input>
          </u-form-item>
          <u-form-item label="审批意见">
            <u-textarea v-model="approvalForm.approvalRemarks" placeholder="请输入审批意见（可选）"></u-textarea>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showApprovalDialog = false">取消</u-button>
          <u-button type="primary" @click="handleApprove" :loading="submitting">确定</u-button>
        </view>
      </view>
    </u-popup>

    <!-- 审批拒绝对话框 -->
    <u-popup v-model="showRejectDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">审批拒绝</view>
        <u-form :model="rejectForm" ref="rejectFormRef">
          <u-form-item label="拒绝原因" required>
            <u-textarea v-model="rejectForm.rejectionReason" placeholder="请输入拒绝原因"></u-textarea>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showRejectDialog = false">取消</u-button>
          <u-button type="error" @click="handleReject" :loading="submitting">确定</u-button>
        </view>
      </view>
    </u-popup>
  </view>
</template>

<script>
import {
  getApplicationOrderDetail,
  approveApplicationOrder,
  rejectApplicationOrder,
  updateToWaitingShipment,
  updateToShipped,
  updateToInTransit,
  completeApplicationOrder
} from '@/api/order.js'

export default {
  data() {
    return {
      orderId: null,
      order: null,
      loading: false,
      userRole: '',
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
  computed: {
    showActions() {
      if (!this.order) return false

      // 彩票中心可以审批待审批的订单
      if (this.userRole === 'LotteryCenter' && this.order.status === 'Pending') {
        return true
      }

      // 其他角色的状态更新权限
      return this.canUpdateStatus
    },
    canUpdateStatus() {
      if (!this.order) return false

      const statusMap = {
        'Approved': this.userRole === 'LotteryCenter',
        'WaitingShipment': this.userRole === 'PrintingFactory',
        'Shipped': this.userRole === 'PrintingFactory',
        'InTransit': this.userRole === 'SalesOutlet'
      }

      return statusMap[this.order.status] || false
    }
  },
  onLoad(options) {
    if (options.id) {
      this.orderId = parseInt(options.id)
      this.getUserRole()
      this.loadOrderDetail()
    }
  },
  methods: {
    getUserRole() {
      const userInfo = uni.getStorageSync('userInfo')
      this.userRole = userInfo?.role || ''
    },

    async loadOrderDetail() {
      this.loading = true
      try {
        const res = await getApplicationOrderDetail(this.orderId)
        this.order = res.data
        this.approvalForm.approvedQuantity = this.order.quantity
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    async handleApprove() {
      if (!this.approvalForm.printingFactoryId) {
        uni.showToast({
          title: '请输入印刷厂ID',
          icon: 'none'
        })
        return
      }

      this.submitting = true
      try {
        await approveApplicationOrder(this.orderId, {
          approvedQuantity: this.approvalForm.approvedQuantity,
          printingFactoryId: parseInt(this.approvalForm.printingFactoryId),
          approvalRemarks: this.approvalForm.approvalRemarks
        })

        uni.showToast({
          title: '审批成功',
          icon: 'success'
        })

        this.showApprovalDialog = false
        this.loadOrderDetail()
      } catch (error) {
        uni.showToast({
          title: error.message || '审批失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    async handleReject() {
      if (!this.rejectForm.rejectionReason) {
        uni.showToast({
          title: '请输入拒绝原因',
          icon: 'none'
        })
        return
      }

      this.submitting = true
      try {
        await rejectApplicationOrder(this.orderId, {
          rejectionReason: this.rejectForm.rejectionReason
        })

        uni.showToast({
          title: '已拒绝',
          icon: 'success'
        })

        this.showRejectDialog = false
        this.loadOrderDetail()
      } catch (error) {
        uni.showToast({
          title: error.message || '操作失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    async handleStatusUpdate() {
      const actionMap = {
        'Approved': updateToWaitingShipment,
        'WaitingShipment': updateToShipped,
        'Shipped': updateToInTransit,
        'InTransit': completeApplicationOrder
      }

      const action = actionMap[this.order.status]
      if (!action) return

      try {
        await action(this.orderId)
        uni.showToast({
          title: '状态更新成功',
          icon: 'success'
        })
        this.loadOrderDetail()
      } catch (error) {
        uni.showToast({
          title: error.message || '更新失败',
          icon: 'none'
        })
      }
    },

    getActionText() {
      const textMap = {
        'Approved': '更新为待发货',
        'WaitingShipment': '更新为已发货',
        'Shipped': '更新为运输中',
        'InTransit': '确认收货'
      }
      return textMap[this.order.status] || '更新状态'
    },

    getStatusClass(status) {
      const classMap = {
        'Pending': 'status-pending',
        'Approved': 'status-approved',
        'Rejected': 'status-rejected',
        'WaitingShipment': 'status-waiting',
        'Shipped': 'status-shipped',
        'InTransit': 'status-transit',
        'Completed': 'status-completed'
      }
      return classMap[status] || ''
    },

    getStatusText(status) {
      const textMap = {
        'Pending': '待审批',
        'Approved': '已通过',
        'Rejected': '已拒绝',
        'WaitingShipment': '待发货',
        'Shipped': '已发货',
        'InTransit': '运输中',
        'Completed': '已完成'
      }
      return textMap[status] || status
    },

    formatTime(time) {
      if (!time) return ''
      const date = new Date(time)
      return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')} ${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`
    }
  }
}
</script>

<style scoped>
.order-detail {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 100rpx;
}

.loading-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
}

.detail-container {
  padding: 20rpx;
}

.status-card {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 40rpx;
  margin-bottom: 20rpx;
  text-align: center;
}

.status-badge {
  display: inline-block;
  padding: 12rpx 32rpx;
  border-radius: 40rpx;
  font-size: 28rpx;
  font-weight: 600;
  margin-bottom: 20rpx;
}

.status-pending {
  background-color: #fff7e6;
  color: #faad14;
}

.status-approved {
  background-color: #f6ffed;
  color: #52c41a;
}

.status-rejected {
  background-color: #fff1f0;
  color: #ff4d4f;
}

.status-waiting {
  background-color: #e6f7ff;
  color: #1890ff;
}

.status-shipped {
  background-color: #f0f5ff;
  color: #597ef7;
}

.status-transit {
  background-color: #f9f0ff;
  color: #9254de;
}

.status-completed {
  background-color: #f6ffed;
  color: #52c41a;
}

.order-number {
  font-size: 32rpx;
  color: #333;
  font-weight: 600;
}

.info-card {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 30rpx;
  margin-bottom: 20rpx;
}

.card-title {
  font-size: 32rpx;
  font-weight: 600;
  color: #333;
  margin-bottom: 24rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 16rpx 0;
  border-bottom: 1rpx solid #f8f8f8;
}

.info-item:last-child {
  border-bottom: none;
}

.info-item .label {
  font-size: 28rpx;
  color: #999;
}

.info-item .value {
  font-size: 28rpx;
  color: #333;
  font-weight: 500;
}

.action-buttons {
  padding: 20rpx;
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
