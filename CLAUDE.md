# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

**Intchain** 是一个彩票印刷物流管理系统，通过微信小程序实现彩票中心、销售网点、印刷厂三方的业务协同。

### 核心业务流程

彩票中心发布产品 → 销售网点申请 → 彩票中心审批 → 印刷厂印刷 → 发货配送 → 物流跟踪 → 确认收货

### 用户角色

- **管理员（Admin）**：系统配置、添加彩票中心和印刷厂
- **彩票中心（LotteryCenter）**：发布产品、审批订单、管理销售网点
- **印刷厂（PrintingFactory）**：接收印刷订单、发货、物流管理
- **销售网点（SalesOutlet）**：申请彩票、查看订单、确认收货

## 技术栈

### 前端
- **框架**：uni-app 3.x（基于 Vue.js）
- **UI组件库**：uView UI 2.x
- **状态管理**：Pinia（推荐）或 Vuex
- **HTTP请求**：uni.request 封装
- **开发工具**：HBuilderX

### API网关
- **框架**：Ocelot（.NET Core API Gateway）
- **功能**：路由转发、负载均衡、认证授权、限流熔断
- **协议**：HTTP/HTTPS、WebSocket

### 后端服务
- **框架**：ASP.NET Core 8.0
- **ORM**：Entity Framework Core 8.0（Code First）
- **认证授权**：JWT + ASP.NET Core Identity
- **API文档**：Swagger/OpenAPI
- **日志**：Serilog
- **缓存**：Redis 7.x

### 数据库
- **主数据库**：MySQL 8.0
- **缓存**：Redis 7.x（库存、会话管理、分布式锁）
- **对象存储**：阿里云OSS 或 腾讯云COS

## 项目架构

### 微服务架构 + API网关

```
┌─────────────────────────────────────────────────────────────┐
│                      前端层（uni-app 3.x）                    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │销售网点端│  │彩票中心端│  │ 印刷厂端 │  │ 管理员端 │   │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↓ HTTPS
┌─────────────────────────────────────────────────────────────┐
│                   API网关层（Ocelot Gateway）                 │
│  ┌────────────────────────────────────────────────────────┐ │
│  │ • 路由转发  • 负载均衡  • 认证授权  • 限流熔断        │ │
│  │ • 日志记录  • 请求聚合  • 服务发现  • 缓存策略        │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              后端服务层（ASP.NET Core 8.0 微服务）            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  用户服务    │  │  订单服务    │  │  库存服务    │     │
│  │ UserService  │  │ OrderService │  │InventoryServ │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  审批服务    │  │  物流服务    │  │  通知服务    │     │
│  │ApprovalServ  │  │LogisticsServ │  │NotificationS │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    数据访问层（EF Core 8.0）                  │
│  ┌────────────────────────────────────────────────────────┐ │
│  │ • DbContext  • Repository模式  • UnitOfWork           │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                        数据存储层                             │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐                 │
│  │ MySQL 8.0│  │ Redis 7.x│  │ 阿里云OSS│                 │
│  └──────────┘  └──────────┘  └──────────┘                 │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                      基础设施层                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │  微信API     │  │  物流API     │  │  短信服务    │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
└─────────────────────────────────────────────────────────────┘
```

### 服务划分说明

**用户服务（UserService）**：
- 用户认证和授权
- 角色权限管理
- 用户信息管理

**订单服务（OrderService）**：
- 申请订单管理
- 印刷订单管理
- 订单状态流转

**库存服务（InventoryService）**：
- 彩票产品管理
- 库存扣减和释放
- 库存查询和统计

**审批服务（ApprovalService）**：
- 审批流程管理
- 审批记录查询
- 审批通知

**物流服务（LogisticsService）**：
- 物流信息管理
- 第三方物流对接
- 物流状态更新

**通知服务（NotificationService）**：
- 微信模板消息推送
- 短信通知
- 站内消息

## 核心领域概念

### 核心实体（Entities）

- **Users**：用户表，包含角色标识（Admin/LotteryCenter/PrintingFactory/SalesOutlet）
- **LotteryCenter**：彩票中心
- **SalesOutlet**：销售网点（关联彩票中心）
- **PrintingFactory**：印刷厂
- **LotteryProduct**：彩票产品（包含库存信息）
- **ApplicationOrder**：申请订单（销售网点申请）
- **PrintingOrder**：印刷订单（发给印刷厂）
- **LogisticsInfo**：物流信息
- **ApprovalRecord**：审批记录

### 订单状态机

**申请订单状态流转**：
```
待审批(Pending) → 审批通过(Approved) → 待发货(WaitingShipment)
                                    → 已发货(Shipped) → 待收货(InTransit)
                                    → 已完成(Completed)
       ↓
    审批拒绝(Rejected)
```

**印刷订单状态流转**：
```
待接单(Pending) → 生产中(InProduction) → 待发货(WaitingShipment)
              → 已发货(Shipped) → 已完成(Completed)
```

## 关键技术实现

### 1. API网关（Ocelot）

**配置示例**：
```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/users/{everything}",
      "DownstreamScheme": "http",
      "DownstreamHostAndPorts": [
        { "Host": "localhost", "Port": 5001 }
      ],
      "UpstreamPathTemplate": "/api/users/{everything}",
      "UpstreamHttpMethod": [ "Get", "Post", "Put", "Delete" ],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer"
      },
      "RateLimitOptions": {
        "EnableRateLimiting": true,
        "Period": "1s",
        "Limit": 100
      }
    }
  ]
}
```

**网关功能**：
- 统一入口：所有前端请求通过网关转发
- JWT验证：在网关层统一验证token
- 限流熔断：防止服务过载
- 负载均衡：多实例服务自动负载均衡

### 2. 库存管理（防止超卖）

使用 Redis 分布式锁实现库存扣减：

```csharp
// 使用 Redis 分布式锁防止超卖
using (var redisLock = await _redisLockFactory.CreateLockAsync($"inventory:{productId}"))
{
    if (await redisLock.AcquireAsync(TimeSpan.FromSeconds(5)))
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory.Available >= quantity)
        {
            inventory.Available -= quantity;
            await _inventoryRepository.UpdateAsync(inventory);
        }
    }
}
```

**库存扣减策略**：
- 销售网点申请时：预扣库存（待审批状态）
- 审批通过：确认扣减
- 审批拒绝：释放库存

### 3. 认证授权

**微信小程序登录流程**：
1. 小程序调用 `wx.login()` 获取 code
2. 小程序将 code 发送到网关
3. 网关转发到用户服务
4. 用户服务使用 code 换取 openid 和 session_key
5. 用户服务生成 JWT token 返回
6. 小程序后续请求携带 token，网关验证

**权限控制**：
- 网关层：验证 JWT token 有效性
- 服务层：使用 `[Authorize(Policy = "RoleName")]` 进行细粒度权限控制

## 代码审核流程

### Git分支策略

采用 Git Flow 分支模型：

```
main（主分支）
  ├── develop（开发分支）
  │     ├── feature/user-login（功能分支）
  │     ├── feature/order-management（功能分支）
  │     └── feature/inventory-system（功能分支）
  ├── release/v1.0.0（发布分支）
  └── hotfix/fix-login-bug（热修复分支）
```

### 代码审核规范

**提交前检查**：
1. 代码编译通过，无编译错误
2. 单元测试全部通过
3. 代码符合编码规范（使用 EditorConfig 和 StyleCop）
4. 无明显的代码异味（重复代码、过长方法等）

**Pull Request 流程**：
1. 开发者在 feature 分支完成功能开发
2. 提交 PR 到 develop 分支
3. 至少需要 1 名团队成员审核通过
4. CI/CD 自动化测试通过
5. 合并到 develop 分支

**代码审核要点**：
- **功能正确性**：代码是否实现了需求
- **代码质量**：是否遵循 SOLID 原则和设计模式
- **安全性**：是否存在安全漏洞（SQL注入、XSS等）
- **性能**：是否存在性能问题（N+1查询、内存泄漏等）
- **可维护性**：代码是否易于理解和维护
- **测试覆盖率**：关键业务逻辑是否有单元测试

## 开发工作流程

### 开发阶段优先级

**第一阶段：基础设施搭建**
- 搭建 API 网关（Ocelot）
- 创建各个微服务项目骨架
- 配置数据库连接和 EF Core
- 实现用户认证和授权
- 配置 Redis 缓存

**第二阶段：核心业务开发**
- 彩票产品管理功能
- 销售网点申请功能
- 彩票中心审批流程
- 库存管理（Redis分布式锁）
- 订单状态流转

**第三阶段：物流管理**
- 印刷订单生成和管理
- 发货管理功能
- 第三方物流API对接
- 物流跟踪和收货确认

**第四阶段：优化完善**
- 微信模板消息推送
- 数据统计和报表
- 性能优化和缓存优化
- 安全加固和压力测试

## API规范

### 统一响应格式

所有API接口返回统一的JSON格式：

```json
{
  "code": 200,
  "message": "success",
  "data": {},
  "timestamp": 1234567890
}
```

### RESTful API设计

- 使用标准HTTP方法：GET（查询）、POST（创建）、PUT（更新）、DELETE（删除）
- URL版本控制：`/api/v1/`
- 资源命名使用复数形式：`/api/v1/products`、`/api/v1/orders`
- 网关统一前缀：所有请求通过网关 `https://gateway.example.com/api/v1/`

## 重要注意事项

### 数据一致性

- **订单状态变更**：使用数据库事务保证一致性
- **库存扣减**：使用Redis分布式锁防止超卖
- **审批流程**：记录完整的审批历史，支持审计
- **幂等性**：关键接口（如订单创建、支付）需要实现幂等性

### 安全性

- **输入验证**：验证所有用户输入，防止SQL注入、XSS攻击
- **权限验证**：网关层和服务层双重权限验证
- **敏感信息**：使用配置中心管理敏感信息，不要硬编码
- **日志记录**：记录关键操作日志，但不要记录敏感信息（密码、token等）
- **HTTPS**：生产环境必须使用HTTPS

### 性能优化

- **缓存策略**：使用Redis缓存热点数据（产品信息、库存数据）
- **数据库优化**：为常用查询字段添加索引
- **异步处理**：物流查询、消息推送等耗时操作使用异步处理
- **分页查询**：列表查询必须实现分页，避免一次性加载大量数据
- **网关缓存**：在网关层缓存不常变化的数据

## 参考文档

- **需求文档**：[doc/需求文档.md](doc/需求文档.md)
- **技术路线文档**：[doc/技术路线文档.md](doc/技术路线文档.md)
- **开发方案文档**：[doc/开发方案.md](doc/开发方案.md)
- **开发计划文档**：[doc/开发计划.md](doc/开发计划.md)
- **部署方案文档**：[doc/部署方案.md](doc/部署方案.md)
- **微信小程序开发文档**：https://developers.weixin.qq.com/miniprogram/dev/framework/
- **uni-app官方文档**：https://uniapp.dcloud.net.cn/
- **ASP.NET Core文档**：https://learn.microsoft.com/zh-cn/aspnet/core/
- **Ocelot文档**：https://ocelot.readthedocs.io/
- **Entity Framework Core文档**：https://learn.microsoft.com/zh-cn/ef/core/

## 项目状态

当前项目处于规划阶段，技术栈已确定：
- 前端：uni-app 3.x
- 网关：Ocelot
- 后端：ASP.NET Core 8.0
- 数据库：MySQL 8.0
- 缓存：Redis 7.x

开发前准备工作：
1. 搭建开发环境
2. 创建项目骨架（网关 + 微服务）
3. 设计数据库表结构
4. 定义API接口规范
5. 配置CI/CD流程
