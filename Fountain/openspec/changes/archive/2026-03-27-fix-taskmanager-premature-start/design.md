# 设计方案

## 架构概览

```
MainMenuPanel.OnStartClicked()
    │  发布 GameStartEvent
    ▼
BaseTaskTrigger（订阅 GameStartEvent，设置 gameStarted = true）
    │  gameStarted == true 后才允许 CheckTriggerCondition / StartTask
    ▼
GameEventBus → TaskManager.OnTaskStart
```

## 问题溯源

### 调用链

```
MainMenuPanel.OnStartClicked()
  → GameEventBus.Publish(GameStartEvent)       ← 已有
  → GameEventBus.Publish(LoadSceneEvent)       ← 已有
      → GameSceneManager 加载游戏场景（Additive）
          → 场景中所有 MonoBehaviour.Start() 执行
              → BaseTaskTrigger.Start()         ← 此时 gameStarted 应为 true
              → BaseTaskTrigger.Update()        ← 每帧检查 CheckTriggerCondition
```

**问题**：`GameStartEvent` 在 `LoadSceneEvent` 之前发布，但场景加载是异步的。场景中的 `Start()` 在加载完成后才执行，此时 `GameStartEvent` 早已发布完毕。`BaseTaskTrigger` 没有订阅 `GameStartEvent`，所以不知道游戏是否已开始，直接在 `Update()` 中检查触发条件。

**另一个问题**：`GameEventBus._handlers` 是静态的，跨场景不清理。若从游戏场景返回主菜单再重新开始，旧场景的订阅者（已销毁的对象）仍留在 `_handlers` 中，调用时会产生空引用或重复响应。

## 修复方案

### 方案 A（推荐）：BaseTaskTrigger 订阅 GameStartEvent

在 `BaseTaskTrigger` 中增加 `gameStarted` 标志，订阅 `GameStartEvent`：

```csharp
private bool gameStarted = false;

protected override void Start()
{
    base.Start();
    GameEventBus.Subscribe<GameStartEvent>(OnGameStart);
}

private void OnDestroy()
{
    GameEventBus.Unsubscribe<GameStartEvent>(OnGameStart);
}

private void OnGameStart(GameStartEvent e)
{
    gameStarted = true;
}

protected override void Update()
{
    if (!gameStarted) return;   // ← 守卫
    base.Update();
}
```

**优点**：最小改动，只修改 `BaseTaskTrigger` 一个文件，所有子类自动受保护。

**注意**：`GameStartEvent` 在场景加载前发布，场景加载完成后 `Start()` 才执行，此时订阅 `GameStartEvent` 已经错过了事件。需要在 `Start()` 中检查游戏是否已经开始（通过 `GameSceneManager` 或 `PanelManager` 的状态，或改为在 `GameStartEvent` 发布时设置一个静态标志）。

### 方案 B（推荐，更可靠）：静态标志 + 事件双重保障

新增静态标志 `GameState.IsStarted`，在 `GameStartEvent` 发布时设置，在返回主菜单时重置：

```csharp
// 新增 GameState.cs（或在 GameEventBus 中）
public static class GameState
{
    public static bool IsGameStarted { get; private set; }

    public static void SetStarted() => IsGameStarted = true;
    public static void SetStopped() => IsGameStarted = false;
}
```

`BaseTaskTrigger.Start()` 直接读取 `GameState.IsGameStarted`，同时订阅 `GameStartEvent` 以响应后续变化。

**但这引入了新的全局状态**，增加复杂度。

### 方案 C（最简，推荐）：PanelManager 已有 isStarted，复用

`PanelManager` 已有 `isStarted` 字段，在 `OnGameStart` 中设为 `true`，在 `OnMenuClicked` 中设为 `false`。但 `BaseTaskTrigger` 不应直接依赖 `PanelManager`。

### 最终方案：方案 A + 修复事件时序

**关键洞察**：`GameStartEvent` 在 `LoadSceneEvent` 之前发布（见 `MainMenuPanel.OnStartClicked`）。游戏场景加载完成后，`BaseTaskTrigger.Start()` 执行时，`GameStartEvent` 早已发布完毕，订阅已经错过。

**解决时序问题**：将 `GameStartEvent` 改为在场景加载完成后（`SceneLoadedEvent`）发布，或在 `BaseTaskTrigger.Start()` 中通过一个持久化标志判断。

最简方案：**在 `GameEventBus` 中为 `GameStartEvent` 保留"最后一次发布的状态"**（sticky event），或在 `PanelManager`/`GameSceneManager` 中维护一个可查询的静态属性。

**实际最简方案**：在 `MainMenuPanel` 中，将 `GameStartEvent` 改为在 `SceneLoadedEvent` 之后发布（监听 `SceneLoadedEvent` 再发布 `GameStartEvent`）。这样场景加载完成、`Start()` 执行后，`GameStartEvent` 才发布，`BaseTaskTrigger` 能正确接收。

## 选定方案：调整 GameStartEvent 发布时机 + BaseTaskTrigger 守卫

### 变更 1：MainMenuPanel — 延迟 GameStartEvent 到场景加载完成后

```csharp
// 修改前
public void OnStartClicked()
{
    GameEventBus.Publish(new GameStartEvent());
    GameEventBus.Publish(new LoadSceneEvent { ... });
}

// 修改后：移除 GameStartEvent，改为监听 SceneLoadedEvent 后发布
private void OnEnable()
{
    GameEventBus.Subscribe<SceneLoadedEvent>(OnSceneLoaded);
    ...
}
private void OnDisable()
{
    GameEventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoaded);
    ...
}
private bool _waitingForScene = false;
public void OnStartClicked()
{
    _waitingForScene = true;
    SetMainMenuState(false);
    GameEventBus.Publish(new LoadSceneEvent { ... });
}
private void OnSceneLoaded(SceneLoadedEvent e)
{
    if (!_waitingForScene) return;
    _waitingForScene = false;
    GameEventBus.Publish(new GameStartEvent());
}
```

### 变更 2：BaseTaskTrigger — 增加 gameStarted 守卫

```csharp
private bool _gameStarted = false;

protected virtual void Start()
{
    if (string.IsNullOrEmpty(taskId))
        taskId = System.Guid.NewGuid().ToString();
    GameEventBus.Subscribe<GameStartEvent>(OnGameStarted);
    if (debugMode) Debug.Log($"[{GetType().Name}] Started: {taskName}, ID: {taskId}");
}

private void OnDestroy()
{
    GameEventBus.Unsubscribe<GameStartEvent>(OnGameStarted);
}

private void OnGameStarted(GameStartEvent e) => _gameStarted = true;

protected virtual void Update()
{
    if (!_gameStarted) return;
    if (!taskCompleted && CheckTriggerCondition())
    {
        if (!taskStarted) StartTask();
        else UpdateTaskProgress();
    }
}
```

### 变更 3：GameEventBus — 场景卸载时清理静态订阅

在 `GameSceneManager` 的场景卸载前调用 `GameEventBus.ClearAllSubscriptions()`，防止已销毁对象的订阅残留。

**注意**：`ClearAllSubscriptions` 会清除所有订阅，包括 `TaskManager`、`PanelManager` 等持久对象的订阅。需要这些对象在清理后重新订阅。

更安全的方案：不清理全部，而是让各对象在 `OnDisable`/`OnDestroy` 中正确取消订阅（现有代码已有此机制）。`GameEventBus` 的静态状态问题主要影响已销毁对象的悬空引用，但 `GameEventBus.Subscribe` 已有重复检查，`Publish` 中的 `try/catch` 也能防止崩溃。

**结论**：变更 3 暂不实施，现有的 `OnDisable`/`OnDestroy` 取消订阅机制已足够。

## 文件变更

| 文件 | 变更 |
|------|------|
| `Assets/Script/UI/MainMenuPanel.cs` | 移除 `OnStartClicked` 中的 `GameStartEvent`；订阅 `SceneLoadedEvent`，在目标场景加载完成后发布 `GameStartEvent` |
| `Assets/Script/Task/Trigger/BaseTaskTrigger.cs` | 增加 `_gameStarted` 守卫；订阅/取消订阅 `GameStartEvent` |

## 关键约束

- `MainMenuPanel` 已在 `OnEnable`/`OnDisable` 中管理订阅，新增 `SceneLoadedEvent` 订阅需遵循同样模式
- `BaseTaskTrigger` 的 `OnDestroy` 目前不存在，需新增（`ScriptTrigger` 已有 `OnDestroy`，基类新增后子类的 `OnDestroy` 需调用 `base.OnDestroy()` 或改为 `virtual`）
- `ScriptTrigger` 已有自己的 `OnDestroy`，需确保基类的取消订阅也被执行（改为 `protected virtual void OnDestroy()`，`ScriptTrigger` 调用 `base.OnDestroy()`）
