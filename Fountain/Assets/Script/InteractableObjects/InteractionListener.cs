using UnityEngine;
using Foutain.Player;

public class InteractionListener : MonoBehaviour
{
    [Header("监听设置")]
    [Tooltip("与此物体关联的任务触发器ID")]
    [SerializeField] private string taskTriggerId;

    [Tooltip("自动查找并关联触发器")]
    [SerializeField] private bool autoFindTrigger = true;

    [Tooltip("手动指定的触发器")]
    [SerializeField] private InteractTaskTrigger specificTrigger;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    // 不再强制要求IInteractable接口
    private InteractTaskTrigger taskTrigger;

    private void Start()
    {
        FindTaskTrigger();
    }

    private void FindTaskTrigger()
    {
        if (specificTrigger != null)
        {
            taskTrigger = specificTrigger;
            if (debugMode) Debug.Log($"找到指定的触发器: {taskTrigger.name}");
            return;
        }

        if (autoFindTrigger && !string.IsNullOrEmpty(taskTriggerId))
        {
            // 查找所有InteractTaskTrigger
            InteractTaskTrigger[] triggers = FindObjectsOfType<InteractTaskTrigger>();
            foreach (InteractTaskTrigger trigger in triggers)
            {
                // 可以通过ID或其他方式匹配
                if (trigger.TaskId == taskTriggerId)
                {
                    taskTrigger = trigger;
                    if (debugMode) Debug.Log($"通过ID找到触发器: {trigger.name}");
                    break;
                }
            }
        }

        if (taskTrigger == null && debugMode)
        {
            Debug.Log($"未找到关联的触发器，将通知所有触发器");
        }
    }

    /// <summary>
    /// 由外部脚本在交互时调用这个方法
    /// </summary>
    public void OnInteracted(PlayerInteractor player)
    {
        if (debugMode) Debug.Log($"物体 {gameObject.name} 被交互");

        // 通知关联的触发器
        if (taskTrigger != null)
        {
            taskTrigger.OnObjectInteracted(gameObject);
        }
        else
        {
            // 如果没有直接关联，尝试查找场景中所有触发器
            InteractTaskTrigger[] triggers = FindObjectsOfType<InteractTaskTrigger>();
            if (debugMode) Debug.Log($"找到 {triggers.Length} 个触发器");

            foreach (InteractTaskTrigger trigger in triggers)
            {
                trigger.OnObjectInteracted(gameObject);
            }
        }
    }
}