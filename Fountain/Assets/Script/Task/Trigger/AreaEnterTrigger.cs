using UnityEngine;

public class AreaEnterTrigger : BaseTaskTrigger
{
    [Header("区域设置")]
    [SerializeField] private string targetAreaTag = "Player";
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool showDebug = true;

    private bool areaEntered = false;
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

    protected override int GetProgressAmount() => 1;

    protected override void IncrementProgress()
    {
        currentProgress++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (taskCompleted) return;

        if (other.CompareTag(targetAreaTag) && !areaEntered)
        {
            areaEntered = true;

            if (showDebug) Debug.Log($"[{TaskName}] 玩家进入区域，触发任务");

            // 启动任务（只触发一次）
            if (!taskStarted)
            {
                StartTask();
            }

            if (triggerOnce)
            {
                enabled = false; // 禁用触发器，不再检测
            }
        }
    }
}