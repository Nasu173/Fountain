# OnMenuClicked 卸载所有无关场景

## 问题/需求

`OnMenuClicked` 当前使用 `LoadSceneEvent { Additive = true, SceneToUnload = gameObject.scene.name }`，只卸载当前场景（游戏场景）。但游戏运行时可能同时加载了多个场景（如关卡子场景、UI 场景等），返回主菜单时需要卸载除指定保留场景以外的所有场景。

## 目标

扩展 `LoadSceneEvent` 支持"卸载全部，保留指定场景"模式，`GameSceneManager` 在加载完成后遍历所有已加载场景并卸载不在保留列表中的场景，`OnMenuClicked` 使用该模式。

## 方案

- `LoadSceneEvent` 新增 `bool UnloadAll` 和 `string[] ScenesToKeep` 字段
- `GameSceneManager.LoadRoutine` 中：若 `UnloadAll == true`，加载完成后遍历 `SceneManager.sceneCount`，卸载所有不在 `ScenesToKeep` 且不是刚加载的目标场景的场景
- `PanelManager.OnMenuClicked` 改用 `UnloadAll = true`，`ScenesToKeep` 由 Inspector 配置
