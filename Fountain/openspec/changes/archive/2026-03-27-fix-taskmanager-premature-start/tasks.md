# 实现任务

## Task 1: ✅ 修改 MainMenuPanel — 延迟 GameStartEvent 到场景加载完成后

- 路径: `Assets/Script/UI/MainMenuPanel.cs`
- 在 `OnEnable` 中新增订阅 `SceneLoadedEvent`
- 在 `OnDisable` 中新增取消订阅 `SceneLoadedEvent`
- 新增私有字段 `private bool _waitingForScene = false`
- 修改 `OnStartClicked()`：移除 `GameEventBus.Publish(new GameStartEvent())`，改为设置 `_waitingForScene = true`
- 新增 `OnSceneLoaded(SceneLoadedEvent e)` 处理器：若 `_waitingForScene` 为 true，重置标志并发布 `GameStartEvent`

```csharp
// OnEnable 新增
GameEventBus.Subscribe<SceneLoadedEvent>(OnSceneLoaded);

// OnDisable 新增
GameEventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoaded);

// OnStartClicked 修改：移除 GameEventBus.Publish(new GameStartEvent())，新增：
_waitingForScene = true;

// 新增方法
private void OnSceneLoaded(SceneLoadedEvent e)
{
    if (!_waitingForScene) return;
    _waitingForScene = false;
    GameEventBus.Publish(new GameStartEvent());
}
```

## Task 2: ✅ 修改 BaseTaskTrigger — 增加游戏已开始守卫

- 路径: `Assets/Script/Task/Trigger/BaseTaskTrigger.cs`
- 新增私有字段 `private bool _gameStarted = false`
- 在 `Start()` 中新增 `GameEventBus.Subscribe<GameStartEvent>(OnGameStarted)`
- 新增 `protected virtual void OnDestroy()` 方法，取消订阅 `GameStartEvent`
- 新增私有方法 `private void OnGameStarted(GameStartEvent e) => _gameStarted = true`
- 修改 `Update()`：在方法开头增加 `if (!_gameStarted) return;`

```csharp
// 新增字段
private bool _gameStarted = false;

// Start() 末尾新增
GameEventBus.Subscribe<GameStartEvent>(OnGameStarted);

// 新增方法
private void OnGameStarted(GameStartEvent e) => _gameStarted = true;

protected virtual void OnDestroy()
{
    GameEventBus.Unsubscribe<GameStartEvent>(OnGameStarted);
}

// Update() 开头新增守卫
protected virtual void Update()
{
    if (!_gameStarted) return;
    if (!taskCompleted && CheckTriggerCondition()) { ... }
}
```

## Task 3: ✅ 修改 ScriptTrigger — 调用 base.OnDestroy()

- 路径: `Assets/Script/Task/Trigger/ScriptTrigger.cs`
- `ScriptTrigger` 已有 `private void OnDestroy()`，需改为 `protected override void OnDestroy()`
- 在方法末尾调用 `base.OnDestroy()`，确保基类的 `GameStartEvent` 取消订阅也被执行

```csharp
// 修改前
private void OnDestroy()
{
    GameEventBus.Unsubscribe<ScriptTriggerEvent>(OnScriptTriggerEvent);
}

// 修改后
protected override void OnDestroy()
{
    GameEventBus.Unsubscribe<ScriptTriggerEvent>(OnScriptTriggerEvent);
    base.OnDestroy();
}
```

## Task 4: ✅ 验证编译与行为

- 检查 Unity Console 无编译错误
- 验证点击"开始游戏"后，任务触发器在场景加载完成前不响应任何触发条件
- 验证 `GameStartEvent` 在游戏场景 `Start()` 执行完毕后才发布（通过 Console 日志确认顺序）
- 验证任务链（nextTaskTrigger）仍正常工作
- 验证从游戏返回主菜单再重新开始时，任务不会被提前触发
