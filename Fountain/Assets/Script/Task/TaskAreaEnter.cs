using UnityEngine;

/// <summary>
/// 进入区域时自动推进任务进度
/// 挂载在带有 Trigger Collider 的 GameObject 上
/// </summary>
public class TaskAreaEnter : MonoBehaviour
{
    [Header("任务配置")]
    [SerializeField] private BaseTaskTrigger taskTrigger;

    [Header("触发设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool canTriggerMultipleTimes = false;
    [SerializeField] private int progressPerEnter = 1;

    [Header("调试")]
    [SerializeField] private bool showDebug = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (!canTriggerMultipleTimes && hasTriggered) return;

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
            Amount = progressPerEnter
        });

        hasTriggered = true;
    }
}
