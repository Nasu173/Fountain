using UnityEngine;

/// <summary>
/// 可交互任务对象接口
/// 实现此接口的对象能被任务系统识别并处理交互行为
/// 通常与IInteractable接口一起使用
/// </summary>
public interface ITaskInteractable
{
    /// <summary>
    /// 获取该对象关联的任务ID
    /// 用于确定交互时应该更新哪个任务的进度
    /// </summary>
    /// <returns>任务唯一标识符</returns>
    string GetTaskId();

    /// <summary>
    /// 获取交互的配置信息
    /// 定义交互行为的各种选项（如是否可重复交互、每次增加的进度等）
    /// </summary>
    /// <returns>交互配置对象</returns>
    InteractConfig GetInteractConfig();
}

/// <summary>
/// 交互配置类
/// 定义对象被交互时的各种行为选项
/// 可序列化，支持在Inspector中配置
/// </summary>
[System.Serializable]
public class InteractConfig
{
    [Tooltip("是否允许重复交互同一个对象")]
    public bool canInteractMultipleTimes = false;

    [Tooltip("是否仅首次交互增加任务进度")]
    public bool progressOnlyFirstTime = true;

    [Tooltip("交互后是否销毁该对象")]
    public bool destroyOnInteract = false;

    [Tooltip("每次交互增加的进度值")]
    public int progressPerInteract = 1;
}
