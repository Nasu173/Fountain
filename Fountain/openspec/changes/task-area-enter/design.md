# 设计方案

## 文件

- 新建 `Assets/Script/Task/TaskAreaEnter.cs`

## 类设计

```
TaskAreaEnter : MonoBehaviour
```

### Inspector 字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `taskTrigger` | `BaseTaskTrigger` | 关联触发器，提供任务信息 |
| `playerTag` | `string` | 用于过滤进入者的 Tag，默认 "Player" |
| `canTriggerMultipleTimes` | `bool` | 是否允许多次触发，默认 false |
| `progressPerEnter` | `int` | 每次进入增加的进度值，默认 1 |
| `showDebug` | `bool` | 调试日志开关 |

### 逻辑（OnTriggerEnter）

1. 检查进入者 Tag 是否匹配 `playerTag`
2. 若 `!canTriggerMultipleTimes && hasTriggered`，直接返回
3. 若 `!taskTrigger.TaskStarted`，发布 `TaskStartEvent`（从 trigger 读取信息）
4. 发布 `TaskProgressEvent`（taskId + progressPerEnter）
5. 标记 `hasTriggered = true`

### 与 TaskInteractable 的对比

| | TaskInteractable | TaskAreaEnter |
|---|---|---|
| 触发方式 | 玩家按键 | 进入碰撞体 |
| 接口 | IInteractable | 无 |
| 视觉反馈 | OutlineVisual | 无（区域触发不需要） |
| 多次触发 | canInteractMultipleTimes | canTriggerMultipleTimes |
| 进度值 | config.progressPerInteract | progressPerEnter |
| 事件发布 | 相同模式 | 相同模式 |

## 使用要求

- GameObject 必须有 Collider 且勾选 **Is Trigger**
- 玩家 GameObject 的 Tag 需与 `playerTag` 匹配（默认 "Player"）
