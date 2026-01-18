<template>
  <view class="index-page">
    <!-- 用户信息卡片 -->
    <view class="user-card">
      <view class="user-info">
        <view class="avatar">
          <u-icon name="account-fill" size="60" color="#fff"></u-icon>
        </view>
        <view class="info">
          <text class="username">{{ userInfo.username || '未登录' }}</text>
          <text class="role">{{ getRoleText(userInfo.role) }}</text>
        </view>
      </view>
    </view>

    <!-- 功能菜单 -->
    <view class="menu-container">
      <view class="menu-title">功能菜单</view>
      <u-grid :col="3" :border="false">
        <u-grid-item
          v-for="menu in filteredMenus"
          :key="menu.id"
          @click="navigateTo(menu.path)"
        >
          <view class="menu-item">
            <u-icon :name="menu.icon" size="40" :color="menu.color"></u-icon>
            <text class="menu-text">{{ menu.name }}</text>
          </view>
        </u-grid-item>
      </u-grid>
    </view>

    <!-- 退出登录按钮 -->
    <view class="logout-button">
      <u-button type="error" @click="handleLogout" block>退出登录</u-button>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      userInfo: {},
      allMenus: [
        // 管理员菜单
        { id: 'user-mgmt', name: '用户管理', icon: 'account', color: '#2979ff', path: '/pages/user/UserManagement', roles: ['Admin', 'PrintingFactory', 'SalesOutlet'] },
        { id: 'mini-program', name: '小程序管理', icon: 'setting', color: '#19be6b', path: '/pages/admin/MiniProgramManagement', roles: ['Admin'] },

        // 印刷厂菜单
        { id: 'printing-orders', name: '印刷订单管理', icon: 'list', color: '#ff9900', path: '/pages/order/PrintingOrderList', roles: ['PrintingFactory', 'LotteryCenter'] },
        { id: 'lottery-center-mgmt', name: '彩票中心管理', icon: 'home', color: '#fa3534', path: '/pages/lottery/LotteryCenterManagement', roles: ['PrintingFactory'] },
        { id: 'inventory-mgmt', name: '库存管理', icon: 'grid', color: '#909399', path: '/pages/inventory/InventoryManagement', roles: ['PrintingFactory'] },

        // 彩票中心菜单
        { id: 'outlet-mgmt', name: '销售网点管理', icon: 'map', color: '#8B4513', path: '/pages/outlet/OutletManagement', roles: ['LotteryCenter'] },
        { id: 'product-mgmt', name: '产品管理', icon: 'tags', color: '#9c27b0', path: '/pages/product/ProductList', roles: ['LotteryCenter'] },

        // 销售网点菜单
        { id: 'view-products', name: '查看产品', icon: 'eye', color: '#00bcd4', path: '/pages/product/ProductList', roles: ['SalesOutlet'] },

        // 通用菜单
        { id: 'application-orders', name: '申请订单管理', icon: 'file-text', color: '#ff6b6b', path: '/pages/order/OrderList', roles: ['PrintingFactory', 'LotteryCenter', 'SalesOutlet'] },
        { id: 'approval', name: '订单审批', icon: 'checkmark-circle', color: '#52c41a', path: '/pages/approval/ApprovalPage', roles: ['LotteryCenter'] }
      ]
    }
  },
  onLoad() {
    this.getUserInfo()
  },
  computed: {
    filteredMenus() {
      if (!this.userInfo.role) return []
      return this.allMenus.filter(menu => menu.roles.includes(this.userInfo.role))
    }
  },
  methods: {
    getUserInfo() {
      const userInfo = uni.getStorageSync('userInfo')
      if (userInfo) {
        this.userInfo = userInfo
      } else {
        uni.showToast({
          title: '请先登录',
          icon: 'none'
        })
        setTimeout(() => {
          uni.redirectTo({
            url: '/pages/login/login'
          })
        }, 1500)
      }
    },

    getRoleText(role) {
      const roleMap = {
        'Admin': '管理员',
        'LotteryCenter': '彩票中心',
        'PrintingFactory': '印刷厂',
        'SalesOutlet': '销售网点'
      }
      return roleMap[role] || '未知角色'
    },

    navigateTo(path) {
      uni.navigateTo({
        url: path
      })
    },

    handleLogout() {
      uni.showModal({
        title: '提示',
        content: '确定要退出登录吗？',
        success: (res) => {
          if (res.confirm) {
            uni.removeStorageSync('userInfo')
            uni.removeStorageSync('token')
            uni.redirectTo({
              url: '/pages/login/login'
            })
          }
        }
      })
    }
  }
}
</script>

<style scoped>
.index-page {
  min-height: 100vh;
  background-color: #f5f5f5;
  padding-bottom: 40rpx;
}

.user-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 40rpx 30rpx;
  margin-bottom: 20rpx;
}

.user-info {
  display: flex;
  align-items: center;
}

.avatar {
  width: 120rpx;
  height: 120rpx;
  border-radius: 60rpx;
  background-color: rgba(255, 255, 255, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 30rpx;
}

.info {
  display: flex;
  flex-direction: column;
}

.username {
  font-size: 36rpx;
  font-weight: 600;
  color: #fff;
  margin-bottom: 10rpx;
}

.role {
  font-size: 26rpx;
  color: rgba(255, 255, 255, 0.9);
  padding: 6rpx 16rpx;
  background-color: rgba(255, 255, 255, 0.2);
  border-radius: 20rpx;
  align-self: flex-start;
}

.menu-container {
  background-color: #fff;
  border-radius: 16rpx;
  margin: 20rpx;
  padding: 30rpx 20rpx;
}

.menu-title {
  font-size: 32rpx;
  font-weight: 600;
  color: #333;
  margin-bottom: 30rpx;
  padding-left: 10rpx;
}

.menu-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 30rpx 0;
}

.menu-text {
  font-size: 26rpx;
  color: #666;
  margin-top: 16rpx;
  text-align: center;
}

.logout-button {
  margin: 40rpx 30rpx 0;
}
</style>
