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
- **状态管理**：Vuex 或 Pinia
- **开发工具**：HBuilderX

### 后端
- **框架**：ASP.NET Core 8.0
- **ORM**：Entity Framework Core（Code First）
- **认证授权**：JWT + ASP.NET Core Identity
- **API文档**：Swagger/OpenAPI
- **日志**：Serilog
- **缓存**：Redis

### 数据库
- **主数据库**：MySQL 8.0
- **缓存**：Redis（库存、会话管理）
- **对象存储**：阿里云OSS 或 腾讯云COS

## 项目架构

### 分层架构（Layered Architecture + DDD）

```
前端层（uni-app）
    ├── 销售网点端
    ├── 彩票中心端
    ├── 印刷厂端
    └── 管理员端

API层（ASP.NET Core Web API）
    ├── Controllers（控制器）
    ├── Middleware（中间件：认证、异常处理、日志）
    └── DTOs（数据传输对象）

业务逻辑层（Business Logic Layer）
    ├── OrderService（订单管理）
    ├── InventoryService（库存管理）
    ├── ApprovalService（审批流程）
    ├── UserService（用户权限）
    ├── LogisticsService（物流对接）
    └── NotificationService（消息通知）

数据访问层（Data Access Layer）
    ├── DbContext（EF Core）
    ├── Repositories（仓储模式）
    └── UnitOfWork（工作单元）

基础设施层（Infrastructure）
    ├── 微信API对接
    ├── 物流API对接（快递100/快递鸟）
    ├── Redis缓存服务
    └── OSS文件存储
```

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

**申请订单状态**：
```
待审批(Pending) → 审批通过(Approved) → 待发货(WaitingShipment)
                                    → 已发货(Shipped) → 待收货(InTransit)
                                    → 已完成(Completed)
       ↓
    审批拒绝(Rejected)
```

**印刷订单状态**：
```
待接单(Pending) → 生产中(InProduction) → 待发货(WaitingShipment)
              → 已发货(Shipped) → 已完成(Completed)
```

## 关键技术实现

### 1. 库存管理（防止超卖）

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

### 2. 认证授权

**微信小程序登录流程**：
1. 小程序调用 `wx.login()` 获取 code
2. 后端使用 code 换取 openid 和 session_key
3. 后端生成 JWT token 返回给小程序
4. 小程序后续请求携带 token

**权限控制**：
- 使用 ASP.NET Core Policy-based Authorization
- JWT token 中包含用户角色信息
- API 接口使用 `[Authorize(Policy = "RoleName")]` 进行权限控制

### 3. 物流对接

**第三方物流API**：
- 快递100（https://www.kuaidi100.com/）
- 快递鸟（https://www.kdniao.com/）

**实现方式**：
- 印刷厂发货时填写物流公司和运单号
- 后端异步查询物流信息
- 定时任务更新物流状态
- 物流状态变更时推送消息通知

## 开发工作流程

### 开发阶段优先级

**第一阶段：基础功能**
- 用户登录认证（微信授权 + JWT）
- 用户角色管理
- 彩票中心、销售网点、印刷厂基础数据管理
- 权限控制实现

**第二阶段：核心业务**
- 彩票产品发布和管理
- 销售网点申请彩票
- 彩票中心审批流程
- 库存管理（Redis分布式锁）

**第三阶段：物流管理**
- 印刷订单生成和管理
- 发货管理
- 物流信息对接
- 物流跟踪和收货确认

**第四阶段：优化完善**
- 消息通知（模板消息）
- 数据统计和报表
- 性能优化
- 缓存优化

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

## 重要注意事项

### 数据一致性

- **订单状态变更**：使用数据库事务保证一致性
- **库存扣减**：使用Redis分布式锁防止超卖
- **审批流程**：记录完整的审批历史，支持审计
- **幂等性**：关键接口（如订单创建、支付）需要实现幂等性

### 安全性

- **输入验证**：验证所有用户输入，防止SQL注入、XSS攻击
- **权限验证**：每个API接口都需要验证用户角色权限
- **敏感信息**：不要在代码中硬编码API密钥、数据库密码等敏感信息
- **日志记录**：记录关键操作日志，但不要记录敏感信息（密码、token等）

### 性能优化

- **缓存策略**：使用Redis缓存热点数据（产品信息、库存数据）
- **数据库优化**：为常用查询字段添加索引
- **异步处理**：物流查询、消息推送等耗时操作使用异步处理
- **分页查询**：列表查询必须实现分页，避免一次性加载大量数据

## 参考文档

- **需求文档**：[doc/需求文档.md](doc/需求文档.md)
- **技术路线文档**：[doc/技术路线文档.md](doc/技术路线文档.md)
- **微信小程序开发文档**：https://developers.weixin.qq.com/miniprogram/dev/framework/
- **uni-app官方文档**：https://uniapp.dcloud.net.cn/
- **ASP.NET Core文档**：https://learn.microsoft.com/zh-cn/aspnet/core/
- **Entity Framework Core文档**：https://learn.microsoft.com/zh-cn/ef/core/

## 项目状态

当前项目处于规划阶段，尚未开始实际开发。在开始编码前，建议：
1. 确认技术栈选型
2. 搭建开发环境
3. 创建项目骨架
4. 设计数据库表结构
5. 定义API接口规范
