# 设计方案

## 架构

一个 C# 脚本，放在 `Assets/Script/UI/` 目录：

### MainMenuPanel.cs
- 继承 `MonoBehaviour`
- `[SerializeField] string _gameSceneAddress` — 目标场景的 Addressable 地址，可在 Inspector 中设置
- 三个公开方法，绑定到对应 Button 的 `OnClick`：
  - `OnStartClicked()`：`GameEventBus.Publish(new LoadSceneEvent { SceneAddress = _gameSceneAddress })`
  - `OnSettingClicked()`：`GameEventBus.Publish(new SettingEvent())`
  - `OnQuitClicked()`：`Application.Quit()`

## 依赖

- `LoadSceneEvent`（`Foutain.Scene`）：由 `GameSceneManager` 处理，执行 Addressable 场景切换
- `SettingEvent`（全局）：由场景中的 `PanelManager` 处理，唤出设置面板

## 文件路径

- `Assets/Script/UI/MainMenuPanel.cs`
