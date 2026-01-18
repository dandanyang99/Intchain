<template>
  <view class="order-apply">
    <u-form :model="form" ref="formRef" label-width="120">
      <!-- 产品选择 -->
      <u-form-item label="选择产品" prop="productId" required>
        <view class="product-selector" @click="showProductPicker = true">
          <text v-if="selectedProduct" class="selected-text">{{ selectedProduct.name }}</text>
          <text v-else class="placeholder-text">请选择产品</text>
          <u-icon name="arrow-right"></u-icon>
        </view>
      </u-form-item>

      <!-- 产品信息展示 -->
      <view v-if="selectedProduct" class="product-info">
        <view class="info-item">
          <text class="label">单价:</text>
          <text class="value">¥{{ selectedProduct.unitPrice }}</text>
        </view>
        <view class="info-item">
          <text class="label">可用库存:</text>
          <text class="value">{{ selectedProduct.availableStock }}</text>
        </view>
      </view>

      <!-- 数量输入 -->
      <u-form-item label="申请数量" prop="quantity" required>
        <u-number-box
          v-model="form.quantity"
          :min="1"
          :max="selectedProduct ? selectedProduct.availableStock : 999999"
        ></u-number-box>
      </u-form-item>

      <!-- 总金额 -->
      <view v-if="selectedProduct && form.quantity" class="total-amount">
        <text class="label">预计金额:</text>
        <text class="amount">¥{{ totalAmount }}</text>
      </view>

      <!-- 备注 -->
      <u-form-item label="备注" prop="remarks">
        <u-textarea
          v-model="form.remarks"
          placeholder="请输入备注信息（可选）"
          :maxlength="200"
          count
        ></u-textarea>
      </u-form-item>
    </u-form>

    <!-- 提交按钮 -->
    <view class="submit-button">
      <u-button
        type="primary"
        :loading="submitting"
        @click="handleSubmit"
        block
      >提交申请</u-button>
    </view>

    <!-- 产品选择器 -->
    <u-popup v-model="showProductPicker" mode="bottom" :round="10">
      <view class="product-picker">
        <view class="picker-header">
          <text class="cancel" @click="showProductPicker = false">取消</text>
          <text class="title">选择产品</text>
          <text class="confirm" @click="confirmProduct">确定</text>
        </view>
        <view class="product-list">
          <view
            v-for="product in productList"
            :key="product.id"
            class="product-item"
            :class="{ active: tempProductId === product.id }"
            @click="tempProductId = product.id"
          >
            <view class="product-name">{{ product.name }}</view>
            <view class="product-detail">
              <text class="price">¥{{ product.unitPrice }}</text>
              <text class="stock">库存: {{ product.availableStock }}</text>
            </view>
          </view>
        </view>
      </view>
    </u-popup>
  </view>
</template>

<script>
import { getProductList } from '@/api/product.js'
import { createApplicationOrder } from '@/api/order.js'

export default {
  data() {
    return {
      form: {
        productId: null,
        quantity: 1,
        remarks: ''
      },
      selectedProduct: null,
      productList: [],
      showProductPicker: false,
      tempProductId: null,
      submitting: false
    }
  },
  computed: {
    totalAmount() {
      if (!this.selectedProduct || !this.form.quantity) return 0
      return (this.selectedProduct.unitPrice * this.form.quantity).toFixed(2)
    }
  },
  onLoad() {
    this.loadProducts()
  },
  methods: {
    // 加载产品列表
    async loadProducts() {
      try {
        const res = await getProductList({ pageSize: 100 })
        this.productList = res.data || []
      } catch (error) {
        uni.showToast({
          title: error.message || '加载产品失败',
          icon: 'none'
        })
      }
    },

    // 确认选择产品
    confirmProduct() {
      if (!this.tempProductId) {
        uni.showToast({
          title: '请选择产品',
          icon: 'none'
        })
        return
      }

      this.form.productId = this.tempProductId
      this.selectedProduct = this.productList.find(p => p.id === this.tempProductId)
      this.showProductPicker = false

      // 重置数量
      this.form.quantity = 1
    },

    // 提交申请
    async handleSubmit() {
      if (!this.form.productId) {
        uni.showToast({
          title: '请选择产品',
          icon: 'none'
        })
        return
      }

      if (!this.form.quantity || this.form.quantity < 1) {
        uni.showToast({
          title: '请输入有效的数量',
          icon: 'none'
        })
        return
      }

      if (this.selectedProduct && this.form.quantity > this.selectedProduct.availableStock) {
        uni.showToast({
          title: '申请数量超过可用库存',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        const userInfo = uni.getStorageSync('userInfo')

        const data = {
          salesOutletId: userInfo.salesOutletId,
          lotteryCenterId: userInfo.lotteryCenterId,
          productId: this.form.productId,
          quantity: this.form.quantity,
          remarks: this.form.remarks
        }

        await createApplicationOrder(data)

        uni.showToast({
          title: '申请提交成功',
          icon: 'success'
        })

        setTimeout(() => {
          uni.navigateBack()
        }, 1500)
      } catch (error) {
        uni.showToast({
          title: error.message || '提交失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    }
  }
}
</script>

<style scoped>
.order-apply {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding: 20rpx;
}

.product-selector {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20rpx;
  background-color: #fff;
  border-radius: 8rpx;
  border: 1rpx solid #e5e5e5;
}

.selected-text {
  font-size: 28rpx;
  color: #333;
}

.placeholder-text {
  font-size: 28rpx;
  color: #999;
}

.product-info {
  background-color: #fff;
  border-radius: 8rpx;
  padding: 20rpx;
  margin: 20rpx 0;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 10rpx 0;
}

.info-item .label {
  font-size: 28rpx;
  color: #666;
}

.info-item .value {
  font-size: 28rpx;
  color: #333;
  font-weight: 600;
}

.total-amount {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx 20rpx;
  background-color: #fff;
  border-radius: 8rpx;
  margin: 20rpx 0;
}

.total-amount .label {
  font-size: 32rpx;
  color: #333;
}

.total-amount .amount {
  font-size: 40rpx;
  color: #ff6b6b;
  font-weight: 700;
}

.submit-button {
  margin-top: 60rpx;
  padding: 0 20rpx;
}

.product-picker {
  background-color: #fff;
  border-radius: 20rpx 20rpx 0 0;
  max-height: 80vh;
}

.picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.picker-header .cancel,
.picker-header .confirm {
  font-size: 28rpx;
  color: #666;
}

.picker-header .confirm {
  color: #2979ff;
}

.picker-header .title {
  font-size: 32rpx;
  font-weight: 600;
  color: #333;
}

.product-list {
  max-height: 60vh;
  overflow-y: auto;
}

.product-item {
  padding: 30rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.product-item.active {
  background-color: #f0f7ff;
}

.product-name {
  font-size: 30rpx;
  color: #333;
  margin-bottom: 10rpx;
}

.product-detail {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.product-detail .price {
  font-size: 32rpx;
  color: #ff6b6b;
  font-weight: 600;
}

.product-detail .stock {
  font-size: 24rpx;
  color: #999;
}
</style>
