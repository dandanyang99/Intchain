<template>
  <view class="mini-program-management">
    <view class="page-header">
      <text class="page-title">小程序管理</text>
    </view>

    <!-- 配置列表 -->
    <view class="config-list">
      <view class="config-item">
        <view class="item-header">
          <text class="item-title">基本信息</text>
        </view>
        <view class="item-content">
          <view class="info-row">
            <text class="label">小程序名称:</text>
            <text class="value">彩票印刷物流管理系统</text>
          </view>
          <view class="info-row">
            <text class="label">版本号:</text>
            <text class="value">v1.0.0</text>
          </view>
          <view class="info-row">
            <text class="label">AppID:</text>
            <text class="value">wx1234567890abcdef</text>
          </view>
        </view>
      </view>

      <view class="config-item">
        <view class="item-header">
          <text class="item-title">系统配置</text>
        </view>
        <view class="item-content">
          <view class="config-row">
            <text class="label">维护模式</text>
            <u-switch v-model="config.maintenanceMode" @change="handleConfigChange"></u-switch>
          </view>
          <view class="config-row">
            <text class="label">允许注册</text>
            <u-switch v-model="config.allowRegister" @change="handleConfigChange"></u-switch>
          </view>
          <view class="config-row">
            <text class="label">调试模式</text>
            <u-switch v-model="config.debugMode" @change="handleConfigChange"></u-switch>
          </view>
        </view>
      </view>

      <view class="config-item">
        <view class="item-header">
          <text class="item-title">统计信息</text>
        </view>
        <view class="item-content">
          <view class="stat-row">
            <view class="stat-item">
              <text class="stat-value">{{ stats.totalUsers }}</text>
              <text class="stat-label">总用户数</text>
            </view>
            <view class="stat-item">
              <text class="stat-value">{{ stats.totalOrders }}</text>
              <text class="stat-label">总订单数</text>
            </view>
          </view>
          <view class="stat-row">
            <view class="stat-item">
              <text class="stat-value">{{ stats.activeUsers }}</text>
              <text class="stat-label">活跃用户</text>
            </view>
            <view class="stat-item">
              <text class="stat-value">{{ stats.todayOrders }}</text>
              <text class="stat-label">今日订单</text>
            </view>
          </view>
        </view>
      </view>
    </view>

    <!-- 操作按钮 -->
    <view class="action-section">
      <u-button type="primary" @click="handleRefresh" block>刷新数据</u-button>
      <u-button type="warning" @click="handleClearCache" block style="margin-top: 20rpx;">清除缓存</u-button>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      config: {
        maintenanceMode: false,
        allowRegister: true,
        debugMode: false
      },
      stats: {
        totalUsers: 0,
        totalOrders: 0,
        activeUsers: 0,
        todayOrders: 0
      }
    }
  },
  onLoad() {
    this.loadStats()
  },
  methods: {
    // 加载统计数据
    async loadStats() {
      try {
        // 模拟数据，实际应该从API获取
        this.stats = {
          totalUsers: 156,
          totalOrders: 342,
          activeUsers: 89,
          todayOrders: 23
        }
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      }
    },

    // 配置变更
    handleConfigChange() {
      uni.showToast({
        title: '配置已更新',
        icon: 'success'
      })
    },

    // 刷新数据
    handleRefresh() {
      this.loadStats()
      uni.showToast({
        title: '数据已刷新',
        icon: 'success'
      })
    },

    // 清除缓存
    handleClearCache() {
      uni.showModal({
        title: '提示',
        content: '确定要清除缓存吗？',
        success: (res) => {
          if (res.confirm) {
            uni.clearStorageSync()
            uni.showToast({
              title: '缓存已清除',
              icon: 'success'
            })
          }
        }
      })
    }
  }
}
</script>

<style scoped>
.mini-program-management {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 100rpx;
}

.page-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 60rpx 30rpx 40rpx;
  margin-bottom: 20rpx;
}

.page-title {
  font-size: 40rpx;
  font-weight: 600;
  color: #fff;
}

.config-list {
  padding: 20rpx;
}

.config-item {
  background-color: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 20rpx;
  box-shadow: 0 2rpx 8rpx rgba(0, 0, 0, 0.06);
}

.item-header {
  margin-bottom: 20rpx;
  padding-bottom: 16rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.item-title {
  font-size: 30rpx;
  font-weight: 600;
  color: #333;
}

.item-content {
  padding: 10rpx 0;
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

.config-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16rpx 0;
  border-bottom: 1rpx solid #f8f8f8;
}

.config-row:last-child {
  border-bottom: none;
}

.config-row .label {
  font-size: 28rpx;
  color: #333;
}

.stat-row {
  display: flex;
  justify-content: space-around;
  padding: 20rpx 0;
}

.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.stat-value {
  font-size: 48rpx;
  font-weight: 700;
  color: #2979ff;
  margin-bottom: 10rpx;
}

.stat-label {
  font-size: 24rpx;
  color: #999;
}

.action-section {
  padding: 20rpx 30rpx;
}
</style>
