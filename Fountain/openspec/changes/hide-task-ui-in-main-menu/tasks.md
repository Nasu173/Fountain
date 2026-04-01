# 实现任务

## Task 1: ✅ 修改 MainMenuPanel — 在 OnEnable 中发布 MenuEvent

- 路径: `Assets/Script/UI/MainMenuPanel.cs`
- 在 `OnEnable()` 方法中，`SetMainMenuState(true)` 之前新增一行：

```csharp
GameEventBus.Publish(new MenuEvent());
```

## Task 2: ✅ 修改 TaskManager — 订阅 MenuEvent 和 GameStartEvent 控制容器显示

- 路径: `Assets/Script/Task/TaskManager.cs`
- 在 `OnEnable()` 中新增两行订阅：

```csharp
GameEventBus.Subscribe<MenuEvent>(OnMenuEvent);
GameEventBus.Subscribe<GameStartEvent>(OnGameStart);
```

- 在 `OnDisable()` 中新增两行取消订阅：

```csharp
GameEventBus.Unsubscribe<MenuEvent>(OnMenuEvent);
GameEventBus.Unsubscribe<GameStartEvent>(OnGameStart);
```

- 新增两个私有方法：

```csharp
private void OnMenuEvent(MenuEvent e)
{
    if (taskUIContainer != null)
        taskUIContainer.gameObject.SetActive(false);
}

private void OnGameStart(GameStartEvent e)
{
    if (taskUIContainer != null)
        taskUIContainer.gameObject.SetActive(true);
}
```

## Task 3: ✅ 验证

- 检查 Unity Console 无编译错误
- 验证进入主菜单时任务UI容器不可见
- 验证点击"开始游戏"后任务UI容器恢复可见
- 验证从游戏暂停菜单返回主菜单时任务UI也被隐藏
