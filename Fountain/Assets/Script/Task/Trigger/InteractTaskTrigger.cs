using UnityEngine;
using System.Collections.Generic;

public class InteractTaskTrigger : BaseTaskTrigger
{
    [Header("交互任务特定设置")]
    // 移除 debugMode 字段，直接使用基类的

    private HashSet<GameObject> interactedObjects = new HashSet<GameObject>();
    private int currentProgress = 0;

    // 重写CurrentProgress属性
    protected override int CurrentProgress
    {
        get => currentProgress;
        set => currentProgress = value;
    }

    protected override bool CheckTriggerCondition() => false;

    protected override int GetProgressAmount() => 1;

    protected override void IncrementProgress()
    {
        currentProgress++;
    }

    /// <summary>
    /// 当物体被交互时调用
    /// </summary>
    public void OnObjectInteracted(GameObject obj, InteractConfig config)
    {
        if (taskCompleted)
        {
            if (debugMode) Debug.Log($"[{TaskName}] 任务已完成，忽略交互");
            return;
        }

        // 检查是否可以交互
        if (!CanInteract(obj, config))
        {
            if (debugMode) Debug.Log($"[{TaskName}] 无法交互（已超过限制）");
            return;
        }

        // 记录交互
        if (config.progressOnlyFirstTime)
        {
            interactedObjects.Add(obj);
        }

        if (debugMode) Debug.Log($"[{TaskName}] 物体被交互: {obj.name}");

        // 启动任务或更新进度
        if (!taskStarted)
        {
            StartTask();
        }

        // 更新进度（多次）
        for (int i = 0; i < config.progressPerInteract; i++)
        {
            UpdateTaskProgress();
        }

        // 检查任务是否完成
        if (currentProgress >= TargetCount)
        {
            OnTaskCompleted();
        }
    }

    private bool CanInteract(GameObject obj, InteractConfig config)
    {
        // 如果能多次交互，总是可以
        if (config.canInteractMultipleTimes) return true;

        // 如果只能交互一次，检查是否已交互过
        return !interactedObjects.Contains(obj);
    }
}