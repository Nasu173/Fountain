using System;
using UnityEngine;

/// <summary>
/// 任务数据类
/// 存储单个任务的所有数据，包括名称、进度、描述等
/// </summary>
[Serializable] // 可序列化，方便在Unity Inspector中显示和保存
public class TaskData
{
    [Tooltip("任务名称")]
    public string taskName;

    [Tooltip("目标数量（完成任务需要的总次数）")]
    public int targetCount;

    [Tooltip("当前进度数量")]
    public int currentCount;

    [Tooltip("任务描述")]
    public string description;

    [Tooltip("任务是否已完成")]
    public bool isCompleted;

    /// <summary>
    /// 构造函数：创建新的任务数据
    /// </summary>
    /// <param name="name">任务名称</param>
    /// <param name="target">目标数量</param>
    /// <param name="desc">任务描述</param>
    public TaskData(string name, int target, string desc)
    {
        taskName = name;          // 设置任务名称
        targetCount = target;      // 设置目标数量
        currentCount = 0;          // 初始进度为0
        description = desc;        // 设置任务描述
        isCompleted = false;       // 初始状态为未完成
    }

    /// <summary>
    /// 更新任务进度
    /// </summary>
    /// <param name="amount">要增加的进度值</param>
    public void UpdateProgress(int amount)
    {
        // 只有在任务未完成时才能更新进度
        if (!isCompleted)
        {
            // 增加进度，但不超过目标数量
            currentCount = Mathf.Min(currentCount + amount, targetCount);

            // 检查是否达到完成条件
            if (currentCount >= targetCount)
            {
                isCompleted = true; // 标记任务为已完成
            }
        }
    }

    /// <summary>
    /// 获取任务完成百分比
    /// </summary>
    /// <returns>0-1之间的浮点数，表示完成比例</returns>
    public float GetProgressPercentage()
    {
        // 防止除零错误
        if (targetCount <= 0) return 0f;

        // 返回当前进度除以目标数量的比值
        return (float)currentCount / targetCount;
    }

    /// <summary>
    /// 获取任务进度文本（如 "3/5"）
    /// </summary>
    /// <returns>格式化的进度字符串</returns>
    public string GetProgressText()
    {
        return currentCount + "/" + targetCount;
    }
}