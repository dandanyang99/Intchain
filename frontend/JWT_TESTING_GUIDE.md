# JWT Token Verification Testing Guide

## 测试环境准备

### 1. 启动后端服务
确保以下服务正在运行：
- **Gateway**: http://localhost:5199
- **UserService**: http://localhost:5153

### 2. 安装前端依赖
```bash
cd e:\work\Intchain\frontend
npm install
```

### 3. 启动前端开发服务器
```bash
# 微信小程序
npm run dev:mp-weixin

# H5 (浏览器)
npm run dev:h5
```

## 测试场景

### 场景 1: 用户注册流程
1. 打开应用，进入注册页面
2. 填写用户信息（用户名、密码、角色等）
3. 点击注册按钮
4. **预期结果**:
   - 注册成功后自动登录
   - 收到 JWT token
   - Token 被保存到本地存储
   - 用户信息被保存到本地存储

### 场景 2: 用户登录流程
1. 打开应用，进入登录页面
2. 填写用户名和密码
3. 点击登录按钮
4. **预期结果**:
   - 登录成功
   - 收到 JWT token
   - Token 被保存到本地存储
   - 自动跳转到首页

### 场景 3: Token 自动添加到请求头
1. 登录成功后
2. 访问需要认证的接口（如获取用户信息）
3. **预期结果**:
   - 请求头自动包含 `Authorization: Bearer <token>`
   - 请求成功返回数据

### 场景 4: Token 过期处理
1. 登录成功后
2. 手动修改本地存储中的 token，将 exp 时间改为过去的时间
3. 尝试访问需要认证的接口
4. **预期结果**:
   - 请求被拦截，不会发送到服务器
   - 显示"登录已过期，请重新登录"提示
   - Token 和用户信息被清除
   - 自动跳转到登录页

### 场景 5: Token 格式无效处理
1. 登录成功后
2. 手动修改本地存储中的 token 为无效格式（如 "invalid_token"）
3. 尝试访问需要认证的接口
4. **预期结果**:
   - 请求被拦截
   - 显示"登录已过期，请重新登录"提示
   - Token 和用户信息被清除
   - 自动跳转到登录页

### 场景 6: 认证接口不需要 Token
1. 未登录状态
2. 调用注册或登录接口
3. **预期结果**:
   - 请求正常发送，不会被拦截
   - 不会添加 Authorization 头
   - 接口正常返回结果

### 场景 7: 服务器返回 401 错误
1. 登录成功后
2. 服务器端 token 验证失败（如 token 被篡改）
3. **预期结果**:
   - 响应拦截器捕获 401 错误
   - 显示"登录已过期，请重新登录"提示
   - Token 和用户信息被清除
   - 自动跳转到登录页

## 调试技巧

### 1. 查看请求日志
在开发环境下，所有请求和响应都会打印到控制台：
```
【请求】 http://localhost:5199/api/auth/register {...}
【响应】 http://localhost:5199/api/auth/register {...}
```

### 2. 查看本地存储
在浏览器开发者工具或微信开发者工具中查看本地存储：
- `intchain_token`: JWT token
- `intchain_user_info`: 用户信息（JSON 字符串）

### 3. 解码 Token
使用 `decodeToken()` 函数查看 token 内容：
```javascript
import { decodeToken } from '@/utils/auth.js'

const token = uni.getStorageSync('intchain_token')
const payload = decodeToken(token)
console.log('Token payload:', payload)
// 输出: { userId: 1, username: "test", role: "SalesOutlet", exp: 1234567890, ... }
```

### 4. 检查 Token 有效性
```javascript
import { isTokenValid, isTokenExpired } from '@/utils/auth.js'

const token = uni.getStorageSync('intchain_token')
console.log('Token valid:', isTokenValid(token))
console.log('Token expired:', isTokenExpired(token))
```

## 常见问题

### Q1: Token 无法保存到本地存储
**解决方案**: 检查是否调用了 `setToken()` 函数，确保在登录/注册成功后保存 token。

### Q2: 请求没有携带 Token
**解决方案**:
1. 检查 token 是否已保存到本地存储
2. 检查请求 URL 是否包含 `/api/auth/`（这些接口不会添加 token）
3. 检查 token 是否有效（未过期、格式正确）

### Q3: Token 过期时间不正确
**解决方案**: 检查后端 JWT 配置中的 `ExpirationMinutes` 设置，确保与前端预期一致。

### Q4: 跳转到登录页失败
**解决方案**: 确保登录页路径为 `/pages/login/login`，如果路径不同，需要修改 `request.js` 中的跳转路径。

## 性能优化建议

1. **Token 刷新机制**: 当 token 即将过期时（如剩余 5 分钟），自动刷新 token
2. **请求队列**: 当 token 过期时，暂停所有请求，刷新 token 后重新发送
3. **缓存策略**: 对于不常变化的数据，使用本地缓存减少请求次数

## 安全建议

1. **HTTPS**: 生产环境必须使用 HTTPS 传输 token
2. **Token 存储**: 考虑使用加密存储 token
3. **敏感操作**: 对于敏感操作（如修改密码），要求重新验证身份
4. **Token 过期时间**: 根据业务需求设置合理的过期时间（当前为 1440 分钟 = 24 小时）
