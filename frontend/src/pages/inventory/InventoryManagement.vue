<template>
  <view class="inventory-management">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索产品名称"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 库存列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="item in inventoryList" :key="item.id">
          <view class="inventory-item">
            <view class="item-header">
              <text class="product-name">{{ item.productName }}</text>
              <view class="stock-badge" :class="getStockClass(item.availableStock)">
                库存: {{ item.availableStock }}
              </view>
            </view>
            <view class="item-info">
              <view class="info-row">
                <text class="label">产品ID:</text>
                <text class="value">{{ item.productId }}</text>
              </view>
              <view class="info-row">
                <text class="label">总库存:</text>
                <text class="value">{{ item.totalStock }}</text>
              </view>
              <view class="info-row">
                <text class="label">已分配:</text>
                <text class="value">{{ item.allocatedStock }}</text>
              </view>
            </view>
            <view class="action-buttons">
              <u-button type="success" size="small" @click="handleStockIn(item)">入库</u-button>
              <u-button type="warning" size="small" @click="handleStockOut(item)">出库</u-button>
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
      v-if="!loading && inventoryList.length === 0"
      text="暂无库存数据"
      mode="list"
    ></u-empty>

    <!-- 入库对话框 -->
    <u-popup v-model="showStockInDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">库存入库</view>
        <view class="product-info">
          <text class="product-name">{{ currentItem?.productName }}</text>
        </view>
        <u-form :model="stockForm">
          <u-form-item label="入库数量" required>
            <u-number-box v-model="stockForm.quantity" :min="1"></u-number-box>
          </u-form-item>
          <u-form-item label="备注">
            <u-textarea v-model="stockForm.remarks" placeholder="请输入备注（可选）"></u-textarea>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showStockInDialog = false">取消</u-button>
          <u-button type="success" @click="confirmStockIn" :loading="submitting">确定</u-button>
        </view>
      </view>
    </u-popup>

    <!-- 出库对话框 -->
    <u-popup v-model="showStockOutDialog" mode="center" :round="10">
      <view class="dialog-content">
        <view class="dialog-title">库存出库</view>
        <view class="product-info">
          <text class="product-name">{{ currentItem?.productName }}</text>
          <text class="available-stock">可用库存: {{ currentItem?.availableStock }}</text>
        </view>
        <u-form :model="stockForm">
          <u-form-item label="出库数量" required>
            <u-number-box
              v-model="stockForm.quantity"
              :min="1"
              :max="currentItem?.availableStock || 999"
            ></u-number-box>
          </u-form-item>
          <u-form-item label="备注">
            <u-textarea v-model="stockForm.remarks" placeholder="请输入备注（可选）"></u-textarea>
          </u-form-item>
        </u-form>
        <view class="dialog-buttons">
          <u-button @click="showStockOutDialog = false">取消</u-button>
          <u-button type="warning" @click="confirmStockOut" :loading="submitting">确定</u-button>
        </view>
      </view>
    </u-popup>
  </view>
</template>

<script>
import { getInventoryList, inventoryIn, inventoryOut } from '@/api/inventory.js'

export default {
  data() {
    return {
      searchKeyword: '',
      inventoryList: [],
      loading: false,
      noMore: false,
      page: 1,
      pageSize: 10,
      showStockInDialog: false,
      showStockOutDialog: false,
      submitting: false,
      currentItem: null,
      stockForm: {
        quantity: 1,
        remarks: ''
      }
    }
  },
  onLoad() {
    this.loadInventoryList()
  },
  methods: {
    // 加载库存列表
    async loadInventoryList(isRefresh = false) {
      if (this.loading) return

      this.loading = true

      try {
        const params = {
          page: isRefresh ? 1 : this.page,
          pageSize: this.pageSize,
          keyword: this.searchKeyword
        }
        const res = await getInventoryList(params)

        if (isRefresh) {
          this.inventoryList = res.data || []
          this.page = 1
        } else {
          this.inventoryList = [...this.inventoryList, ...(res.data || [])]
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
      this.loadInventoryList()
    },

    // 搜索
    handleSearch() {
      this.page = 1
      this.noMore = false
      this.loadInventoryList(true)
    },

    // 清除搜索
    handleClear() {
      this.searchKeyword = ''
      this.handleSearch()
    },

    // 入库
    handleStockIn(item) {
      this.currentItem = item
      this.stockForm = {
        quantity: 1,
        remarks: ''
      }
      this.showStockInDialog = true
    },

    // 出库
    handleStockOut(item) {
      this.currentItem = item
      this.stockForm = {
        quantity: 1,
        remarks: ''
      }
      this.showStockOutDialog = true
    },

    // 确认入库
    async confirmStockIn() {
      if (!this.stockForm.quantity || this.stockForm.quantity < 1) {
        uni.showToast({
          title: '请输入有效的入库数量',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        await inventoryIn({
          productId: this.currentItem.productId,
          quantity: this.stockForm.quantity,
          remarks: this.stockForm.remarks
        })

        uni.showToast({
          title: '入库成功',
          icon: 'success'
        })

        this.showStockInDialog = false
        this.loadInventoryList(true)
      } catch (error) {
        uni.showToast({
          title: error.message || '入库失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    // 确认出库
    async confirmStockOut() {
      if (!this.stockForm.quantity || this.stockForm.quantity < 1) {
        uni.showToast({
          title: '请输入有效的出库数量',
          icon: 'none'
        })
        return
      }

      if (this.stockForm.quantity > this.currentItem.availableStock) {
        uni.showToast({
          title: '出库数量不能超过可用库存',
          icon: 'none'
        })
        return
      }

      this.submitting = true

      try {
        await inventoryOut({
          productId: this.currentItem.productId,
          quantity: this.stockForm.quantity,
          remarks: this.stockForm.remarks
        })

        uni.showToast({
          title: '出库成功',
          icon: 'success'
        })

        this.showStockOutDialog = false
        this.loadInventoryList(true)
      } catch (error) {
        uni.showToast({
          title: error.message || '出库失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    // 获取库存样式类
    getStockClass(stock) {
      if (stock <= 0) {
        return 'stock-empty'
      } else if (stock < 100) {
        return 'stock-low'
      } else if (stock < 500) {
        return 'stock-medium'
      } else {
        return 'stock-high'
      }
    }
  }
}
</script>

<style scoped>
.inventory-management {
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

.inventory-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.item-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.product-name {
  font-size: 30rpx;
  font-weight: 600;
  color: #333;
}

.stock-badge {
  padding: 6rpx 16rpx;
  border-radius: 20rpx;
  font-size: 24rpx;
  font-weight: 600;
}

.stock-empty {
  background-color: #fff1f0;
  color: #ff4d4f;
}

.stock-low {
  background-color: #fff7e6;
  color: #faad14;
}

.stock-medium {
  background-color: #e6f7ff;
  color: #1890ff;
}

.stock-high {
  background-color: #f6ffed;
  color: #52c41a;
}

.item-info {
  margin-bottom: 20rpx;
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

.product-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 30rpx;
  padding: 20rpx;
  background-color: #f5f5f5;
  border-radius: 8rpx;
}

.product-info .product-name {
  font-size: 28rpx;
  color: #333;
  margin-bottom: 10rpx;
}

.product-info .available-stock {
  font-size: 24rpx;
  color: #666;
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
