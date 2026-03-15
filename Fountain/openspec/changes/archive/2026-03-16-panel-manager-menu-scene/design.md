# 设计方案

## 变更：PanelManager.cs

- 新增字段：`[SerializeField] private string _menuSceneAddress`
- `OnMenuClicked` 替换为：
  1. `Time.timeScale = 1f`（恢复时间，避免场景切换后时间仍为 0）
  2. `GameInputManager.Instance.ShowCursor()`（保持鼠标可见）
  3. `GameEventBus.Publish(new LoadSceneEvent { SceneAddress = _menuSceneAddress, Additive = true, SceneToUnload = gameObject.scene.name })`

## 文件路径

- `Assets/Script/Manager/PanelManager.cs`
