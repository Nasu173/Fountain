# 设计方案

## 架构概览

```
[触发器 / 完成器]
    │  发布 TaskStartEvent / TaskProgressEvent
    ▼
GameEventBus
    │  订阅
    ▼
TaskManager（监听事件，管理 TaskData + TaskUI）
    │
    ▼
TaskUI（显示/动画，结构不变）
```

## 事件定义（GameEventList.cs 新增）

```csharp
// 任务开始事件（由触发器发布）
public class TaskStartEvent : IGameEvent
{
    public string TaskId;
    public string TaskName;
    public string TaskNumber;
    public int TargetCount;
    public string Description;
}

// 任务进度更新事件（由完成器/触发器发布）
public class TaskProgressEvent : IGameEvent
{
    public string TaskId;
    public int Amount;
}
```

## 文件变更

### 新增 / 修改

| 文件 | 变更 |
|------|------|
| `Assets/Script/GameEvent/GameEventList.cs` | 新增 `TaskStartEvent`、`TaskProgressEvent` |
| `Assets/Script/Task/TaskManager.cs` | 改为订阅事件；移除 `AddTask()`/`UpdateTaskProgress()` 公开方法（改为私有）；`GetActiveTasks()` 返回只读副本 |
| `Assets/Script/Task/Trigger/BaseTaskTrigger.cs` | `StartTask()` 改为发布 `TaskStartEvent`；`UpdateTaskProgress()` 改为发布 `TaskProgressEvent`；`StartNextTask()` 去掉反射，改为调用公开方法 `TriggerStart()` |
| `Assets/Script/Task/TaskInteractable.cs` | 合并三个重复类的功能；实现 `IInteractable`；发布 `TaskStartEvent`（首次）+ `TaskProgressEvent` |

### 删除

| 文件 | 原因 |
|------|------|
| `Assets/Script/Task/SimpleTaskInteractable.cs` | 与 `TaskInteractable` 完全重复 |
| `Assets/Script/Task/InteractForTaskSimple.cs` | 反射实现，由 `TaskInteractable` 替代 |
| `Assets/Script/InteractableObjects/InteractionListener.cs` | 与 `TaskInteractable` 完全重复 |

## 关键设计决策

### BaseTaskTrigger 公开 TriggerStart()

去掉反射调用，改为在 `BaseTaskTrigger` 上新增 `public void TriggerStart()` 方法，供任务链和外部直接调用：

```csharp
public void TriggerStart()
{
    if (!taskStarted) StartTask();
}
```

### TaskInteractable 统一逻辑

`TaskInteractable` 持有 `[SerializeField] BaseTaskTrigger taskTrigger`（用于获取任务信息），交互时：
1. 若任务未开始，发布 `TaskStartEvent`（从 trigger 读取信息）
2. 发布 `TaskProgressEvent`（taskId + progressPerInteract）

不再需要直接引用 `InteractTaskTrigger`，改为引用基类 `BaseTaskTrigger`，解耦更彻底。

### TaskManager 订阅生命周期

```csharp
void OnEnable()  => 订阅 TaskStartEvent + TaskProgressEvent
void OnDisable() => 取消订阅
```

### InteractConfig 保留

`InteractConfig` 类保留，`TaskInteractable` 继续使用它管理交互行为（canInteractMultipleTimes、progressOnlyFirstTime、destroyOnInteract、progressPerInteract）。

## 文件路径

- `Assets/Script/GameEvent/GameEventList.cs`
- `Assets/Script/Task/TaskManager.cs`
- `Assets/Script/Task/Trigger/BaseTaskTrigger.cs`
- `Assets/Script/Task/TaskInteractable.cs`
- ~~`Assets/Script/Task/SimpleTaskInteractable.cs`~~ （删除）
- ~~`Assets/Script/Task/InteractForTaskSimple.cs`~~ （删除）
- ~~`Assets/Script/InteractableObjects/InteractionListener.cs`~~ （删除）
