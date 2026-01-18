<template>
  <view class="product-publish">
    <view class="form-container">
      <u-form :model="formData" ref="form" label-width="160">
        <!-- 产品名称 -->
        <u-form-item label="产品名称" prop="name" required>
          <u-input
            v-model="formData.name"
            placeholder="请输入产品名称"
            maxlength="100"
          ></u-input>
        </u-form-item>

        <!-- 单价 -->
        <u-form-item label="单价（元）" prop="unitPrice" required>
          <u-input
            v-model="formData.unitPrice"
            type="number"
            placeholder="请输入单价"
          ></u-input>
        </u-form-item>

        <!-- 总库存 -->
        <u-form-item label="总库存" prop="totalStock" required>
          <u-input
            v-model="formData.totalStock"
            type="number"
            placeholder="请输入总库存数量"
          ></u-input>
        </u-form-item>

        <!-- 印刷厂选择 -->
        <u-form-item label="印刷厂" prop="printingFactoryId" required>
          <u-input
            v-model="printingFactoryName"
            placeholder="请选择印刷厂"
            readonly
            @click="showFactoryPicker = true"
          ></u-input>
        </u-form-item>

        <!-- 提示信息 -->
        <view class="form-tip">
          <u-icon name="info-circle" color="#faad14" size="32"></u-icon>
          <text class="tip-text">发布产品后将自动创建印刷订单发送给印刷厂</text>
        </view>
      </u-form>

      <!-- 操作按钮 -->
      <view class="action-buttons">
        <u-button
          type="primary"
          size="large"
          :loading="submitting"
          @click="handleSubmit"
        >
          {{ isEditMode ? '保存修改' : '发布产品' }}
        </u-button>

        <u-button
          size="large"
          :disabled="submitting"
          @click="handleCancel"
        >
          取消
        </u-button>
      </view>
    </view>

    <!-- 印刷厂选择器 -->
    <u-picker
      :show="showFactoryPicker"
      :columns="factoryColumns"
      @confirm="onFactoryConfirm"
      @cancel="showFactoryPicker = false"
    ></u-picker>
  </view>
</template>

<script>
import { createProduct, updateProduct, getProductDetail } from '@/api/product.js'

export default {
  data() {
    return {
      isEditMode: false,
      productId: null,
      formData: {
        name: '',
        unitPrice: '',
        totalStock: '',
        printingFactoryId: '',
        lotteryCenterId: ''
      },
      printingFactoryName: '',
      showFactoryPicker: false,
      factoryColumns: [],
      factoryList: [],
      submitting: false
    }
  },
  onLoad(options) {
    // 判断是新建还是编辑模式
    if (options.id && options.mode === 'edit') {
      this.isEditMode = true
      this.productId = options.id
      this.loadProductDetail()
    }

    // 获取用户信息（彩票中心ID）
    this.getUserInfo()

    // 加载印刷厂列表
    this.loadFactoryList()
  },
  methods: {
    // 获取用户信息
    getUserInfo() {
      const userInfo = uni.getStorageSync('userInfo')
      this.formData.lotteryCenterId = userInfo?.lotteryCenterId || ''
    },

    // 加载产品详情（编辑模式）
    async loadProductDetail() {
      try {
        const res = await getProductDetail(this.productId)
        const product = res.data

        this.formData = {
          name: product.name,
          unitPrice: String(product.unitPrice),
          totalStock: String(product.totalStock),
          printingFactoryId: '',
          lotteryCenterId: product.lotteryCenterId
        }
      } catch (error) {
        uni.showToast({
          title: error.message || '加载失败',
          icon: 'none'
        })
      }
    },

    // 加载印刷厂列表
    async loadFactoryList() {
      try {
        // TODO: 调用印刷厂列表API
        // 这里暂时使用模拟数据
        this.factoryList = [
          { id: 1, name: '印刷厂A' },
          { id: 2, name: '印刷厂B' },
          { id: 3, name: '印刷厂C' }
        ]

        // 转换为picker需要的格式
        this.factoryColumns = [
          this.factoryList.map(item => item.name)
        ]
      } catch (error) {
        console.error('加载印刷厂列表失败:', error)
      }
    },

    // 印刷厂选择确认
    onFactoryConfirm(e) {
      const index = e.value[0]
      const factory = this.factoryList[index]

      this.formData.printingFactoryId = factory.id
      this.printingFactoryName = factory.name
      this.showFactoryPicker = false
    },

    // 表单验证
    validateForm() {
      if (!this.formData.name) {
        uni.showToast({
          title: '请输入产品名称',
          icon: 'none'
        })
        return false
      }

      if (!this.formData.unitPrice || Number(this.formData.unitPrice) <= 0) {
        uni.showToast({
          title: '请输入有效的单价',
          icon: 'none'
        })
        return false
      }

      if (!this.formData.totalStock || Number(this.formData.totalStock) <= 0) {
        uni.showToast({
          title: '请输入有效的库存数量',
          icon: 'none'
        })
        return false
      }

      if (!this.formData.printingFactoryId) {
        uni.showToast({
          title: '请选择印刷厂',
          icon: 'none'
        })
        return false
      }

      return true
    },

    // 提交表单
    async handleSubmit() {
      if (!this.validateForm()) return

      this.submitting = true

      try {
        const data = {
          name: this.formData.name,
          unitPrice: Number(this.formData.unitPrice),
          totalStock: Number(this.formData.totalStock),
          printingFactoryId: this.formData.printingFactoryId,
          lotteryCenterId: this.formData.lotteryCenterId
        }

        if (this.isEditMode) {
          await updateProduct(this.productId, data)
          uni.showToast({
            title: '修改成功',
            icon: 'success'
          })
        } else {
          await createProduct(data)
          uni.showToast({
            title: '发布成功',
            icon: 'success'
          })
        }

        // 延迟返回上一页
        setTimeout(() => {
          uni.navigateBack()
        }, 1500)
      } catch (error) {
        uni.showToast({
          title: error.message || '操作失败',
          icon: 'none'
        })
      } finally {
        this.submitting = false
      }
    },

    // 取消
    handleCancel() {
      uni.navigateBack()
    }
  }
}
</script>

<style scoped>
.product-publish {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.form-container {
  padding: 20rpx;
}

.form-tip {
  display: flex;
  align-items: center;
  padding: 24rpx;
  margin: 20rpx 0;
  background-color: #fffbe6;
  border-radius: 8rpx;
  border: 1rpx solid #ffe58f;
}

.tip-text {
  margin-left: 12rpx;
  font-size: 28rpx;
  color: #faad14;
  flex: 1;
}

.action-buttons {
  margin-top: 60rpx;
}

.action-buttons .u-button {
  margin-bottom: 20rpx;
}
</style>
