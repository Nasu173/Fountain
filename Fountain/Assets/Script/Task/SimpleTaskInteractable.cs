using UnityEngine;

/// <summary>
/// 简单的可交互任务对象
/// 仅实现IInteractable接口，需要通过外部方式调用OnPlayerInteract方法
/// 适用于不需要高亮效果的简单交互对象
/// </summary>
public class SimpleTaskInteractable : MonoBehaviour
{
    [Header("任务配置")]
    [Tooltip("关联的交互任务触发器")]
    [SerializeField] private InteractTaskTrigger taskTrigger;

    [Tooltip("手动指定任务ID（如果没有指定触发器时使用）")]
    [SerializeField] private string customTaskId;

    [Header("交互设置")]
    [Tooltip("是否能多次交互同一个对象")]
    [SerializeField] private bool canInteractMultipleTimes = false;

    [Tooltip("是否仅首次交互增加进度")]
    [SerializeField] private bool progressOnlyFirstTime = true;

    [Tooltip("交互后是否销毁该对象")]
    [SerializeField] private bool destroyOnInteract = false;

    [Tooltip("每次交互增加的进度值")]
    [SerializeField] private int progressPerInteract = 1;

    [Header("调试")]
    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool showDebug = true;

    private InteractConfig config;      // 交互配置对象
    private bool hasInteracted = false;  // 是否已经交互过

    /// <summary>
    /// 初始化，创建交互配置对象
    /// </summary>
    private void Awake()
    {
        // 将Inspector中的配置参数转换为InteractConfig对象
        config = new InteractConfig
        {
            canInteractMultipleTimes = canInteractMultipleTimes,
            progressOnlyFirstTime = progressOnlyFirstTime,
            destroyOnInteract = destroyOnInteract,
            progressPerInteract = progressPerInteract
        };
    }

    /// <summary>
    /// 当玩家交互此对象时调用（由外部脚本调用）
    /// </summary>
    public void OnPlayerInteract()
    {
        // 检查交互限制条件
        if (!canInteractMultipleTimes && hasInteracted)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已经交互过");
            return;
        }

        if (showDebug) Debug.Log($"[{gameObject.name}] 交互中");

        // 触发关联的任务
        TriggerTask();

        // 标记为已交互
        hasInteracted = true;

        // 如果设置为交互后销毁，则销毁对象
        if (destroyOnInteract)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 触发关联的任务
    /// 优先使用指定的触发器，否则通过ID查找触发器
    /// </summary>
    private void TriggerTask()
    {
        // 如果指定了触发器，直接使用
        if (taskTrigger != null)
        {
            // 调用触发器的交互处理方法，传递配置参数
            taskTrigger.OnObjectInteracted(gameObject, config);
        }
        // 如果指定了任务ID，通过ID查找触发器
        else if (!string.IsNullOrEmpty(customTaskId))
        {
            // 查找场景中所有的交互任务触发器
            var triggers = FindObjectsOfType<InteractTaskTrigger>();
            foreach (var trigger in triggers)
            {
                // 找到ID匹配的触发器
                if (trigger.TaskId == customTaskId)
                {
                    // 调用触发器的交互处理方法
                    trigger.OnObjectInteracted(gameObject, config);
                    break; // 找到后退出循环
                }
            }
        }
    }
}
