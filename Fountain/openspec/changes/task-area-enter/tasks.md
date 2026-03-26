# 实现任务

## Task 1: ✅ 创建 TaskAreaEnter.cs

- 路径: `Assets/Script/Task/TaskAreaEnter.cs`
- 继承 `MonoBehaviour`
- Inspector 字段：
  - `[SerializeField] BaseTaskTrigger taskTrigger`
  - `[SerializeField] string playerTag = "Player"`
  - `[SerializeField] bool canTriggerMultipleTimes = false`
  - `[SerializeField] int progressPerEnter = 1`
  - `[SerializeField] bool showDebug = true`
- 私有字段：`bool hasTriggered = false`
- `OnTriggerEnter(Collider other)`：
  1. `if (!other.CompareTag(playerTag)) return`
  2. `if (!canTriggerMultipleTimes && hasTriggered) return`
  3. `if (taskTrigger == null)` → LogWarning + return
  4. 若 `!taskTrigger.TaskStarted`，发布 `TaskStartEvent`（从 trigger 读取所有字段）
  5. 发布 `TaskProgressEvent { TaskId = taskTrigger.TaskId, Amount = progressPerEnter }`
  6. `hasTriggered = true`
