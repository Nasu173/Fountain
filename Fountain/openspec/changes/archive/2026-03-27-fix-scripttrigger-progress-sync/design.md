# 设计方案

## 问题溯源

### 执行流程对比

**当前（错误）：**
```
第1次 ScriptTriggerEvent
  taskStarted=false → StartTask() → taskStarted=true, currentProgress=0
  currentProgress(0) >= TargetCount(1) → false → OnTaskCompleted() 不执行

第2次 ScriptTriggerEvent（通常不会有）
  taskStarted=true → UpdateTaskProgress() → currentProgress=1
  currentProgress(1) >= TargetCount(1) → true → OnTaskCompleted() ✓
```

**修复后（正确）：**
```
第1次 ScriptTriggerEvent
  taskStarted=false → StartTask() → taskStarted=true, currentProgress=0
  UpdateTaskProgress() → currentProgress=1
  currentProgress(1) >= TargetCount(1) → true → OnTaskCompleted() ✓ → nextTaskTrigger 触发
```

## 修复

将 `if/else` 改为顺序调用：

```csharp
// 修改前
if (!taskStarted)
    StartTask();
else
    UpdateTaskProgress();

// 修改后
if (!taskStarted) StartTask();
UpdateTaskProgress();
```

`UpdateTaskProgress()` 内部已有 `if (taskStarted && !taskCompleted)` 守卫，`StartTask()` 设置 `taskStarted = true` 后立即调用 `UpdateTaskProgress()` 是安全的。

## 文件变更

| 文件 | 变更 |
|------|------|
| `Assets/Script/Task/Trigger/ScriptTrigger.cs` | `OnScriptTriggerEvent` 中将 `if/else` 改为顺序调用 |
