# TaskAreaEnter：进入区域推进任务进度

## 问题/需求

现有 `TaskInteractable` 通过玩家主动按键交互来推进任务进度。但部分任务需要玩家**走进某个区域**时自动推进进度（如到达指定地点、进入触发区域），无需按键交互。

## 目标

仿照 `TaskInteractable` 的事件驱动模式，实现 `TaskAreaEnter` 脚本：挂载在带有 Trigger Collider 的 GameObject 上，当玩家进入碰撞体时自动发布 `TaskStartEvent`（首次）和 `TaskProgressEvent`，逻辑与 `TaskInteractable` 保持一致。

## 方案

- 新建 `TaskAreaEnter.cs`，挂载在带 Trigger Collider 的 GameObject 上
- 使用 `OnTriggerEnter` 检测玩家进入（通过 Tag 或 Layer 过滤）
- 进入时发布 `TaskStartEvent`（若任务未开始）+ `TaskProgressEvent`
- 支持 `canTriggerMultipleTimes`（是否允许多次触发）和 `progressPerEnter`（每次进度值）
- 不需要 `IInteractable` 接口（无需玩家主动按键）
