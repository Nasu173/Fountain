# 返回游戏时清除旧任务进度

## 问题/需求

`TaskManager` 是 `DontDestroyOnLoad` 单例，`activeTasks` 和 `activeTaskUIs` 字典在整个游戏生命周期中持续存在。当玩家从游戏场景返回主菜单后再次进入游戏时，旧任务数据和 UI 仍然保留，新触发的同名任务会因 `ContainsKey` 检查被跳过，或与旧任务同时存在，导致多任务重叠的问题。

## 目标

- 返回主菜单时清除所有活跃任务数据和对应 UI
- 再次进入游戏时任务系统从干净状态开始
- 不影响任务系统的其他功能

## 方案

在 `TaskManager` 已有的 `OnMenuEvent` 方法中，除了隐藏 `taskUIContainer`，同时销毁所有活跃任务 UI 并清空 `activeTasks` 和 `activeTaskUIs` 字典。

这是最小改动方案：复用已有的 `MenuEvent` 订阅，在同一处理函数中追加清理逻辑。
