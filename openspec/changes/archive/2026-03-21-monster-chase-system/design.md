# Monster Chase System — Design

## Architecture Overview

```
TaskManager (现有)
    │ 任务阶段结束
    ▼
MonsterChase.cs ──── NavMeshAgent ──── 追逐玩家
    │                    │
    │              速度同步
    │                    ▼
    │              Animator (Speed 参数)
    │
    │ OnTriggerEnter (碰到玩家)
    ▼
处决流程协程
    ├── 禁用 PlayerMove / PlayerSight
    ├── 播放处决动画 (Animator trigger)
    ├── 等待动画结束
    └── 发布 MonsterCatchEvent
            │
            ▼
        RevivePanel.cs (订阅事件，显示复活 UI)
```

## MonsterChase.cs

**职责**：怪物 AI 主控，管理追逐、停止、处决三个状态。

**状态机（简单枚举）**：
```
Idle → Chasing → Executing → Done
```

**关键字段**：
```csharp
[SerializeField] float chaseSpeed = 4f;       // NavMeshAgent 速度
[SerializeField] float executionDistance = 1.2f; // 触发处决的距离
[SerializeField] string animSpeedParam = "Speed";  // Animator float 参数名
[SerializeField] string animExecuteTrigger = "Execute"; // 处决触发器名
[SerializeField] float executionAnimDuration = 3f; // 处决动画时长（秒）
```

**追逐逻辑**（Update）：
- 状态为 `Chasing` 时：`agent.SetDestination(player.position)`
- 每帧同步：`animator.SetFloat(animSpeedParam, agent.velocity.magnitude)`
- 距离 ≤ executionDistance 时进入处决流程

**停止追逐**（公开方法 `StopChase()`）：
- 由外部（任务阶段结束时）调用
- `agent.isStopped = true`，状态切换为 `Idle`
- 动画速度归零

**处决流程**（协程 `ExecutePlayer()`）：
1. 状态 → `Executing`，停止 NavMeshAgent
2. 怪物朝向玩家（`transform.LookAt`）
3. 禁用 `PlayerMove.enabled = false`，`PlayerSight.enabled = false`
4. 触发 `animator.SetTrigger(animExecuteTrigger)`
5. `yield return new WaitForSeconds(executionAnimDuration)`
6. 发布 `GameEventBus.Publish(new MonsterCatchEvent())`
7. 状态 → `Done`

**激活入口**（公开方法 `StartChase()`）：
- 由任务触发器或 ScriptTrigger 调用
- 状态 → `Chasing`，`agent.isStopped = false`

## RevivePanel.cs

**职责**：监听 `MonsterCatchEvent`，显示复活 UI 面板，解锁鼠标。

**关键逻辑**：
```csharp
void OnEnable()  → GameEventBus.Subscribe<MonsterCatchEvent>(Show)
void OnDisable() → GameEventBus.Unsubscribe<MonsterCatchEvent>(Show)

void Show(MonsterCatchEvent e)
{
    gameObject.SetActive(true);
    CursorManager.Instance?.SetPausePanelEnabled(true); // 显示鼠标
}

public void OnReviveClicked()
{
    GameEventBus.Publish(new ReviveEvent());
    gameObject.SetActive(false);
    CursorManager.Instance?.SetPausePanelEnabled(false);
}
```

## 新增游戏事件（GameEventList.cs 追加）

```csharp
[System.Serializable]
public class MonsterCatchEvent : IGameEvent { }

[System.Serializable]
public class ReviveEvent : IGameEvent { }
```

## 动画适配方案

| Animator 参数 | 类型 | 说明 |
|---|---|---|
| `Speed` | Float | 绑定 `agent.velocity.magnitude`，驱动行走/跑步混合树 |
| `Execute` | Trigger | 触发处决动画 |

推荐 Animator 结构：
```
Any State ──[Execute trigger]──► Execute State (处决动画)
Idle ──[Speed > 0.1]──► Walk/Run Blend Tree
Walk/Run Blend Tree ──[Speed < 0.1]──► Idle
```

## 集成方式

怪物 GameObject 需要：
- `NavMeshAgent` 组件（已烘焙 NavMesh）
- `Animator` 组件（含上述参数）
- `MonsterChase` 脚本
- `CapsuleCollider`（设为 Trigger，用于检测玩家碰撞）

任务阶段结束时调用：
```csharp
monster.GetComponent<MonsterChase>().StopChase();
```

任务阶段开始时调用：
```csharp
monster.GetComponent<MonsterChase>().StartChase();
```

或通过 `ScriptTrigger` 事件驱动，无需硬引用。
