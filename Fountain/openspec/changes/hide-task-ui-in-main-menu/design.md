# 设计方案

## 架构概览

```
MainMenuPanel.OnEnable()
    │  发布 MenuEvent
    ▼
TaskManager（订阅 MenuEvent，隐藏 taskUIContainer）

MainMenuPanel.OnStartClicked() → SceneLoadedEvent → GameStartEvent
    ▼
TaskManager（订阅 GameStartEvent，显示 taskUIContainer）
```

## 分析

`TaskManager` 是 `DontDestroyOnLoad` 单例，持有 `taskUIContainer`（`Transform`）。控制其显示/隐藏只需切换 `taskUIContainer.gameObject.SetActive(bool)`。

**触发时机：**

- **隐藏**：`MainMenuPanel.OnEnable()` 在主菜单面板激活时调用，此时应隐藏任务UI。`MenuEvent` 已在 `GameEventList.cs` 中定义但从未被发布，可在此处发布。
- **显示**：`GameStartEvent` 在场景加载完成后由 `MainMenuPanel.OnSceneLoaded` 发布，此时游戏已开始，任务UI应可见。

**为何不用 `SceneLoadedEvent`：**
`SceneLoadedEvent` 在每次场景加载完成时都会触发（包括小游戏场景切换），需要额外判断场景地址。`MenuEvent` 语义更明确。

## 变更

### 变更 1：MainMenuPanel — 在 OnEnable 中发布 MenuEvent

```csharp
private void OnEnable()
{
    // 现有代码...
    GameEventBus.Publish(new MenuEvent());  // ← 新增
    SetMainMenuState(true);
}
```

### 变更 2：TaskManager — 订阅 MenuEvent 和 GameStartEvent

```csharp
private void OnEnable()
{
    GameEventBus.Subscribe<TaskStartEvent>(OnTaskStart);
    GameEventBus.Subscribe<TaskProgressEvent>(OnTaskProgress);
    GameEventBus.Subscribe<MenuEvent>(OnMenuEvent);       // ← 新增
    GameEventBus.Subscribe<GameStartEvent>(OnGameStart);  // ← 新增
}

private void OnDisable()
{
    GameEventBus.Unsubscribe<TaskStartEvent>(OnTaskStart);
    GameEventBus.Unsubscribe<TaskProgressEvent>(OnTaskProgress);
    GameEventBus.Unsubscribe<MenuEvent>(OnMenuEvent);       // ← 新增
    GameEventBus.Unsubscribe<GameStartEvent>(OnGameStart);  // ← 新增
}

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

## 文件变更

| 文件 | 变更 |
|------|------|
| `Assets/Script/UI/MainMenuPanel.cs` | `OnEnable` 中新增 `GameEventBus.Publish(new MenuEvent())` |
| `Assets/Script/Task/TaskManager.cs` | 订阅/取消订阅 `MenuEvent` 和 `GameStartEvent`；新增两个处理方法 |

## 关键约束

- `taskUIContainer` 可能为 null（Inspector 未赋值），需做 null 检查
- `MenuEvent` 在 `MainMenuPanel.OnEnable` 中发布，早于 `SetMainMenuState(true)`，顺序无影响
- 初始状态：游戏启动时若先进入主菜单，`taskUIContainer` 默认激活状态由 Prefab/Scene 决定；`MenuEvent` 会在主菜单面板激活时立即隐藏它，无需额外初始化逻辑
