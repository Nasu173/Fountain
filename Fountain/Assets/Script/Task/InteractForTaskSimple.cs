using UnityEngine;
using Fountain.Player;
using Fountain.Common;
using System.Reflection;

/// <summary>
/// 简单交互收集对象
/// 实现IInteractable接口，通过玩家交互系统触发任务的交互
/// 使用反射机制直接更新任务进度，适用于简单的收集类任务
/// </summary>
public class InteractForTaskSimple : MonoBehaviour, IInteractable
{
    [Header("任务配置")]
    [Tooltip("关联的任务触发器（如AreaEnterTrigger等）")]
    [SerializeField] private BaseTaskTrigger taskTrigger;

    [Tooltip("手动指定任务ID（如果没有指定触发器时使用）")]
    [SerializeField] private string customTaskId;

    [Tooltip("自动查找第一个活跃的任务触发器")]
    [SerializeField] private bool autoFindTask = false;

    [Header("收集设置")]
    [Tooltip("是否能多次收集同一个对象")]
    [SerializeField] private bool canCollectMultipleTimes = false;

    [Tooltip("是否仅首次收集增加进度")]
    [SerializeField] private bool progressOnlyFirstTime = true;

    [Tooltip("收集后是否销毁该对象")]
    [SerializeField] private bool destroyOnCollect = true;

    [Tooltip("每次收集增加的进度值")]
    [SerializeField] private int progressPerCollect = 1;

    [Tooltip("收集对象的物品标签（留空则不检查）")]
    [SerializeField] private string requiredItemTag = "Collectible";

    [Header("视觉效果")]
    [Tooltip("轮廓效果组件")]
    [SerializeField] private OutlineVisual outlineVisual;

    [Tooltip("高亮颜色（没有OutlineVisual时使用）")]
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("调试")]
    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool showDebug = true;

    private BaseTaskTrigger targetTask;      // 找到的目标任务触发器
    private bool isCollected = false;         // 是否已被收集
    private Color originalColor;               // 保存原始颜色，用于恢复
    private Renderer objectRenderer;           // 对象的渲染组件

    /// <summary>
    /// 初始化，获取渲染器和原始颜色
    /// </summary>
    private void Awake()
    {
        // 获取渲染组件，保存颜色信息
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color; // 保存原始颜色
        }
    }

    /// <summary>
    /// 启动时查找关联的任务触发器
    /// </summary>
    private void Start()
    {
        FindTargetTask();
    }

    /// <summary>
    /// 查找关联的任务触发器
    /// 优先级：指定触发器 > 通过ID查找 > 自动查找
    /// </summary>
    private void FindTargetTask()
    {
        // 优先使用指定的触发器
        if (taskTrigger != null)
        {
            targetTask = taskTrigger;
            if (showDebug) Debug.Log($"[{gameObject.name}] 使用指定触发器: {targetTask.TaskName}");
            return;
        }

        // 通过ID查找
        if (!string.IsNullOrEmpty(customTaskId))
        {
            BaseTaskTrigger[] tasks = FindObjectsOfType<BaseTaskTrigger>();
            foreach (var task in tasks)
            {
                if (task.TaskId == customTaskId)
                {
                    targetTask = task;
                    if (showDebug) Debug.Log($"[{gameObject.name}] 通过ID找到触发器: {task.TaskName}");
                    break;
                }
            }
        }

        // 自动查找（使用场景中第一个找到的任务触发器）
        if (autoFindTask && targetTask == null)
        {
            BaseTaskTrigger[] tasks = FindObjectsOfType<BaseTaskTrigger>();
            if (tasks.Length > 0)
            {
                targetTask = tasks[0];
                if (showDebug) Debug.Log($"[{gameObject.name}] 自动找到触发器: {targetTask.TaskName}");
            }
        }

        if (targetTask == null && showDebug)
        {
            Debug.LogWarning($"[{gameObject.name}] 未找到关联的任务触发器");
        }
    }

    /// <summary>
    /// IInteractable接口方法，当玩家按E键交互时调用
    /// </summary>
    /// <param name="player">发起交互的玩家对象</param>
    public void InteractWith(PlayerInteractor player)
    {
        // 检查是否已经收集过
        if (!canCollectMultipleTimes && isCollected)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已经收集，无法再次收集");
            return;
        }

        // 检查标签是否符合要求
        if (!string.IsNullOrEmpty(requiredItemTag) && !CompareTag(requiredItemTag))
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 标签不符合要求: {requiredItemTag}");
            return;
        }

        if (showDebug) Debug.Log($"[{gameObject.name}] 被玩家收集");

        // 尝试更新任务进度
        bool progressUpdated = UpdateTaskProgress();

        if (progressUpdated)
        {
            isCollected = true; // 标记为已收集

            // 收集后销毁对象
            if (destroyOnCollect)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// IInteractable接口方法，当玩家看向对象时调用
    /// 用于显示高亮效果
    /// </summary>
    public void Select()
    {
        // 如果已经收集且不能多次收集，不显示
        if (!canCollectMultipleTimes && isCollected) return;

        // 使用OutlineVisual组件显示轮廓效果
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(true);
            if (showDebug) Debug.Log($"[{gameObject.name}] 被选中 - 显示轮廓");
        }
        // 否则使用颜色高亮
        else if (objectRenderer != null)
        {
            objectRenderer.material.color = highlightColor;
            if (showDebug) Debug.Log($"[{gameObject.name}] 被选中 - 改变颜色");
        }
    }

    /// <summary>
    /// IInteractable接口方法，当玩家离开对象时调用
    /// 用于取消高亮效果
    /// </summary>
    public void Deselect()
    {
        // 关闭OutlineVisual轮廓
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(false);
        }
        // 恢复原始颜色
        else if (objectRenderer != null)
        {
            objectRenderer.material.color = originalColor;
        }
    }

    /// <summary>
    /// 更新关联任务的进度
    /// 使用反射机制访问BaseTaskTrigger的私有字段和方法
    /// </summary>
    /// <returns>是否成功更新进度</returns>
    private bool UpdateTaskProgress()
    {
        if (targetTask == null)
        {
            if (showDebug) Debug.LogWarning($"[{gameObject.name}] 未找到目标任务，无法更新进度");
            return false;
        }

        // 通过反射检查任务是否已经开始
        FieldInfo taskStartedField = typeof(BaseTaskTrigger).GetField("taskStarted",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (taskStartedField != null)
        {
            bool taskStarted = (bool)taskStartedField.GetValue(targetTask);

            if (!taskStarted)
            {
                if (showDebug) Debug.Log($"[{gameObject.name}] 任务未开始，无法收集");
                return false;
            }
        }

        // 通过反射检查任务是否已完成
        FieldInfo taskCompletedField = typeof(BaseTaskTrigger).GetField("taskCompleted",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (taskCompletedField != null)
        {
            bool taskCompleted = (bool)taskCompletedField.GetValue(targetTask);

            if (taskCompleted)
            {
                if (showDebug) Debug.Log($"[{gameObject.name}] 任务已完成，无法收集");
                return false;
            }
        }

        // 如果设置为仅首次增加进度，检查是否已经收集过
        if (progressOnlyFirstTime && isCollected)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 仅首次收集增加进度，已经收集过");
            return false;
        }

        // 通过反射调用任务的进度更新方法
        MethodInfo updateMethod = typeof(BaseTaskTrigger).GetMethod("UpdateTaskProgress",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (updateMethod != null)
        {
            // 根据配置的次数循环更新进度
            for (int i = 0; i < progressPerCollect; i++)
            {
                updateMethod.Invoke(targetTask, null);
                if (showDebug) Debug.Log($"[{gameObject.name}] 尝试更新进度: +1 (共{progressPerCollect}次)");
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取关联的任务ID
    /// </summary>
    /// <returns>任务ID</returns>
    public string GetTaskId()
    {
        if (targetTask != null)
            return targetTask.TaskId;
        return customTaskId;
    }

    /// <summary>
    /// 检查对象是否已被收集
    /// </summary>
    /// <returns>true表示已被收集，false表示未被收集</returns>
    public bool IsCollected()
    {
        return isCollected;
    }
}
