using UnityEngine;
using Foutain.Player;

public class InteractTaskTrigger : BaseTaskTrigger
{
    [Header("交互任务设置")]
    [Tooltip("需要交互的物体标签")]
    [SerializeField] private string targetTag = "Interactable";

    [Tooltip("是否需要特定物体名称（可选）")]
    [SerializeField] private string targetObjectName;

    [Tooltip("是否只触发一次")]
    [SerializeField] private bool triggerOnce = true;

    [Tooltip("每次交互增加的进度")]
    [SerializeField] private int progressPerInteraction = 1;

    private int interactionCount = 0;
    private bool hasTriggered = false;

    // 缓存所有需要监听的交互物体
    private GameObject[] targetObjects;

    protected override void Start()
    {
        base.Start();

        // 查找所有目标物体
        FindTargetObjects();
    }

    private void FindTargetObjects()
    {
        if (!string.IsNullOrEmpty(targetObjectName))
        {
            // 按名称查找特定物体
            GameObject obj = GameObject.Find(targetObjectName);
            if (obj != null)
            {
                targetObjects = new GameObject[] { obj };
                if (debugMode) Debug.Log($"[{GetType().Name}] 找到目标物体: {targetObjectName}");
            }
        }
        else if (!string.IsNullOrEmpty(targetTag))
        {
            // 按标签查找物体
            targetObjects = GameObject.FindGameObjectsWithTag(targetTag);
            if (debugMode) Debug.Log($"[{GetType().Name}] 找到 {targetObjects.Length} 个标签为 {targetTag} 的物体");
        }
    }

    protected override bool CheckTriggerCondition()
    {
        // 这个触发器由事件驱动，所以这里返回false
        return false;
    }

    protected override int GetProgressAmount()
    {
        return progressPerInteraction;
    }

    /// <summary>
    /// 当玩家与物体交互时调用这个方法
    /// </summary>
    public void OnObjectInteracted(GameObject interactedObject)
    {
        if (taskCompleted)
        {
            if (debugMode) Debug.Log($"[{GetType().Name}] 任务已完成，忽略交互");
            return;
        }

        if (triggerOnce && hasTriggered)
        {
            if (debugMode) Debug.Log($"[{GetType().Name}] 已触发过，忽略本次交互");
            return;
        }

        // 检查是否是目标物体
        if (IsTargetObject(interactedObject))
        {
            interactionCount++;
            hasTriggered = true;

            if (debugMode) Debug.Log($"[{GetType().Name}] 与目标物体交互: {interactedObject.name}, 次数: {interactionCount}/{targetCount}");

            if (!taskStarted)
            {
                StartTask();
            }
            else
            {
                UpdateTaskProgress();
            }

            // 检查任务是否完成
            if (interactionCount >= targetCount && !taskCompleted)
            {
                OnTaskCompleted();
            }
        }
        else
        {
            if (debugMode) Debug.Log($"[{GetType().Name}] 交互物体不是目标: {interactedObject.name}");
        }
    }

    private bool IsTargetObject(GameObject obj)
    {
        if (obj == null) return false;

        // 检查名称匹配
        if (!string.IsNullOrEmpty(targetObjectName))
        {
            if (obj.name == targetObjectName || obj.name.Contains(targetObjectName))
                return true;
        }

        // 检查标签匹配
        if (!string.IsNullOrEmpty(targetTag))
        {
            if (obj.CompareTag(targetTag))
                return true;
        }

        return false;
    }

    // 在编辑器中修改参数时重新查找物体
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            FindTargetObjects();
        }
#endif
    }
}