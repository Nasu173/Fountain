# 实现任务

## Task 1: 修改 PanelManager.cs — OnMenuClicked 切换场景 ✓
- 文件: `Assets/Script/Manager/PanelManager.cs`
- 新增 `[SerializeField] private string _menuSceneAddress`
- `OnMenuClicked` 替换为：
  - `Time.timeScale = 1f`
  - `GameInputManager.Instance.ShowCursor()`
  - 发布 `LoadSceneEvent { SceneAddress = _menuSceneAddress, Additive = true, SceneToUnload = gameObject.scene.name }`
- 添加 `using Foutain.Scene;`
