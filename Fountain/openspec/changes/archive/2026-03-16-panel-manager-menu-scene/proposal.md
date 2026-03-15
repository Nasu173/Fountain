# PanelManager 主菜单按钮场景切换

## 问题/需求

`PanelManager.OnMenuClicked` 目前只发布 `MenuEvent`，没有实际切换场景的逻辑。需要让它能切换至指定场景（如主菜单场景），切换方式与开始游戏一致：Additive 加载目标场景，同时卸载当前场景。

## 目标

修改 `PanelManager.OnMenuClicked`，使其通过 `LoadSceneEvent`（Additive 模式）切换至可在 Inspector 中配置的目标场景，并恢复游戏时间与鼠标状态。

## 方案

- `PanelManager` 新增 `[SerializeField] string _menuSceneAddress` 字段
- `OnMenuClicked` 改为发布 `LoadSceneEvent { SceneAddress = _menuSceneAddress, Additive = true, SceneToUnload = gameObject.scene.name }`，并恢复 `Time.timeScale = 1f` 和显示鼠标
