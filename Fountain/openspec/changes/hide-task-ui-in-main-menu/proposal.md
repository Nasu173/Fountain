# 任务UI在主菜单时隐藏

## 问题/需求

`TaskManager` 是 `DontDestroyOnLoad` 单例，其 `taskUIContainer`（任务UI容器）在整个游戏生命周期中持续存在。当玩家从游戏场景返回主菜单时，若有任务UI仍在显示（或新任务被触发），任务UI会叠加在主菜单界面上，破坏主菜单的视觉体验。

## 目标

- 进入主菜单时隐藏任务UI容器
- 离开主菜单（游戏开始）时恢复任务UI容器显示
- 不影响任务系统的数据状态（任务数据保持不变，仅控制UI可见性）

## 方案

`TaskManager` 订阅 `GameStartEvent` 和 `MenuEvent`（或监听主菜单状态），在主菜单激活时隐藏 `taskUIContainer`，在游戏开始时显示。

由于项目已有 `GameStartEvent`（游戏开始）事件，且主菜单通过 `LoadSceneEvent` 加载，可以通过监听这两个事件来控制容器的显示/隐藏。

最简方案：`TaskManager` 订阅 `GameStartEvent`（显示容器）和 `MenuEvent`（隐藏容器）。`MenuEvent` 在主菜单场景加载时发布，`GameStartEvent` 在游戏场景加载完成后发布。
