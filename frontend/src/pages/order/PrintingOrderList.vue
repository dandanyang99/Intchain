<template>
  <view class="printing-order-list">
    <!-- 状态筛选标签 -->
    <view class="status-tabs">
      <scroll-view scroll-x class="tabs-scroll">
        <view
          v-for="tab in statusTabs"
          :key="tab.value"
          class="tab-item"
          :class="{ active: currentStatus === tab.value }"
          @click="changeStatus(tab.value)"
        >
          {{ tab.label }}
        </view>
      </scroll-view>
    </view>

    <!-- 搜索栏 -->
    <view class="search-bar">
      <u-search
        v-model="searchKeyword"
        placeholder="搜索订单号"
        @search="handleSearch"
        @clear="handleClear"
      ></u-search>
    </view>

    <!-- 订单列表 -->
    <view class="list-container">
      <u-list @scrolltolower="loadMore">
        <u-list-item v-for="order in orderList" :key="order.id">
          <view class="order-item" @click="goToDetail(order.id)">
            <!-- 订单头部 -->
            <view class="order-header">
              <text class="order-number">{{ order.orderNumber }}</text>
              <view class="status-badge" :class="getStatusClass(order.status)">
                {{ getStatusText(order.status) }}
              </view>
            </view>

            <!-- 订单信息 -->
            <view class="order-info">
              <view class="info-row">
                <text class="label">申请订单ID:</text>
                <text class="value">{{ order.applicationOrderId }}</text>
              </view>
              <view class="info-row">
                <text class="label">产品ID:</text>
                <text class="value">{{ order.productId }}</text>
              </view>
              <view class="info-row">
                <text class="label">数量:</text>
                <text class="value">{{ order.quantity }}</text>
              </view>
              <view class="info-row" v-if="order.remarks">
                <text class="label">备注:</text>
                <text class="value">{{ order.remarks }}</text>
              </view>
            </view>

            <!-- 订单时间 -->
            <view class="order-footer">
              <text class="time">{{ formatTime(order.createdAt) }}</text>
              <u-icon name="arrow-right" size="16"></u-icon>
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
      text="暂无印刷订单"
      mode="list"
    ></u-empty>
  </view>
</template>

<script>
import { getPrintingOrderList, getPrintingOrdersByStatus } from '@/api/order.js'

export default {
  data() {
    return {
      searchKeyword: '',
      orderList: [],
      loading: false,
      noMore: false,
      page: 1,
      pageSize: 10,
      userRole: '',
      currentStatus: 'all',
      statusTabs: [
        { label: '全部', value: 'all' },
        { label: '待接单', value: 'Pending' },
        { label: '生产中', value: 'InProduction' },
        { label: '待发货', value: 'WaitingShipment' },
        { label: '已发货', value: 'Shipped' },
        { label: '已完成', value: 'Completed' }
      ]
    }
  },
  onLoad() {
    this.getUserRole()
    this.loadOrderList()
  },
  methods: {
    // 获取用户角色
    getUserRole() {
      const userInfo = uni.getStorageSync('userInfo')
      this.userRole = userInfo?.role || ''
    },

    // 切换状态
    changeStatus(status) {
      this.currentStatus = status
      this.page = 1
      this.noMore = false
      this.loadOrderList(true)
    },

    // 加载订单列表
    async loadOrderList(isRefresh = false) {
      if (this.loading) return

      this.loading = true

      try {
        let res
        if (this.currentStatus === 'all') {
          const params = {
            page: isRefresh ? 1 : this.page,
            pageSize: this.pageSize,
            keyword: this.searchKeyword
          }
          res = await getPrintingOrderList(params)
        } else {
          res = await getPrintingOrdersByStatus(this.currentStatus)
        }

        if (isRefresh) {
          this.orderList = res.data || []
          this.page = 1
        } else {
          this.orderList = [...this.orderList, ...(res.data || [])]
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
      if (this.noMore || this.loading || this.currentStatus !== 'all') return
      this.page++
      this.loadOrderList()
    },

    // 搜索
    handleSearch() {
      this.page = 1
      this.noMore = false
      this.loadOrderList(true)
    },

    // 清除搜索
    handleClear() {
      this.searchKeyword = ''
      this.handleSearch()
    },

    // 跳转到订单详情
    goToDetail(orderId) {
      uni.navigateTo({
        url: `/pages/order/PrintingOrderDetail?id=${orderId}`
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
}
</script>

<style scoped>
.printing-order-list {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 100rpx;
}

.status-tabs {
  background-color: #fff;
  padding: 20rpx 0;
  border-bottom: 1rpx solid #f0f0f0;
}

.tabs-scroll {
  white-space: nowrap;
}

.tab-item {
  display: inline-block;
  padding: 10rpx 30rpx;
  margin: 0 10rpx;
  font-size: 28rpx;
  color: #666;
  border-radius: 40rpx;
}

.tab-item.active {
  background-color: #ff9900;
  color: #fff;
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

.order-info {
  margin-bottom: 16rpx;
}

.info-row {
  display: flex;
  padding: 8rpx 0;
}

.info-row .label {
  font-size: 26rpx;
  color: #999;
  width: 180rpx;
}

.info-row .value {
  flex: 1;
  font-size: 26rpx;
  color: #333;
}

.order-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-top: 12rpx;
  border-top: 1rpx solid #f0f0f0;
}

.order-footer .time {
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
</style>
