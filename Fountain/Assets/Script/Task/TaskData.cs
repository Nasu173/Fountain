using System;
using UnityEngine;

/// <summary>
/// 任务数据类
/// 存储单个任务的所有数据，包括名称、进度、描述和完成状态
/// 可序列化，支持在Inspector中编辑和保存
/// </summary>
[Serializable]
public class TaskData
{
    [Tooltip("任务名称（显示在UI标题）")]
    public string taskName;

    [Tooltip("目标数量（任务需要完成的总次数）")]
    public int targetCount;

    [Tooltip("当前完成数量（已完成的次数）")]
    public int currentCount;

    [Tooltip("任务描述（详细说明任务内容）")]
    public string description;

    [Tooltip("任务是否已完成（达到目标数量时自动设为true）")]
    public bool isCompleted;

    /// <summary>
    /// 构造函数，创建新的任务数据实例
    /// 初始化所有必要字段，进度从0开始
    /// </summary>
    /// <param name="name">任务名称</param>
    /// <param name="target">目标数量（需要完成的次数）</param>
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
    /// 增加完成数量，并自动检查是否达到目标
    /// </summary>
    /// <param name="amount">要增加的进度值（通常为1）</param>
    public void UpdateProgress(int amount)
    {
        // 只有任务未完成时才能更新进度
        if (!isCompleted)
        {
            // 增加进度，使用Mathf.Min确保不超过目标数量
            currentCount = Mathf.Min(currentCount + amount, targetCount);

            // 检查是否达到完成条件
            if (currentCount >= targetCount)
            {
                isCompleted = true; // 自动标记任务为已完成
            }
        }
    }

    /// <summary>
    /// 获取任务完成百分比
    /// 用于进度条显示
    /// </summary>
    /// <returns>0-1之间的浮点数，表示完成比例（0%到100%）</returns>
    public float GetProgressPercentage()
    {
        // 防止除以零错误
        if (targetCount <= 0) return 0f;

        // 返回当前进度除以目标数量的比值
        return (float)currentCount / targetCount;
    }

    /// <summary>
    /// 获取进度的文本表示
    /// 用于UI文本显示
    /// </summary>
    /// <returns>格式化的进度字符串（如"3/5"）</returns>
    public string GetProgressText()
    {
        return currentCount + "/" + targetCount;
    }
}
