# Addressable 场景管理系统

## 问题/需求

游戏需要在多个场景之间切换（主菜单、游戏场景、过场等），当前缺乏统一的场景管理机制。直接使用 `SceneManager.LoadScene` 会导致场景名称硬编码散落各处，难以维护，且无法利用 Addressable 的异步加载与内存管理优势。

## 目标

实现一套基于 Addressable 的场景管理系统，支持异步加载/卸载场景，通过事件总线解耦调用方与场景管理器，加载过程可扩展（如接入 Loading UI）。

## 方案

- `SceneLoader`：核心加载器，封装 Addressable 异步加载/卸载逻辑
- `SceneManager`（单例）：对外接口，接收切换请求，协调加载流程
- `SceneEventList`：定义场景相关事件（请求切换、加载进度、加载完成）
- 调用方通过 `GameEventBus.Publish<LoadSceneEvent>()` 发起切换，与 SceneManager 完全解耦
