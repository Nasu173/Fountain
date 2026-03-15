# 实现任务

## Task 1: 修改 SceneEventList.cs — LoadSceneEvent 新增 UnloadAll 和 ScenesToKeep ✓
- 文件: `Assets/Script/Scene/SceneEventList.cs`
- `LoadSceneEvent` 新增：`public bool UnloadAll;` 和 `public string[] ScenesToKeep;`

## Task 2: 修改 GameSceneManager.cs — LoadRoutine 支持 UnloadAll ✓
- 文件: `Assets/Script/Scene/GameSceneManager.cs`
- 在 `LoadRoutine` 末尾，原 `if (e.Additive && !string.IsNullOrEmpty(e.SceneToUnload))` 改为：
  - `if (e.Additive && e.UnloadAll)`：遍历所有已加载场景，卸载不等于 `e.SceneAddress` 且不在 `e.ScenesToKeep` 中的场景
  - `else if (e.Additive && !string.IsNullOrEmpty(e.SceneToUnload))`：保持原有单场景卸载逻辑

## Task 3: 修改 PanelManager.cs — OnMenuClicked 使用 UnloadAll ✓
- 文件: `Assets/Script/Manager/PanelManager.cs`
- 新增 `[SerializeField] private string[] _scenesToKeep`
- `OnMenuClicked` 的 `LoadSceneEvent` 增加 `UnloadAll = true, ScenesToKeep = _scenesToKeep`
