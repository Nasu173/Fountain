# 修复点击开始游戏后任务被提前触发的问题

## 问题/需求

点击主菜单"开始游戏"后，`TaskManager` 的 `Start` 函数（实为 `OnTaskStart` 事件处理器）被多次调用，导致任务在玩家真正开始游戏前就被触发。

### 根本原因（多层叠加）

**原因 1：GameEventBus 静态状态跨场景不清理**

`GameEventBus._handlers` 是静态字典，在场景切换时从不重置。当游戏场景以 Additive 方式加载时，新场景中的触发器会在 `Start()` 中订阅事件，但旧场景遗留的订阅仍然存在，导致同一事件被多个处理器响应。

**原因 2：TaskManager 重复订阅**

`TaskManager` 在 `OnEnable` 中订阅事件。若 `TaskManager` 所在的 GameObject 被 `SetActive(false)` 再 `SetActive(true)`（例如场景重载、UI 面板切换触发父对象激活状态变化），`OnEnable` 会再次执行。虽然 `GameEventBus.Subscribe` 有重复检查，但若 `OnDisable` 未被正确调用（如对象被销毁前），订阅不会被清理。

**原因 3：触发器 Start() 在场景加载时立即执行**

`BaseTaskTrigger.Start()` 在游戏场景加载完成后立即执行（Unity 生命周期）。此时玩家尚未真正"进入游戏"（`GameStartEvent` 已发布，但玩家可能还在过渡动画中）。若某个触发器的 `CheckTriggerCondition()` 在场景初始化时恰好返回 `true`，或 `ScriptTrigger` 收到了残留的 `ScriptTriggerEvent` 广播，任务会立即启动。

**原因 4：ScriptTrigger 订阅时机**

`ScriptTrigger.Start()` 订阅 `ScriptTriggerEvent`。由于 `GameEventBus` 静态状态不清理，若上一次游戏会话中发布过 `ScriptTriggerEvent`，新场景加载后的 `ScriptTrigger` 不会收到历史事件（事件是即时的），但若有其他系统在场景加载期间发布了 `ScriptTriggerEvent`，会立即触发。

## 目标

- 确保任务触发器在 `GameStartEvent` 发布后才允许响应触发条件
- 确保 `GameEventBus` 在场景切换时清理与已卸载场景相关的订阅
- 不改变现有任务系统的事件驱动架构
- 不影响任务链（nextTaskTrigger）功能

## 方案

在 `BaseTaskTrigger` 中增加"游戏已开始"守卫：触发器在 `GameStartEvent` 发布前不响应任何触发条件，也不启动任务。

在 `GameSceneManager` 的场景卸载流程中调用 `GameEventBus.ClearAllSubscriptions()`，防止静态状态跨场景污染。
