using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 交互任务触发器
/// 处理玩家与物体的交互行为，并根据交互配置更新任务进度
/// </summary>
public class InteractTaskTrigger : BaseTaskTrigger
{
    [Header("交互任务特定设置")]
    // 使用基类的debugMode字段，不在子类重复定义

    private HashSet<GameObject> interactedObjects = new HashSet<GameObject>(); // 记录已交互过的物体（用于首次交互判断）
    private int currentProgress = 0; // 当前任务进度

    /// <summary>
    /// 重写当前进度属性，使用私有字段currentProgress存储
    /// </summary>
    protected override int CurrentProgress
    {
        get => currentProgress;
        set => currentProgress = value;
    }

    /// <summary>
    /// 检查触发条件
    /// 此触发器由外部事件（OnObjectInteracted）驱动，不需要Update检查
    /// </summary>
    /// <returns>始终返回false</returns>
    protected override bool CheckTriggerCondition() => false;

    /// <summary>
    /// 获取每次交互的进度增加值
    /// 实际进度增加值由config.progressPerInteract控制，此处返回基础值1
    /// </summary>
    /// <returns>固定返回1</returns>
    protected override int GetProgressAmount() => 1;

    /// <summary>
    /// 增加进度计数
    /// </summary>
    protected override void IncrementProgress()
    {
        currentProgress++;
    }

    /// <summary>
    /// 当物体被交互时调用（由可交互物体脚本调用）
    /// </summary>
    /// <param name="obj">被交互的物体对象</param>
    /// <param name="config">交互配置，定义交互行为</param>
    public void OnObjectInteracted(GameObject obj, InteractConfig config)
    {
        // 如果任务已完成，忽略本次交互
        if (taskCompleted)
        {
            if (debugMode) Debug.Log($"[{TaskName}] 任务已完成，忽略交互");
            return;
        }

        // 检查是否可以交互（根据配置判断）
        if (!CanInteract(obj, config))
        {
            if (debugMode) Debug.Log($"[{TaskName}] 无法交互（已超过限制）");
            return;
        }

        // 如果配置为仅首次交互增加进度，记录已交互的物体
        if (config.progressOnlyFirstTime)
        {
            interactedObjects.Add(obj);
        }

        if (debugMode) Debug.Log($"[{TaskName}] 物体被交互: {obj.name}");

        // 如果任务尚未开始，启动任务
        if (!taskStarted)
        {
            StartTask();
        }

        // 根据配置的每次交互进度值，多次更新任务进度
        for (int i = 0; i < config.progressPerInteract; i++)
        {
            UpdateTaskProgress();
        }

        // 检查任务是否达到完成条件
        if (currentProgress >= TargetCount)
        {
            OnTaskCompleted(); // 触发任务完成回调
        }
    }

    /// <summary>
    /// 检查是否可以与物体交互
    /// </summary>
    /// <param name="obj">要交互的物体</param>
    /// <param name="config">交互配置</param>
    /// <returns>true表示可以交互，false表示不能交互</returns>
    private bool CanInteract(GameObject obj, InteractConfig config)
    {
        // 如果配置允许多次交互，总是返回true
        if (config.canInteractMultipleTimes) return true;

        // 如果只能交互一次，检查是否已经交互过
        return !interactedObjects.Contains(obj);
    }
}