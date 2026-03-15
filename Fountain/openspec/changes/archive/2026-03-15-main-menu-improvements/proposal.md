# 主菜单改进：场景叠加加载 & 设置面板联动

## 问题/需求

1. **场景切换**：当前 `SceneLoader` 使用 `LoadSceneMode.Single`，会卸载所有已加载场景。主菜单场景需要只卸载自身，保留其他已加载场景（如持久化场景）。

2. **设置面板联动**：点击设置按钮后，主菜单 Panel 应隐藏；当设置面板的"返回"按钮（`OnBackClicked`）触发时，主菜单 Panel 应重新显示。当前 `PanelManager.OnBackClicked` 没有发布任何事件，`MainMenuPanel` 无法感知。

## 目标

1. 修改 `LoadSceneEvent` 支持 `Additive` 模式，`GameSceneManager` 在加载新场景后卸载指定的旧场景
2. `MainMenuPanel.OnSettingClicked` 隐藏自身 GameObject；新增 `SettingBackEvent` 事件，`PanelManager.OnBackClicked` 发布该事件，`MainMenuPanel` 订阅后重新显示自身
