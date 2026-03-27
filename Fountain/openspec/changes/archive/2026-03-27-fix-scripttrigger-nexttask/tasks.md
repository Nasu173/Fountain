# 实现任务

## Task 1: 修复 ScriptTrigger.OnScriptTriggerEvent

- 路径: `Assets/Script/Task/Trigger/ScriptTrigger.cs`
- 将嵌套 `if` 改回独立两个 `if`（过滤逻辑修复）
- 取消注释 `UpdateTaskProgress()`（进度更新修复）

```csharp
private void OnScriptTriggerEvent(ScriptTriggerEvent e)
{
    if (taskCompleted) return;
    if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;
    if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return;

    if (!taskStarted) StartTask();
    UpdateTaskProgress();
    if (currentProgress >= TargetCount) OnTaskCompleted();
}
```

## Task 2: 验证编译与行为

- 检查 Unity Console 无编译错误
- 验证 `ScriptTrigger` 收到匹配事件后，`currentProgress` 正确递增
- 验证 `currentProgress >= TargetCount` 时 `OnTaskCompleted()` 被调用
- 验证 `nextTaskTrigger` 在任务完成后被正确触发
