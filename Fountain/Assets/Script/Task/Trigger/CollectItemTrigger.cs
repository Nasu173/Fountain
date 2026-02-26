using UnityEngine;

public class CollectItemTrigger : BaseTaskTrigger
{
    [Header("Collect Settings")]
    [SerializeField] private string itemTag = "Collectible";

    private int itemsCollected = 0;
    private int lastCollectedCount = 0;

    protected override bool CheckTriggerCondition()
    {
        return false;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag(itemTag))
        {
            itemsCollected++;

            if (!taskStarted)
            {
                StartTask();
            }
            else if (!taskCompleted)
            {
                UpdateTaskProgress();

                if (itemsCollected >= targetCount)
                {
                    OnTaskCompleted();
                }
            }

            if (other.gameObject != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
}