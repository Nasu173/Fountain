using UnityEngine;

public class CollectItemTrigger : BaseTaskTrigger
{
    [Header("Collect Settings")]
    [SerializeField] private string itemTag = "Collectible";
    [SerializeField] private bool destroyOnCollect = true;

    private int itemsCollected = 0;
    private int lastCollectedCount = 0;
    private int currentProgress = 0;

    // 重写CurrentProgress属性
    protected override int CurrentProgress
    {
        get => currentProgress;
        set => currentProgress = value;
    }

    protected override bool CheckTriggerCondition()
    {
        return false; // 由OnTriggerEnter驱动
    }

    protected override int GetProgressAmount()
    {
        int collected = itemsCollected - lastCollectedCount;
        if (collected > 0)
        {
            lastCollectedCount = itemsCollected;
        }
        return collected;
    }

    protected override void IncrementProgress()
    {
        currentProgress++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag(itemTag))
        {
            itemsCollected++;

            if (!taskStarted)
            {
                StartTask();
            }

            if (!taskCompleted)
            {
                UpdateTaskProgress();
            }

            if (destroyOnCollect && other.gameObject != null)
            {
                Destroy(other.gameObject);
            }

            // 检查任务是否完成
            if (currentProgress >= TargetCount)
            {
                OnTaskCompleted();
            }
        }
    }
}