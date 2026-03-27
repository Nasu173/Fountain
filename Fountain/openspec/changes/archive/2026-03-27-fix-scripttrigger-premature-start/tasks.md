# 实现任务

## Task 1: 修改 BaseTaskTrigger — 暴露 IsGameStarted 属性

- 路径: `Assets/Script/Task/Trigger/BaseTaskTrigger.cs`
- 将 `private bool _gameStarted` 改为 `private bool _gameStarted`（保持 private）
- 新增 `protected bool IsGameStarted => _gameStarted` 属性，供子类访问

```csharp
// 新增属性（紧跟 _gameStarted 字段之后）
protected bool IsGameStarted => _gameStarted;
```

## Task 2: 修改 ScriptTrigger — 增加双重守卫

- 路径: `Assets/Script/Task/Trigger/ScriptTrigger.cs`
- 在 `OnScriptTriggerEvent` 开头增加两个守卫：
  1. `if (!IsGameStarted) return;` — 游戏未开始不响应
  2. `if (string.IsNullOrEmpty(triggerId) && string.IsNullOrEmpty(dialogueId)) return;` — 无配置不响应

```csharp
private void OnScriptTriggerEvent(ScriptTriggerEvent e)
{
    if (taskCompleted) return;
    if (!IsGameStarted) return;
    if (string.IsNullOrEmpty(triggerId) && string.IsNullOrEmpty(dialogueId)) return;

    if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;
    if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return;

    if (!taskStarted) StartTask();
    if (currentProgress >= TargetCount) OnTaskCompleted();
}
```

## Task 3: 验证编译与行为

- 检查 Unity Console 无编译错误
- 验证对话结束后，未配置 `triggerId`/`dialogueId` 的 `ScriptTrigger` 不再被触发
- 验证已正确配置 `dialogueId` 的 `ScriptTrigger` 在对应对话结束后仍能正常触发
- 验证游戏未开始时（`_gameStarted = false`），`ScriptTrigger` 不响应任何广播
