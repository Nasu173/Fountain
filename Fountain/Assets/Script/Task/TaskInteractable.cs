using UnityEngine;
using Foutain.Player;
using Foutain.Common;

/// <summary>
/// 可交互任务对象
/// 实现IInteractable接口，通过玩家交互系统触发任务的交互
/// 同时实现ITaskInteractable接口，提供任务ID和交互配置
/// 支持轮廓高亮效果
/// </summary>
public class TaskInteractable : MonoBehaviour, IInteractable, ITaskInteractable
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

    [Header("视觉效果")]
    [Tooltip("轮廓效果组件（可以手动拖拽）")]
    [SerializeField] private OutlineVisual outlineVisual;

    [Tooltip("是否自动查找OutlineVisual组件")]
    [SerializeField] private bool autoFindOutline = true;

    [Header("调试")]
    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool showDebug = true;

    private InteractConfig config;      // 交互配置对象
    private bool hasInteracted = false;  // 是否已经交互过

    /// <summary>
    /// 初始化，创建交互配置并查找组件
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

        // 自动查找OutlineVisual组件（如果启用且未指定）
        if (autoFindOutline && outlineVisual == null)
        {
            outlineVisual = GetComponent<OutlineVisual>();
            if (outlineVisual == null && showDebug)
            {
                Debug.LogWarning($"[{gameObject.name}] 未找到OutlineVisual组件，轮廓效果不可用");
            }
        }

        // 初始状态关闭轮廓
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(false);
        }
    }

    /// <summary>
    /// IInteractable接口方法，当玩家按E键交互时调用
    /// </summary>
    /// <param name="player">发起交互的玩家对象</param>
    public void InteractWith(PlayerInteractor player)
    {
        if (showDebug) Debug.Log($"[{gameObject.name}] 被玩家交互");

        // 检查交互限制条件
        if (!canInteractMultipleTimes && hasInteracted)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已经交互过，无法再次交互");
            return;
        }

        // 触发关联的任务，传递配置参数
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
            InteractTaskTrigger[] triggers = FindObjectsOfType<InteractTaskTrigger>();
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

    /// <summary>
    /// IInteractable接口方法，当玩家看向对象时调用
    /// 用于显示轮廓效果
    /// </summary>
    public void Select()
    {
        // 使用OutlineVisual显示轮廓
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(true);
            if (showDebug) Debug.Log($"[{gameObject.name}] 被选中 - 显示轮廓");
        }
        else if (showDebug)
        {
            Debug.LogWarning($"[{gameObject.name}] 被选中，但无OutlineVisual组件");
        }
    }

    /// <summary>
    /// IInteractable接口方法，当玩家离开对象时调用
    /// 用于取消轮廓效果
    /// </summary>
    public void Deselect()
    {
        // 使用OutlineVisual关闭轮廓
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(false);
            if (showDebug) Debug.Log($"[{gameObject.name}] 取消选中 - 关闭轮廓");
        }
    }

    /// <summary>
    /// ITaskInteractable接口方法，获取关联的任务ID
    /// </summary>
    /// <returns>任务ID，如果没有关联则返回null</returns>
    public string GetTaskId()
    {
        // 优先使用自定义ID，否则使用触发器的ID
        return !string.IsNullOrEmpty(customTaskId) ? customTaskId :
               (taskTrigger != null ? taskTrigger.TaskId : null);
    }

    /// <summary>
    /// ITaskInteractable接口方法，获取交互配置
    /// </summary>
    /// <returns>交互配置对象</returns>
    public InteractConfig GetInteractConfig()
    {
        return config;
    }
}
