<template>
  <view class="product-detail">
    <!-- 加载状态 -->
    <u-loading-page :loading="loading"></u-loading-page>

    <view v-if="!loading && product" class="content">
      <!-- 产品基本信息 -->
      <view class="info-card">
        <view class="product-header">
          <view class="product-name">{{ product.name }}</view>
          <view class="product-price">¥{{ product.unitPrice }}</view>
        </view>

        <view class="info-row">
          <text class="label">产品ID:</text>
          <text class="value">{{ product.id }}</text>
        </view>

        <view class="info-row">
          <text class="label">发布时间:</text>
          <text class="value">{{ formatTime(product.createdAt) }}</text>
        </view>

        <view class="info-row">
          <text class="label">更新时间:</text>
          <text class="value">{{ formatTime(product.updatedAt) }}</text>
        </view>
      </view>

      <!-- 库存信息 -->
      <view class="info-card">
        <view class="card-title">库存信息</view>

        <view class="stock-grid">
          <view class="stock-card total">
            <view class="stock-label">总库存</view>
            <view class="stock-number">{{ inventoryStats.totalStock || product.totalStock }}</view>
          </view>

          <view class="stock-card available">
            <view class="stock-label">可用库存</view>
            <view class="stock-number">{{ inventoryStats.availableStock || product.availableStock }}</view>
          </view>

          <view class="stock-card reserved">
            <view class="stock-label">预留库存</view>
            <view class="stock-number">{{ inventoryStats.reservedStock || product.reservedStock }}</view>
          </view>

          <view class="stock-card sold">
            <view class="stock-label">已售库存</view>
            <view class="stock-number">{{ inventoryStats.soldStock || 0 }}</view>
          </view>
        </view>
      </view>

      <!-- 申请数量（仅销售网点可见） -->
      <view v-if="userRole === 'SalesOutlet'" class="info-card">
        <view class="card-title">申请数量</view>

        <view class="quantity-selector">
          <u-number-box
            v-model="applyQuantity"
            :min="1"
            :max="inventoryStats.availableStock || product.availableStock"
          ></u-number-box>
        </view>

        <view class="quantity-tip">
          可申请数量: {{ inventoryStats.availableStock || product.availableStock }}
        </view>
      </view>

      <!-- 操作按钮 -->
      <view class="action-buttons">
        <!-- 销售网点：申请产品 -->
        <u-button
          v-if="userRole === 'SalesOutlet'"
          type="primary"
          size="large"
          :disabled="!canApply"
          @click="handleApply"
        >
          立即申请
        </u-button>

        <!-- 彩票中心：编辑产品 -->
        <u-button
          v-if="userRole === 'LotteryCenter'"
          type="primary"
          size="large"
          @click="handleEdit"
        >
          编辑产品
        </u-button>
      </view>
    </view>
  </view>
</template>

<script>
import { getProductDetail, getInventoryStats } from '@/api/product.js'

export default {
  data() {
    return {
      productId: null,
      product: null,
      inventoryStats: {},
      loading: true,
      userRole: '',
      applyQuantity: 1
    }
  },
  computed: {
    canApply() {
      const availableStock = this.inventoryStats.availableStock || this.product?.availableStock || 0
      return availableStock > 0 && this.applyQuantity > 0 && this.applyQuantity <= availableStock
    }
  },
  onLoad(options) {
    this.productId = options.id
    this.getUserRole()
    this.loadProductDetail()
    this.loadInventoryStats()
  },
  methods: {
    // 获取用户角色
    getUserRole() {
      const userInfo = uni.getStorageSync('userInfo')
      this.userRole = userInfo?.role || ''
    },

    // 加载产品详情
    async loadProductDetail() {
      try {
        const res = await getProductDetail(this.productId)
        this.product = res.data
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      } finally {
        this.loading = false
      }
    },

    // 加载库存统计
    async loadInventoryStats() {
      try {
        const res = await getInventoryStats(this.productId)
        this.inventoryStats = res.data
      } catch (error) {
        console.error('加载库存统计失败:', error)
      }
    },

    // 申请产品
    handleApply() {
      uni.navigateTo({
        url: `/pages/order/CreateOrder?productId=${this.productId}&quantity=${this.applyQuantity}`
      })
    },

    // 编辑产品
    handleEdit() {
      uni.navigateTo({
        url: `/pages/product/ProductPublish?id=${this.productId}&mode=edit`
      })
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
.product-detail {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 120rpx;
}

.content {
  padding: 20rpx;
}

.info-card {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 32rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.product-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 24rpx;
  border-bottom: 1rpx solid #f0f0f0;
  margin-bottom: 24rpx;
}

.product-name {
  font-size: 36rpx;
  font-weight: 700;
  color: #333;
}

.product-price {
  font-size: 40rpx;
  font-weight: 700;
  color: #ff6b6b;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 16rpx 0;
}

.label {
  font-size: 28rpx;
  color: #666;
}

.value {
  font-size: 28rpx;
  color: #333;
  font-weight: 500;
}

.card-title {
  font-size: 32rpx;
  font-weight: 600;
  color: #333;
  margin-bottom: 24rpx;
}

.stock-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20rpx;
}

.stock-card {
  padding: 24rpx;
  border-radius: 12rpx;
  text-align: center;
}

.stock-card.total {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stock-card.available {
  background: linear-gradient(135deg, #52c41a 0%, #73d13d 100%);
}

.stock-card.reserved {
  background: linear-gradient(135deg, #faad14 0%, #ffc53d 100%);
}

.stock-card.sold {
  background: linear-gradient(135deg, #ff4d4f 0%, #ff7875 100%);
}

.stock-label {
  font-size: 24rpx;
  color: rgba(255, 255, 255, 0.9);
  margin-bottom: 12rpx;
}

.stock-number {
  font-size: 40rpx;
  font-weight: 700;
  color: #fff;
}

.quantity-selector {
  display: flex;
  justify-content: center;
  padding: 32rpx 0;
}

.quantity-tip {
  text-align: center;
  font-size: 24rpx;
  color: #999;
  margin-top: 16rpx;
}

.action-buttons {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 20rpx;
  background-color: #fff;
  box-shadow: 0 -2rpx 8rpx rgba(0, 0, 0, 0.06);
}
</style>
