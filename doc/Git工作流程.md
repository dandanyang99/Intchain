# Git 工作流程

## 分支策略

本项目采用 **Git Flow** 分支模型，确保代码质量和发布流程的规范性。

## 分支类型

### 1. 主分支（main）
- **用途**：生产环境代码，始终保持稳定可发布状态
- **保护规则**：禁止直接推送，只能通过 Pull Request 合并
- **合并来源**：release 分支、hotfix 分支

### 2. 开发分支（develop）
- **用途**：日常开发的主分支，包含最新的开发进度
- **保护规则**：禁止直接推送，只能通过 Pull Request 合并
- **合并来源**：feature 分支

### 3. 功能分支（feature/*）
- **用途**：开发新功能
- **命名规范**：`feature/功能名称`（例如：`feature/user-login`）
- **创建来源**：从 develop 分支创建
- **合并目标**：合并回 develop 分支
- **生命周期**：功能开发完成后删除

### 4. 发布分支（release/*）
- **用途**：准备新版本发布，进行最后的测试和bug修复
- **命名规范**：`release/v版本号`（例如：`release/v1.0.0`）
- **创建来源**：从 develop 分支创建
- **合并目标**：同时合并到 main 和 develop 分支
- **生命周期**：发布完成后删除

### 5. 热修复分支（hotfix/*）
- **用途**：紧急修复生产环境的bug
- **命名规范**：`hotfix/bug描述`（例如：`hotfix/fix-login-bug`）
- **创建来源**：从 main 分支创建
- **合并目标**：同时合并到 main 和 develop 分支
- **生命周期**：修复完成后删除

## 工作流程

### 开发新功能

```bash
# 1. 从 develop 创建功能分支
git checkout develop
git pull origin develop
git checkout -b feature/user-login

# 2. 开发功能，提交代码
git add .
git commit -m "feat(auth): 实现用户登录功能"

# 3. 推送到远程仓库
git push origin feature/user-login

# 4. 在 GitHub 创建 Pull Request，合并到 develop
# 5. 代码审核通过后，合并并删除功能分支
git checkout develop
git pull origin develop
git branch -d feature/user-login
```

### 发布新版本

```bash
# 1. 从 develop 创建发布分支
git checkout develop
git pull origin develop
git checkout -b release/v1.0.0

# 2. 进行最后的测试和bug修复
git add .
git commit -m "fix: 修复发布前的小问题"

# 3. 推送到远程仓库
git push origin release/v1.0.0

# 4. 合并到 main 分支
git checkout main
git pull origin main
git merge --no-ff release/v1.0.0
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin main --tags

# 5. 合并回 develop 分支
git checkout develop
git merge --no-ff release/v1.0.0
git push origin develop

# 6. 删除发布分支
git branch -d release/v1.0.0
git push origin --delete release/v1.0.0
```

### 紧急修复

```bash
# 1. 从 main 创建热修复分支
git checkout main
git pull origin main
git checkout -b hotfix/fix-login-bug

# 2. 修复bug，提交代码
git add .
git commit -m "fix: 修复登录验证bug"

# 3. 推送到远程仓库
git push origin hotfix/fix-login-bug

# 4. 合并到 main 分支
git checkout main
git merge --no-ff hotfix/fix-login-bug
git tag -a v1.0.1 -m "Hotfix version 1.0.1"
git push origin main --tags

# 5. 合并回 develop 分支
git checkout develop
git merge --no-ff hotfix/fix-login-bug
git push origin develop

# 6. 删除热修复分支
git branch -d hotfix/fix-login-bug
git push origin --delete hotfix/fix-login-bug
```

## 提交消息规范

遵循 Conventional Commits 规范：

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Type 类型
- `feat`: 新功能
- `fix`: 修复bug
- `docs`: 文档更新
- `style`: 代码格式调整
- `refactor`: 重构
- `perf`: 性能优化
- `test`: 测试相关
- `chore`: 构建过程或辅助工具的变动
- `ci`: CI/CD配置变更

### 示例

```
feat(auth): 添加用户登录功能

- 实现JWT token验证
- 添加登录API接口
- 添加用户会话管理

Closes #123
```

## Pull Request 规范

### PR 标题
使用与提交消息相同的格式：`<type>(<scope>): <subject>`

### PR 描述模板
```markdown
## 变更说明
简要描述本次变更的内容和目的

## 变更类型
- [ ] 新功能
- [ ] Bug修复
- [ ] 重构
- [ ] 文档更新
- [ ] 其他

## 测试
- [ ] 单元测试通过
- [ ] 集成测试通过
- [ ] 手动测试通过

## 相关Issue
Closes #issue编号
```

### PR 审核要点
1. 代码功能正确性
2. 代码质量和可维护性
3. 安全性检查
4. 性能影响评估
5. 测试覆盖率

## 分支保护规则

### main 分支
- 禁止直接推送
- 需要至少 1 名审核者批准
- 需要通过所有状态检查（CI/CD）
- 需要分支是最新的

### develop 分支
- 禁止直接推送
- 需要至少 1 名审核者批准
- 需要通过所有状态检查（CI/CD）

---

**文档版本**：1.0
**创建时间**：2026-01-18
**最后更新**：2026-01-18
