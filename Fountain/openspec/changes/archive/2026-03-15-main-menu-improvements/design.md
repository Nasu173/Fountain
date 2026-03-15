# 设计方案

## 变更一：场景叠加加载

### LoadSceneEvent（修改）
新增 `bool Additive` 字段（默认 `false`）：
- `false`：保持原有 `LoadSceneMode.Single` 行为
- `true`：使用 `LoadSceneMode.Additive` 加载新场景，加载完成后卸载 `SceneToUnload` 指定的场景

新增 `string SceneToUnload` 字段：Additive 模式下，加载完成后要卸载的场景名称（Unity 场景名，非 Addressable 地址）。

### SceneLoader（修改）
`LoadSceneAsync` 增加 `LoadSceneMode mode` 参数，传入 `Addressables.LoadSceneAsync`。

### GameSceneManager（修改）
`LoadRoutine` 中：
- 根据 `e.Additive` 决定 `LoadSceneMode`
- 若 `e.Additive == true` 且 `e.SceneToUnload` 非空，加载完成后调用 `UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(e.SceneToUnload)`

### MainMenuPanel（修改）
`OnStartClicked` 改为发布：
```csharp
new LoadSceneEvent { SceneAddress = _gameSceneAddress, Additive = true, SceneToUnload = gameObject.scene.name }
```

## 变更二：设置面板返回联动

### GameEventList（修改）
新增事件：
```csharp
public class SettingBackEvent : IGameEvent { }
```

### PanelManager（修改）
`OnBackClicked` 末尾追加：
```csharp
GameEventBus.Publish(new SettingBackEvent());
```

### MainMenuPanel（修改）
- `OnEnable`：订阅 `SettingBackEvent`
- `OnDisable`：取消订阅
- `OnSettingClicked`：末尾追加 `gameObject.SetActive(false)`
- 新增处理方法 `OnSettingBack(SettingBackEvent e)`：`gameObject.SetActive(true)`

## 文件变更清单

- `Assets/Script/Scene/SceneEventList.cs` — `LoadSceneEvent` 新增字段
- `Assets/Script/Scene/SceneLoader.cs` — `LoadSceneAsync` 增加 mode 参数
- `Assets/Script/Scene/GameSceneManager.cs` — `LoadRoutine` 支持 Additive + 卸载
- `Assets/Script/UI/MainMenuPanel.cs` — 订阅 `SettingBackEvent`，修改 `OnStartClicked` 和 `OnSettingClicked`
- `Assets/Script/GameEvent/GameEventList.cs` — 新增 `SettingBackEvent`
- `Assets/Script/Manager/PanelManager.cs` — `OnBackClicked` 发布 `SettingBackEvent`
