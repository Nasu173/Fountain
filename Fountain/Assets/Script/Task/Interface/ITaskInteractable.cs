using UnityEngine;

/// <summary>
/// 可交互任务物体接口
/// 实现此接口的物体可以被任务系统识别并处理交互行为
/// </summary>
public interface ITaskInteractable
{
    /// <summary>
    /// 获取此物体关联的任务ID
    /// </summary>
    /// <returns>任务唯一标识符</returns>
    string GetTaskId();

    /// <summary>
    /// 获取此物体的交互配置
    /// </summary>
    /// <returns>交互配置对象</returns>
    InteractConfig GetInteractConfig();
}

/// <summary>
/// 交互配置类
/// 定义了物体被交互时的各种行为选项
/// </summary>
[System.Serializable]
public class InteractConfig
{
    [Tooltip("是否允许重复交互同一个物体")]
    public bool canInteractMultipleTimes = false;

    [Tooltip("是否仅首次交互增加任务进度")]
    public bool progressOnlyFirstTime = true;

    [Tooltip("交互后是否销毁物体")]
    public bool destroyOnInteract = false;

    [Tooltip("每次交互增加的进度值")]
    public int progressPerInteract = 1;
}