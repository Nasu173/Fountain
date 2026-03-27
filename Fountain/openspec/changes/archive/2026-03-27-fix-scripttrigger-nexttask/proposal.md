# 修复 ScriptTrigger 的 nextTaskTrigger 未能正确触发

## 问题/需求

`ScriptTrigger` 配置了 `nextTaskTrigger` 后，下一个任务触发器未能被正确触发。

### 根本原因（两个独立 bug）

**Bug 1：`OnScriptTriggerEvent` 中 `OnTaskCompleted()` 永远不会被调用**

```csharp
private void OnScriptTriggerEvent(ScriptTriggerEvent e)
{
    if (!taskStarted) StartTask();
    // UpdateTaskProgress(); ← 被注释掉了！
    if (currentProgress >= TargetCount) OnTaskCompleted();  // ← currentProgress 永远是 0
}
```

`UpdateTaskProgress()` 被注释掉，`currentProgress` 永远不会增加，所以 `currentProgress >= TargetCount` 的条件（`targetCount` 默认为 1）永远为 `false`，`OnTaskCompleted()` 永远不会执行，`nextTaskTrigger` 自然也不会被触发。

**Bug 2：过滤逻辑嵌套错误，导致过滤失效**

用户手动修改后的过滤逻辑变成了嵌套 `if`：

```csharp
if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId)
{
    if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId)
    {
        return;  // ← 只有两个条件都不匹配时才 return
    }
}
```

原意是"任一条件不匹配就过滤"，但嵌套后变成了"两个条件都不匹配才过滤"，逻辑完全相反。这导致本应被过滤的事件通过了，本不该触发的 `ScriptTrigger` 被触发。

## 目标

- 修复 `OnScriptTriggerEvent` 中进度更新逻辑，使 `OnTaskCompleted()` 能被正确调用
- 修复过滤逻辑，恢复为正确的独立 `if` 结构
- 确保 `nextTaskTrigger` 在任务完成后被正确触发

## 方案

恢复 `UpdateTaskProgress()` 调用（取消注释），并将嵌套 `if` 改回独立的两个 `if`。
