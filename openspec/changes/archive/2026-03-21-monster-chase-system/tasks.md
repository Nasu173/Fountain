# Monster Chase System — Tasks

## Phase 1: 事件与基础设施

- [x] **T1** 在 `GameEventList.cs` 末尾追加 `MonsterCatchEvent` 和 `ReviveEvent` 两个事件类

## Phase 2: 怪物 AI

- [x] **T2** 创建 `Assets/Script/Monster/MonsterChase.cs`
  - 引入 `UnityEngine.AI`（NavMeshAgent）
  - 实现 `StartChase()` / `StopChase()` 公开方法
  - Update 中驱动 NavMeshAgent 追逐 `PlayerInstance.Instance.transform`
  - 每帧同步 `agent.velocity.magnitude` 到 Animator `Speed` 参数
  - 距离检测触发 `ExecutePlayer()` 协程
  - 协程内：停止移动 → 朝向玩家 → 禁用 PlayerMove/PlayerSight → 触发 Execute 动画 → 等待 → 发布 `MonsterCatchEvent`

## Phase 3: 复活 UI

- [x] **T3** 创建 `Assets/Script/Monster/RevivePanel.cs`
  - OnEnable/OnDisable 订阅/取消订阅 `MonsterCatchEvent`
  - `Show()` 方法：激活面板，调用 `CursorManager.Instance?.SetPausePanelEnabled(true)`
  - `OnReviveClicked()` 方法：发布 `ReviveEvent`，隐藏面板，恢复鼠标锁定

## Phase 4: Unity Editor 配置

- [x] **T4** 在场景中为怪物 GameObject 添加 `NavMeshAgent`、`Animator`、`CapsuleCollider`（isTrigger=true）、`MonsterChase` 组件
- [x] **T5** 配置 Animator Controller：
  - 添加 `Speed`（Float）和 `Execute`（Trigger）参数
  - 创建 Idle → Walk/Run 混合树（由 Speed 驱动）
  - 添加 Any State → Execute 状态的 Trigger 过渡
- [x] **T6** 创建复活面板 UI，挂载 `RevivePanel.cs`，放入场景并默认隐藏（`SetActive(false)`）
- [ ] **T7** 烘焙场景 NavMesh（Window → AI → Navigation → Bake）

## Phase 5: 集成与测试

- [ ] **T8** 在任务触发器（或 `ScriptTrigger`）中调用 `MonsterChase.StartChase()` 启动追逐
- [ ] **T9** 在任务阶段结束逻辑中调用 `MonsterChase.StopChase()` 停止追逐
- [ ] **T10** 运行测试：验证追逐 → 处决 → 复活 UI 完整流程

## 文件清单

| 文件 | 操作 |
|---|---|
| `Assets/Script/GameEvent/GameEventList.cs` | 修改（追加两个事件） |
| `Assets/Script/Monster/MonsterChase.cs` | 新建 |
| `Assets/Script/Monster/RevivePanel.cs` | 新建 |
| 场景怪物 GameObject | Editor 配置 |
| Animator Controller | Editor 配置 |
| 复活面板 UI Prefab | Editor 配置 |
