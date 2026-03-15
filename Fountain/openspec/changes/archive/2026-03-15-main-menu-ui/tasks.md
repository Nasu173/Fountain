# 实现任务

## Task 1: 创建 MainMenuPanel.cs ✓
- 路径: `Assets/Script/UI/MainMenuPanel.cs`
- 命名空间: `Foutain.UI`
- 继承 `MonoBehaviour`
- `[SerializeField] string _gameSceneAddress` — Inspector 可配置的目标场景地址
- `OnStartClicked()`：发布 `LoadSceneEvent { SceneAddress = _gameSceneAddress }`
- `OnSettingClicked()`：发布 `SettingEvent()`
- `OnQuitClicked()`：调用 `Application.Quit()`
- 三个方法均为 `public`，供 Button `OnClick` 绑定
