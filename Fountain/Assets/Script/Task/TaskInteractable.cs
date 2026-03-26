using UnityEngine;
using Fountain.Player;
using Fountain.Common;

/// <summary>
/// 可交互任务对象（统一实现，替代原 TaskInteractable / SimpleTaskInteractable / InteractionListener）
/// 玩家交互时通过 GameEventBus 发布任务事件
/// </summary>
public class TaskInteractable : MonoBehaviour, IInteractable, ITaskInteractable
{
    [Header("任务配置")]
    [SerializeField] private BaseTaskTrigger taskTrigger;

    [Header("交互设置")]
    [SerializeField] private InteractConfig config = new InteractConfig();

    [Header("视觉效果")]
    [SerializeField] private OutlineVisual outlineVisual;

    [Header("调试")]
    [SerializeField] private bool showDebug = true;

    private bool hasInteracted = false;

    private void Awake()
    {
        if (outlineVisual == null)
            outlineVisual = GetComponent<OutlineVisual>();

        if (outlineVisual != null)
            outlineVisual.SetOutline(false);
    }

    public void InteractWith(PlayerInteractor player)
    {
        if (!config.canInteractMultipleTimes && hasInteracted)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已交互过");
            return;
        }

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
            Amount = config.progressPerInteract
        });

        hasInteracted = true;

        if (config.destroyOnInteract)
            Destroy(gameObject);
    }

    public void Select()
    {
        if (!config.canInteractMultipleTimes && hasInteracted) return;
        outlineVisual?.SetOutline(true);
    }

    public void Deselect()
    {
        outlineVisual?.SetOutline(false);
    }

    public string GetTaskId() => taskTrigger != null ? taskTrigger.TaskId : null;
    public InteractConfig GetInteractConfig() => config;
}
