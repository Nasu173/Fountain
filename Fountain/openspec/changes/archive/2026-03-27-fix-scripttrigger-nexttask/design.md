# 设计方案

## Bug 1 详解：进度永远不更新

`ScriptTrigger` 的任务完成流程依赖 `currentProgress >= TargetCount`：

```
OnScriptTriggerEvent()
  → StartTask()           ← 任务开始，currentProgress = 0
  → UpdateTaskProgress()  ← 被注释！currentProgress 不增加
  → currentProgress(0) >= TargetCount(1) → false → OnTaskCompleted() 不执行
  → nextTaskTrigger 永远不触发
```

修复：取消注释 `UpdateTaskProgress()`。

## Bug 2 详解：过滤逻辑嵌套错误

**错误的嵌套逻辑（当前代码）：**
```csharp
if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId)
{
    if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId)
    {
        return;  // 只有 triggerId 不匹配 AND dialogueId 不匹配才过滤
    }
}
// 其他情况全部通过 ← 错误
```

**正确的独立逻辑：**
```csharp
if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;
if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return;
// triggerId 不匹配 OR dialogueId 不匹配，任一不匹配就过滤
```

## 修复后的完整 OnScriptTriggerEvent

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

## 文件变更

| 文件 | 变更 |
|------|------|
| `Assets/Script/Task/Trigger/ScriptTrigger.cs` | 取消注释 `UpdateTaskProgress()`；将嵌套 `if` 改回独立两个 `if` |
