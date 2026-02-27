using UnityEngine;
using Foutain.Player;
using Foutain.Common;
using System.Reflection;

public class CollectForTaskSimple : MonoBehaviour, IInteractable
{
    [Header("任务关联")]
    [Tooltip("关联的任务触发器（如AreaEnterTrigger）")]
    [SerializeField] private BaseTaskTrigger taskTrigger;

    [Tooltip("手动输入任务ID（当没有指定触发器时使用）")]
    [SerializeField] private string customTaskId;

    [Tooltip("自动查找第一个活跃的任务触发器")]
    [SerializeField] private bool autoFindTask = false;

    [Header("收集配置")]
    [Tooltip("是否能多次收集同一个物体")]
    [SerializeField] private bool canCollectMultipleTimes = false;

    [Tooltip("是否仅首次收集增加进度")]
    [SerializeField] private bool progressOnlyFirstTime = true;

    [Tooltip("收集后是否销毁物体")]
    [SerializeField] private bool destroyOnCollect = true;

    [Tooltip("每次收集增加的进度值")]
    [SerializeField] private int progressPerCollect = 1;

    [Tooltip("收集所需的物品标签（留空则不检查）")]
    [SerializeField] private string requiredItemTag = "Collectible";

    [Header("高亮设置")]
    [Tooltip("描边效果组件")]
    [SerializeField] private OutlineVisual outlineVisual;

    [Tooltip("高亮颜色（当没有OutlineVisual时使用）")]
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("调试")]
    [SerializeField] private bool showDebug = true;

    private BaseTaskTrigger targetTask;
    private bool isCollected = false;
    private Color originalColor;
    private Renderer objectRenderer;

    private void Awake()
    {
        // 获取渲染器组件用于颜色高亮
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
    }

    private void Start()
    {
        FindTargetTask();
    }

    private void FindTargetTask()
    {
        // 优先使用指定的触发器
        if (taskTrigger != null)
        {
            targetTask = taskTrigger;
            if (showDebug) Debug.Log($"[{gameObject.name}] 关联到指定触发器: {targetTask.TaskName}");
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
                    if (showDebug) Debug.Log($"[{gameObject.name}] 通过ID关联到任务: {task.TaskName}");
                    break;
                }
            }
        }

        // 自动查找
        if (autoFindTask && targetTask == null)
        {
            BaseTaskTrigger[] tasks = FindObjectsOfType<BaseTaskTrigger>();
            if (tasks.Length > 0)
            {
                targetTask = tasks[0];
                if (showDebug) Debug.Log($"[{gameObject.name}] 自动关联到任务: {targetTask.TaskName}");
            }
        }

        if (targetTask == null && showDebug)
        {
            Debug.LogWarning($"[{gameObject.name}] 未找到关联的任务触发器");
        }
    }

    /// <summary>
    /// IInteractable接口方法：当玩家按E键交互时调用
    /// </summary>
    public void InteractWith(PlayerInteractor player)
    {
        // 检查是否已经收集过
        if (!canCollectMultipleTimes && isCollected)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已经收集过，无法再次收集");
            return;
        }

        // 检查标签
        if (!string.IsNullOrEmpty(requiredItemTag) && !CompareTag(requiredItemTag))
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 标签不符，需要: {requiredItemTag}");
            return;
        }

        if (showDebug) Debug.Log($"[{gameObject.name}] 被玩家收集");

        // 推进任务进度
        bool progressUpdated = UpdateTaskProgress();

        if (progressUpdated)
        {
            isCollected = true;

            // 收集后销毁物体
            if (destroyOnCollect)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// IInteractable接口方法：当玩家看向物体时调用
    /// </summary>
    public void Select()
    {
        // 如果已经收集过且不能多次收集，不高亮
        if (!canCollectMultipleTimes && isCollected) return;

        // 使用OutlineVisual高亮
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(true);
            if (showDebug) Debug.Log($"[{gameObject.name}] 被选中 - 显示描边");
        }
        // 否则使用颜色高亮
        else if (objectRenderer != null)
        {
            objectRenderer.material.color = highlightColor;
            if (showDebug) Debug.Log($"[{gameObject.name}] 被选中 - 改变颜色");
        }
    }

    /// <summary>
    /// IInteractable接口方法：当玩家离开物体时调用
    /// </summary>
    public void Deselect()
    {
        // 关闭OutlineVisual高亮
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
    /// 更新任务进度
    /// </summary>
    private bool UpdateTaskProgress()
    {
        if (targetTask == null)
        {
            if (showDebug) Debug.LogWarning($"[{gameObject.name}] 未找到目标任务，无法更新进度");
            return false;
        }

        // 检查任务是否已经开始
        FieldInfo taskStartedField = typeof(BaseTaskTrigger).GetField("taskStarted",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (taskStartedField != null)
        {
            bool taskStarted = (bool)taskStartedField.GetValue(targetTask);

            if (!taskStarted)
            {
                if (showDebug) Debug.Log($"[{gameObject.name}] 任务还未开始，不能收集");
                return false;
            }
        }

        // 检查任务是否已完成
        FieldInfo taskCompletedField = typeof(BaseTaskTrigger).GetField("taskCompleted",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (taskCompletedField != null)
        {
            bool taskCompleted = (bool)taskCompletedField.GetValue(targetTask);

            if (taskCompleted)
            {
                if (showDebug) Debug.Log($"[{gameObject.name}] 任务已完成，不能收集");
                return false;
            }
        }

        // 如果仅首次增加进度，检查是否已经收集过
        if (progressOnlyFirstTime && isCollected)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 仅首次收集增加进度，已经收集过");
            return false;
        }

        // 更新进度
        MethodInfo updateMethod = typeof(BaseTaskTrigger).GetMethod("UpdateTaskProgress",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (updateMethod != null)
        {
            for (int i = 0; i < progressPerCollect; i++)
            {
                updateMethod.Invoke(targetTask, null);
                if (showDebug) Debug.Log($"[{gameObject.name}] 推进任务进度: +1 (共{progressPerCollect}次)");
            }
            return true;
        }

        return false;
    }

    /// <summary>
    /// 获取关联的任务ID
    /// </summary>
    public string GetTaskId()
    {
        if (targetTask != null)
            return targetTask.TaskId;
        return customTaskId;
    }

    /// <summary>
    /// 检查物体是否已被收集
    /// </summary>
    public bool IsCollected()
    {
        return isCollected;
    }
}