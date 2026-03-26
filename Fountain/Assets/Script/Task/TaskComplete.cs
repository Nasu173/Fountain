using UnityEngine;
using Fountain.Player;
using Fountain.Common;
public class TaskComplete : MonoBehaviour
{
    [Header("任务配置")]
    [SerializeField] private BaseTaskTrigger taskTrigger;

    [Header("调试")]
    [SerializeField] private bool showDebug = true;

    [SerializeField] private int progressPerInteract;

    public void UpdateTaskProgress()
    {
        if (taskTrigger == null)
        {
            if (showDebug) Debug.LogWarning($"[{gameObject.name}] 未关联 taskTrigger");
            return;
        }

        if (!taskTrigger.TaskStarted)
        {
            GameEventBus.Publish(new TaskStartEvent
            {
                TaskId = taskTrigger.TaskId,
                TaskName = taskTrigger.TaskName,
                TaskNumber = taskTrigger.TaskNumber,
                TargetCount = taskTrigger.TargetCount,
                Description = taskTrigger.Description
            });
        }

        GameEventBus.Publish(new TaskProgressEvent
        {
            TaskId = taskTrigger.TaskId,
            Amount = progressPerInteract
        });
    }

    public string GetTaskId() => taskTrigger != null ? taskTrigger.TaskId : null;
}
