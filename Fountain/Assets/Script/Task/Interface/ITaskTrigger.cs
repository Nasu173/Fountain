/// <summary>
/// 任务触发器接口
/// 定义了所有任务触发器必须实现的基本属性
/// </summary>
public interface ITaskTrigger
{
    /// <summary>
    /// 获取任务的唯一标识符
    /// </summary>
    string TaskId { get; }

    /// <summary>
    /// 获取任务名称
    /// </summary>
    string TaskName { get; }

    /// <summary>
    /// 获取任务编号（显示在UI上，如"#1"）
    /// </summary>
    string TaskNumber { get; }

    /// <summary>
    /// 获取任务目标数量
    /// </summary>
    int TargetCount { get; }

    /// <summary>
    /// 获取任务描述
    /// </summary>
    string Description { get; }
}