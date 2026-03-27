# 修复 ScriptTrigger 被提前触发的问题

## 问题/需求

点击"开始游戏"后，`ScriptTrigger` 的 `Start` 函数（`OnScriptTriggerEvent` 处理器）被多次调用，导致任务在玩家真正开始游戏前就被触发。

### 根本原因

**`DialogueManager.EndDialogue()` 发布 `ScriptTriggerEvent` 时不携带 `TriggerId`**

```csharp
// DialogueManager.cs:150
GameEventBus.Publish(new ScriptTriggerEvent
{
    DialogueID = currentDialogues.id.ToString()
    // TriggerId 为 null/空
});
```

`ScriptTrigger.OnScriptTriggerEvent` 的过滤逻辑：

```csharp
// 如果设置了triggerId，只响应匹配的广播
if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;
// 如果设置了dialogueId，只响应匹配的广播
if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return;
```

**当 `ScriptTrigger` 的 `triggerId` 字段为空时，第一个 `if` 条件为 `false`（不过滤），直接通过。**

这意味着：任何对话结束时发布的 `ScriptTriggerEvent`，都会触发所有 `triggerId` 为空的 `ScriptTrigger`，无论该对话与该触发器是否有关联。

### 为何会"多次调用 Start"

`GameEventBus._handlers` 是静态字典，跨场景不清理。若场景中存在多个 `ScriptTrigger`（或同一触发器因场景重载被多次实例化），每次对话结束都会触发所有未设置 `triggerId` 的 `ScriptTrigger`，导致 `StartTask()` 被重复调用（虽然 `taskStarted` 守卫会阻止重复发布 `TaskStartEvent`，但 `OnScriptTriggerEvent` 仍会被多次调用）。

### 与上一个修复的关系

上一个修复（`fix-taskmanager-premature-start`）解决了 `_gameStarted` 守卫问题，确保触发器在 `GameStartEvent` 前不响应 `Update()`。但 `ScriptTrigger.OnScriptTriggerEvent` 是事件回调，**不经过 `Update()`**，因此不受 `_gameStarted` 守卫保护。只要有对话结束，`ScriptTrigger` 就会被触发，与游戏是否已开始无关。

## 目标

- 确保 `ScriptTrigger` 只响应与自身关联的 `ScriptTriggerEvent`
- 消除"空 `triggerId` 导致响应所有广播"的漏洞
- 不改变现有对话系统的发布逻辑

## 方案

在 `ScriptTrigger.OnScriptTriggerEvent` 中增加 `_gameStarted` 守卫（与 `Update()` 保持一致），确保游戏未开始时不响应任何广播。同时修复过滤逻辑：当 `triggerId` 和 `dialogueId` 均为空时，拒绝响应（要求至少配置一个过滤条件），防止"响应所有广播"的意外行为。
