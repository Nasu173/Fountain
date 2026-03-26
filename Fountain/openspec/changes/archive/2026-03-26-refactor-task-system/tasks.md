# 实现任务

## Task 1: ✅ 在 GameEventList.cs 中新增任务事件

- 路径: `Assets/Script/GameEvent/GameEventList.cs`
- 在文件末尾新增两个事件类：
  ```csharp
  [System.Serializable]
  public class TaskStartEvent : IGameEvent
  {
      public string TaskId;
      public string TaskName;
      public string TaskNumber;
      public int TargetCount;
      public string Description;
  }

  [System.Serializable]
  public class TaskProgressEvent : IGameEvent
  {
      public string TaskId;
      public int Amount;
  }
  ```

## Task 2: ✅ 重构 BaseTaskTrigger.cs

- 路径: `Assets/Script/Task/Trigger/BaseTaskTrigger.cs`
- `StartTask()` 改为发布 `TaskStartEvent`（不再调用 `TaskManager.Instance.AddTask()`）
- `UpdateTaskProgress()` 改为发布 `TaskProgressEvent`（不再调用 `TaskManager.Instance.UpdateTaskProgress()`）
- 新增 `public void TriggerStart()` 方法（供任务链和外部调用，替代反射）：
  ```csharp
  public void TriggerStart() { if (!taskStarted) StartTask(); }
  ```
- `StartNextTask()` 去掉反射，改为 `nextTaskTrigger.TriggerStart()`
- 保留所有 `[SerializeField]` 字段、任务链配置、`debugMode`

## Task 3: ✅ 重构 TaskManager.cs

- 路径: `Assets/Script/Task/TaskManager.cs`
- 在 `OnEnable()` 中订阅 `TaskStartEvent` 和 `TaskProgressEvent`
- 在 `OnDisable()` 中取消订阅
- 将 `AddTask()` 改为 `private void OnTaskStart(TaskStartEvent e)` 处理器（逻辑不变）
- 将 `UpdateTaskProgress()` 改为 `private void OnTaskProgress(TaskProgressEvent e)` 处理器（逻辑不变）
- `GetActiveTasks()` 改为返回 `new Dictionary<string, TaskData>(activeTasks)`（只读副本）
- `GetActiveTaskUIs()` 改为返回 `new Dictionary<string, TaskUI>(activeTaskUIs)`（只读副本）
- 保留单例模式、`DontDestroyOnLoad`、`taskUIPrefab`、`taskUIContainer`、`debugMode`

## Task 4: ✅ 重构 TaskInteractable.cs（合并三个重复类）

- 路径: `Assets/Script/Task/TaskInteractable.cs`
- 实现 `IInteractable` 接口（`InteractWith`、`Select`、`Deselect`）
- `[SerializeField] BaseTaskTrigger taskTrigger` — 关联触发器（用于读取任务信息）
- `[SerializeField] InteractConfig config` — 直接使用 `InteractConfig` 作为 Inspector 字段（不再展开为多个 bool/int）
- `[SerializeField] OutlineVisual outlineVisual` — 轮廓效果
- 交互逻辑（`InteractWith`）：
  1. 检查 `hasInteracted` 和 `config.canInteractMultipleTimes`
  2. 若 `taskTrigger != null && !taskTrigger.taskStarted`，发布 `TaskStartEvent`（从 trigger 读取信息）
  3. 发布 `TaskProgressEvent`（taskId = `taskTrigger.TaskId`，amount = `config.progressPerInteract`）
  4. 标记 `hasInteracted = true`
  5. 若 `config.destroyOnInteract`，销毁 gameObject
- `Select()` / `Deselect()` 控制 `outlineVisual`（若有）
- 注意：`taskTrigger.taskStarted` 需要改为 `public bool TaskStarted => taskStarted`（在 BaseTaskTrigger 中新增属性）

## Task 5: ✅ 在 BaseTaskTrigger.cs 中新增 TaskStarted 属性

- 路径: `Assets/Script/Task/Trigger/BaseTaskTrigger.cs`
- 新增只读属性：`public bool TaskStarted => taskStarted;`
- 此属性供 `TaskInteractable` 判断任务是否已开始，避免重复发布 `TaskStartEvent`

## Task 6: ✅ 删除冗余文件

- 删除 `Assets/Script/Task/SimpleTaskInteractable.cs`
- 删除 `Assets/Script/Task/InteractForTaskSimple.cs`
- 删除 `Assets/Script/InteractableObjects/InteractionListener.cs`
- 删除前确认场景中没有 GameObject 仍在使用这些组件（如有，替换为 `TaskInteractable`）

## Task 7: ✅ 验证编译

- 检查 Unity Console 无编译错误
- 确认 `InteractTaskTrigger` 仍正常工作（它继承 `BaseTaskTrigger`，无需修改）
- 确认任务链（nextTaskTrigger）通过 `TriggerStart()` 正常触发
