# 主菜单 UI 系统

## 问题/需求

游戏需要一个主菜单场景，包含三个核心按钮：开始游戏、设置、退出游戏。当前缺乏对应的 UI 逻辑脚本。

## 目标

实现主菜单的按钮交互逻辑：
1. 开始游戏：通过 Addressable 切换至指定场景（场景地址可在 Inspector 中配置）
2. 设置：复用游戏内的 `SettingEvent` 事件总线机制唤出设置面板
3. 退出游戏：调用 `Application.Quit()`

## 方案

- `MainMenuPanel`：主菜单面板，持有三个按钮的点击处理方法，通过 `GameEventBus` 发布事件或直接调用场景切换
- 场景切换复用已有的 `LoadSceneEvent` + `GameSceneManager` 管道
- 设置面板复用已有的 `SettingEvent` 机制，与游戏内唤出方式完全一致
