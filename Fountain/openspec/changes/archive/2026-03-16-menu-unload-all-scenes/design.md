# 设计方案

## 变更一：SceneEventList.cs — LoadSceneEvent 新增字段

```csharp
/// <summary>true = 加载完成后卸载所有不在 ScenesToKeep 中的场景</summary>
public bool UnloadAll;
/// <summary>UnloadAll 模式下要保留的场景名称列表</summary>
public string[] ScenesToKeep;
```

## 变更二：GameSceneManager.cs — LoadRoutine 支持 UnloadAll

加载完成后，若 `e.UnloadAll == true`：
1. 收集当前所有已加载场景名（`SceneManager.sceneCount` 遍历）
2. 对每个场景：若场景名不等于 `e.SceneAddress`（刚加载的目标场景）且不在 `e.ScenesToKeep` 中，则调用 `SceneManager.UnloadSceneAsync(scene)`
3. 原有 `e.SceneToUnload` 单场景卸载逻辑保持不变（`else if`）

## 变更三：PanelManager.cs — OnMenuClicked 改用 UnloadAll

- 新增 `[SerializeField] private string[] _scenesToKeep`（替代原 `_menuSceneAddress` 中隐含的单场景逻辑，两者共存）
- `OnMenuClicked` 改为：
  ```csharp
  GameEventBus.Publish(new LoadSceneEvent
  {
      SceneAddress = _menuSceneAddress,
      Additive = true,
      UnloadAll = true,
      ScenesToKeep = _scenesToKeep
  });
  ```

## 文件路径

- `Assets/Script/Scene/SceneEventList.cs`
- `Assets/Script/Scene/GameSceneManager.cs`
- `Assets/Script/Manager/PanelManager.cs`
