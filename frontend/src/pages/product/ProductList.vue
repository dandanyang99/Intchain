<template>
  <view class="product-list">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索产品名称"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 产品列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="product in productList" :key="product.id">
          <view class="product-item" @click="goToDetail(product.id)">
            <view class="product-info">
              <view class="product-name">{{ product.name }}</view>
              <view class="product-price">¥{{ product.unitPrice }}</view>
            </view>
            <view class="product-stock">
              <view class="stock-item">
                <text class="stock-label">总库存:</text>
                <text class="stock-value">{{ product.totalStock }}</text>
              </view>
              <view class="stock-item">
                <text class="stock-label">可用:</text>
                <text class="stock-value available">{{ product.availableStock }}</text>
              </view>
              <view class="stock-item">
                <text class="stock-label">预留:</text>
                <text class="stock-value reserved">{{ product.reservedStock }}</text>
              </view>
            </view>
            <view class="product-time">
              发布时间: {{ formatTime(product.createdAt) }}
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
      v-if="!loading && productList.length === 0"
      text="暂无产品"
      mode="list"
    ></u-empty>

    <!-- 发布产品按钮（仅彩票中心可见） -->
    <view v-if="userRole === 'LotteryCenter'" class="fab-button">
      <u-button
        type="primary"
        shape="circle"
        icon="plus"
        @click="goToPublish"
      ></u-button>
    </view>
  </view>
</template>

<script>
import { getProductList } from '@/api/product.js'

export default {
  data() {
    return {
      searchKeyword: '',
      productList: [],
      loading: false,
      noMore: false,
      page: 1,
      pageSize: 10,
      userRole: '', // 用户角色
    }
  },
  onLoad() {
    this.getUserRole()
    this.loadProductList()
  },
  methods: {
    // 获取用户角色
    getUserRole() {
      // 从本地存储或store获取用户角色
      const userInfo = uni.getStorageSync('userInfo')
      this.userRole = userInfo?.role || ''
    },

    // 加载产品列表
    async loadProductList(isRefresh = false) {
      if (this.loading) return

      this.loading = true

      try {
        const params = {
          page: isRefresh ? 1 : this.page,
          pageSize: this.pageSize,
          keyword: this.searchKeyword
        }

        const res = await getProductList(params)

        if (isRefresh) {
          this.productList = res.data || []
          this.page = 1
        } else {
          this.productList = [...this.productList, ...(res.data || [])]
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
      this.loadProductList()
    },

    // 搜索
    handleSearch() {
      this.page = 1
      this.noMore = false
      this.loadProductList(true)
    },

    // 清除搜索
    handleClear() {
      this.searchKeyword = ''
      this.handleSearch()
    },

    // 跳转到产品详情
    goToDetail(productId) {
      uni.navigateTo({
        url: `/pages/product/ProductDetail?id=${productId}`
      })
    },

    // 跳转到发布产品页面
    goToPublish() {
      uni.navigateTo({
        url: '/pages/product/ProductPublish'
      })
    },

    // 格式化时间
    formatTime(time) {
      if (!time) return ''
      const date = new Date(time)
      return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
    }
  }
}
</script>

<style scoped>
.product-list {
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

.product-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.product-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16rpx;
}

.product-name {
  font-size: 32rpx;
  font-weight: 600;
  color: #333;
}

.product-price {
  font-size: 36rpx;
  font-weight: 700;
  color: #ff6b6b;
}

.product-stock {
  display: flex;
  justify-content: space-between;
  padding: 16rpx 0;
  border-top: 1rpx solid #f0f0f0;
  border-bottom: 1rpx solid #f0f0f0;
  margin-bottom: 12rpx;
}

.stock-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stock-label {
  font-size: 24rpx;
  color: #999;
  margin-bottom: 8rpx;
}

.stock-value {
  font-size: 28rpx;
  font-weight: 600;
  color: #333;
}

.stock-value.available {
  color: #52c41a;
}

.stock-value.reserved {
  color: #faad14;
}

.product-time {
  font-size: 24rpx;
  color: #999;
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
</style>
