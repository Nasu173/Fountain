# TaskInteractable 关联 ScriptTrigger 时改走广播路径

## 问题/需求

`TaskInteractable` 关联 `ScriptTrigger` 时，`InteractWith` 直接发布 `TaskProgressEvent`，绕过了 `ScriptTrigger` 的进度逻辑（`currentProgress` 不更新、`OnTaskCompleted()` 不执行、`nextTaskTrigger` 不触发）。

### 根本原因

`TaskInteractable.InteractWith` 对所有触发器类型统一发布 `TaskProgressEvent`，但 `ScriptTrigger` 的进度由自身的 `currentProgress` 字段维护，只有通过 `ScriptTriggerEvent` 才能正确推进。直接发布 `TaskProgressEvent` 只更新了 `TaskManager` 的 UI，`ScriptTrigger.currentProgress` 仍为 0，导致 `OnTaskCompleted()` 永远不执行。

## 目标

`TaskInteractable` 关联 `ScriptTrigger` 时，交互触发 `ScriptTriggerEvent`，由 `ScriptTrigger` 自己处理进度推进和任务链触发。

## 方案

`TaskInteractable` 关联 `ScriptTrigger` 时，`InteractWith` 改为发布 `ScriptTriggerEvent`（携带 `triggerId`），让 `ScriptTrigger` 自己处理进度推进和任务链触发，而非直接发布 `TaskProgressEvent` 绕过 `ScriptTrigger`。

当 `taskTrigger` 是 `ScriptTrigger` 类型时：
```csharp
GameEventBus.Publish(new ScriptTriggerEvent
{
    TriggerId = ((ScriptTrigger)taskTrigger).TriggerId
});
```
需要将 `ScriptTrigger.triggerId` 暴露为 `public` 属性供 `TaskInteractable` 读取。
