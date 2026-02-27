using UnityEngine;
using Foutain.Player;
using Foutain.Common;

public class TaskInteractable : MonoBehaviour, IInteractable, ITaskInteractable
{
    [Header("任务关联")]
    [SerializeField] private InteractTaskTrigger taskTrigger;
    [SerializeField] private string customTaskId;

    [Header("交互配置")]
    [SerializeField] private bool canInteractMultipleTimes = false;
    [SerializeField] private bool progressOnlyFirstTime = true;
    [SerializeField] private bool destroyOnInteract = false;
    [SerializeField] private int progressPerInteract = 1;

    [Header("高亮设置")]
    [Tooltip("描边效果组件（自动查找或手动拖拽）")]
    [SerializeField] private OutlineVisual outlineVisual;
    [SerializeField] private bool autoFindOutline = true;

    [Header("调试")]
    [SerializeField] private bool showDebug = true;

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

        // 自动查找OutlineVisual组件
        if (autoFindOutline && outlineVisual == null)
        {
            outlineVisual = GetComponent<OutlineVisual>();
            if (outlineVisual == null && showDebug)
            {
                Debug.LogWarning($"[{gameObject.name}] 未找到OutlineVisual组件，高亮效果将不可用");
            }
        }

        // 初始状态：关闭描边
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(false);
        }
    }

    public void InteractWith(PlayerInteractor player)
    {
        if (showDebug) Debug.Log($"[{gameObject.name}] 被玩家交互");

        if (!canInteractMultipleTimes && hasInteracted)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已经交互过，无法再次交互");
            return;
        }

        // 传递 config 参数
        TriggerTask();
        hasInteracted = true;

        if (destroyOnInteract)
        {
            Destroy(gameObject);
        }
    }

    private void TriggerTask()
    {
        if (taskTrigger != null)
        {
            // 传递 gameObject 和 config 两个参数
            taskTrigger.OnObjectInteracted(gameObject, config);
        }
        else if (!string.IsNullOrEmpty(customTaskId))
        {
            InteractTaskTrigger[] triggers = FindObjectsOfType<InteractTaskTrigger>();
            foreach (var trigger in triggers)
            {
                if (trigger.TaskId == customTaskId)
                {
                    // 传递 gameObject 和 config 两个参数
                    trigger.OnObjectInteracted(gameObject, config);
                    break;
                }
            }
        }
    }

    public void Select()
    {
        // 使用OutlineVisual显示高亮
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(true);
            if (showDebug) Debug.Log($"[{gameObject.name}] 被选中 - 显示描边");
        }
        else if (showDebug)
        {
            Debug.LogWarning($"[{gameObject.name}] 被选中，但无OutlineVisual组件");
        }
    }

    public void Deselect()
    {
        // 使用OutlineVisual关闭高亮
        if (outlineVisual != null)
        {
            outlineVisual.SetOutline(false);
            if (showDebug) Debug.Log($"[{gameObject.name}] 取消选中 - 隐藏描边");
        }
    }

    public string GetTaskId()
    {
        return !string.IsNullOrEmpty(customTaskId) ? customTaskId :
               (taskTrigger != null ? taskTrigger.TaskId : null);
    }

    public InteractConfig GetInteractConfig()
    {
        return config;
    }
}