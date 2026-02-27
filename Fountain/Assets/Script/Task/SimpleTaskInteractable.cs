using UnityEngine;

public class SimpleTaskInteractable : MonoBehaviour
{
    [Header("任务关联")]
    [SerializeField] private InteractTaskTrigger taskTrigger;
    [SerializeField] private string customTaskId;

    [Header("交互配置")]
    [SerializeField] private bool canInteractMultipleTimes = false;
    [SerializeField] private bool progressOnlyFirstTime = true;
    [SerializeField] private bool destroyOnInteract = false;
    [SerializeField] private int progressPerInteract = 1;

    [Header("调试")]
    [SerializeField] private bool showDebug = true;

    private InteractConfig config;
    private bool hasInteracted = false;

    private void Awake()
    {
        config = new InteractConfig
        {
            canInteractMultipleTimes = canInteractMultipleTimes,
            progressOnlyFirstTime = progressOnlyFirstTime,
            destroyOnInteract = destroyOnInteract,
            progressPerInteract = progressPerInteract
        };
    }

    public void OnPlayerInteract()
    {
        if (!canInteractMultipleTimes && hasInteracted)
        {
            if (showDebug) Debug.Log($"[{gameObject.name}] 已经交互过");
            return;
        }

        if (showDebug) Debug.Log($"[{gameObject.name}] 被交互");

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
            var triggers = FindObjectsOfType<InteractTaskTrigger>();
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
}