using UnityEngine;

public class AreaEnterTrigger : BaseTaskTrigger
{
    [Header("Area Settings")]
    [SerializeField] private string targetAreaTag = "Area";

    private bool playerInArea = false;
    private bool areaEntered = false;

    protected override bool CheckTriggerCondition()
    {
        // 这个触发器在玩家进入区域时触发一次
        return playerInArea && !areaEntered;
    }

    protected override int GetProgressAmount()
    {
        return 1; // 进入区域算作完成一次进度
    }

    protected override void UpdateTaskProgress()
    {
        if (CheckTriggerCondition())
        {
            base.UpdateTaskProgress();
            areaEntered = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = false;
        }
    }
}
