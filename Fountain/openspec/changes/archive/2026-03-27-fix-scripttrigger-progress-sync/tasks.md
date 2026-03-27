# 实现任务

## Task 1: ✅ 暴露 ScriptTrigger.TriggerId 属性

- 路径: `Assets/Script/Task/Trigger/ScriptTrigger.cs`
- 新增 `public string TriggerId => triggerId;` 属性，供 `TaskInteractable` 读取

## Task 3: ✅ 修改 TaskInteractable.InteractWith — 关联 ScriptTrigger 时改走广播路径

- 路径: `Assets/Script/Task/TaskInteractable.cs`
- 在 `InteractWith` 中，判断 `taskTrigger` 是否为 `ScriptTrigger`：
  - 是：发布 `ScriptTriggerEvent { TriggerId = ((ScriptTrigger)taskTrigger).TriggerId }`，不再直接发布 `TaskProgressEvent`
  - 否：保持原有逻辑（发布 `TaskStartEvent` + `TaskProgressEvent`）

```csharp
if (taskTrigger is ScriptTrigger st)
{
    GameEventBus.Publish(new ScriptTriggerEvent { TriggerId = st.TriggerId });
}
else
{
    if (!taskTrigger.TaskStarted)
        GameEventBus.Publish(new TaskStartEvent { ... });
    GameEventBus.Publish(new TaskProgressEvent { ... });
}
```

## Task 4: ✅ 验证编译与行为

- 检查 Unity Console 无编译错误
- 验证 `TaskInteractable` 关联 `ScriptTrigger` 时，交互后通过 `ScriptTriggerEvent` 推进进度并触发 `nextTaskTrigger`
- 验证 `TaskInteractable` 关联其他触发器类型时，原有逻辑不受影响

