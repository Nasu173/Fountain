using UnityEngine;
using Foutain.Player;
using System;

public class InteractionListener : MonoBehaviour
{
    [Header("监听设置")]
    [Tooltip("与此物体关联的任务触发器ID")]
    [SerializeField] private string taskTriggerId;

    [Tooltip("自动查找并关联触发器")]
    [SerializeField] private bool autoFindTrigger = true;

    [Tooltip("手动指定的触发器")]
    [SerializeField] private InteractTaskTrigger specificTrigger;

    [Header("交互配置")]
    [Tooltip("是否能多次交互")]
    [SerializeField] private bool canInteractMultipleTimes = false;

    [Tooltip("是否仅首次交互增加进度")]
    [SerializeField] private bool progressOnlyFirstTime = true;

    [Tooltip("交互后是否销毁物体")]
    [SerializeField] private bool destroyOnInteract = false;

    [Tooltip("每次交互增加的进度值")]
    [SerializeField] private int progressPerInteract = 1;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private InteractTaskTrigger taskTrigger;
    private InteractConfig config;
    private bool hasInteracted = false;

    private void Awake()
    {
        // 创建交互配置
        config = new InteractConfig
        {
            canInteractMultipleTimes = canInteractMultipleTimes,
            progressOnlyFirstTime = progressOnlyFirstTime,
            destroyOnInteract = destroyOnInteract,
            progressPerInteract = progressPerInteract
        };
    }

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
            InteractTaskTrigger[] triggers = FindObjectsOfType<InteractTaskTrigger>();
            foreach (InteractTaskTrigger trigger in triggers)
            {
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
        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (debugMode) Debug.Log($"物体 {gameObject.name} 被交互");

        // 检查交互限制
        if (!canInteractMultipleTimes && hasInteracted)
        {
            if (debugMode) Debug.Log($"[{gameObject.name}] 已经交互过，无法再次交互");
            return;
        }

        // 通知关联的触发器 - 传递 config 参数
        if (taskTrigger != null)
        {
            taskTrigger.OnObjectInteracted(gameObject, config);
            hasInteracted = true;

            if (debugMode) Debug.Log($"[{gameObject.name}] 触发任务: {taskTrigger.TaskName}");
        }
        else
        {
            // 如果没有直接关联，尝试查找场景中所有触发器
            InteractTaskTrigger[] triggers = FindObjectsOfType<InteractTaskTrigger>();
            if (debugMode) Debug.Log($"找到 {triggers.Length} 个触发器");

            foreach (InteractTaskTrigger trigger in triggers)
            {
                trigger.OnObjectInteracted(gameObject, config);
            }
            hasInteracted = true;
        }

        // 交互后销毁物体
        if (destroyOnInteract)
        {
            Destroy(gameObject);
        }
    }
}