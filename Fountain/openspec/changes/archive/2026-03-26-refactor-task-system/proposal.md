# 重构任务系统（事件驱动架构）

## 问题/需求

当前任务系统存在以下问题：

1. **三个几乎相同的"可交互对象"类**：`TaskInteractable`、`SimpleTaskInteractable`、`InteractionListener` 重复实现了相同的逻辑（持有触发器引用、构建 InteractConfig、检查 hasInteracted、调用 trigger.OnObjectInteracted）。
2. **反射滥用**：`InteractForTaskSimple` 通过反射访问 `BaseTaskTrigger` 的私有字段 `taskStarted`/`taskCompleted` 和私有方法 `UpdateTaskProgress`，脆弱且难以维护。`BaseTaskTrigger.StartNextTask()` 也通过反射调用下一个触发器的 `protected StartTask()`。
3. **直接耦合**：触发器和完成器直接调用 `TaskManager.Instance.AddTask()` / `UpdateTaskProgress()`，绕过了项目已有的 `GameEventBus` 事件系统。
4. **可变状态暴露**：`TaskManager.GetActiveTasks()` / `GetActiveTaskUIs()` 直接返回内部字典引用，外部可随意修改。

## 目标

- 任务的**开始**和**进度更新**全部通过 `GameEventBus` 事件完成，消除直接耦合。
- 合并三个重复的可交互对象类为一个 `TaskInteractable`。
- 消除所有反射调用。
- 保留现有 UI 结构（`TaskUI` prefab 层级不变）和 `TaskData` 字段（taskName、targetCount、currentCount、description、isCompleted）。
- 保留任务链（nextTaskTrigger）功能。

## 方案

新增两个事件类到 `GameEventList.cs`：
- `TaskStartEvent`：触发器广播，携带任务全部信息（id、name、number、targetCount、description）
- `TaskProgressEvent`：完成器广播，携带 taskId 和 amount

`TaskManager` 订阅这两个事件，负责 UI 创建和进度更新逻辑（原有逻辑不变，只是入口改为事件）。

触发器（`BaseTaskTrigger` 及子类）在需要开始任务时发布 `TaskStartEvent`，在需要更新进度时发布 `TaskProgressEvent`。

可交互对象统一使用单一的 `TaskInteractable` 组件，发布 `TaskProgressEvent`（任务未开始时先发布 `TaskStartEvent`）。

删除冗余类：`SimpleTaskInteractable`、`InteractionListener`、`InteractForTaskSimple`。
