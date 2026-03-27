# 设计方案

## 问题溯源

### 调用链（触发路径）

```
玩家与 NPC 交互
  → DialogueManager.StartDialogue()
  → DialogueManager.EndDialogue()
      → GameEventBus.Publish(new ScriptTriggerEvent { DialogueID = "xxx" })
          → ScriptTrigger.OnScriptTriggerEvent(e)
              → triggerId 为空 → 第一个 if 不过滤 → 通过
              → dialogueId 为空 → 第二个 if 不过滤 → 通过
              → StartTask() 被调用  ← 提前触发！
```

### 核心漏洞

`ScriptTrigger` 的过滤逻辑是"有配置才过滤，没配置就放行"：

```csharp
if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;  // triggerId 空 → 不过滤
if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return; // dialogueId 空 → 不过滤
```

当两个字段都为空时，任何 `ScriptTriggerEvent` 都能通过，等同于"响应所有广播"。

### `_gameStarted` 守卫的盲区

`BaseTaskTrigger._gameStarted` 守卫只保护 `Update()` 路径。`OnScriptTriggerEvent` 是直接注册到 `GameEventBus` 的回调，绕过了 `Update()` 和 `_gameStarted` 检查。

## 修复方案

### 双重修复

**修复 1：在 `OnScriptTriggerEvent` 中复用 `_gameStarted` 守卫**

将 `_gameStarted` 从 `private` 改为 `protected`（或新增 `protected bool IsGameStarted` 属性），让 `ScriptTrigger` 能访问并在事件回调中检查。

**修复 2：要求至少配置一个过滤条件**

当 `triggerId` 和 `dialogueId` 均为空时，`ScriptTrigger` 不响应任何广播（防止意外的"响应所有"行为）。

```csharp
private void OnScriptTriggerEvent(ScriptTriggerEvent e)
{
    if (taskCompleted) return;
    if (!IsGameStarted) return;  // ← 新增：游戏未开始不响应

    // 至少需要一个过滤条件匹配
    if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;
    if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return;
    // 两者均为空时也不响应（无配置 = 未激活）
    if (string.IsNullOrEmpty(triggerId) && string.IsNullOrEmpty(dialogueId)) return;  // ← 新增

    if (!taskStarted) StartTask();
    if (currentProgress >= TargetCount) OnTaskCompleted();
}
```

## 文件变更

| 文件 | 变更 |
|------|------|
| `Assets/Script/Task/Trigger/BaseTaskTrigger.cs` | 将 `_gameStarted` 改为 `protected`，或新增 `protected bool IsGameStarted => _gameStarted` 属性 |
| `Assets/Script/Task/Trigger/ScriptTrigger.cs` | `OnScriptTriggerEvent` 增加 `IsGameStarted` 守卫 + 空配置守卫 |

## 关键约束

- `_gameStarted` 字段在 `BaseTaskTrigger` 中是 `private`，需改为 `protected` 或通过属性暴露
- 不修改 `DialogueManager` 的发布逻辑（发布方不应承担过滤责任）
- 不影响已正确配置了 `triggerId` 或 `dialogueId` 的 `ScriptTrigger` 实例
