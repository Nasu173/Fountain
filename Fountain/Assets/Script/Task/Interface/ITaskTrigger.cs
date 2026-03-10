/// <summary>
/// 任务触发器接口
/// 定义所有任务触发器必须实现的基础属性
/// 用于统一管理不同类型的任务触发器
/// </summary>
public interface ITaskTrigger
{
    /// <summary>
    /// 获取任务唯一标识符
    /// 用于在TaskManager中注册和查找任务
    /// </summary>
    string TaskId { get; }

    /// <summary>
    /// 获取任务名称
    /// 显示在任务UI的标题位置
    /// </summary>
    string TaskName { get; }

    /// <summary>
    /// 获取任务编号（显示在UI上，如"#1"）
    /// 用于标识任务的顺序或优先级
    /// </summary>
    string TaskNumber { get; }

    /// <summary>
    /// 获取任务目标数量
    /// 表示任务需要完成的总次数
    /// </summary>
    int TargetCount { get; }

    /// <summary>
    /// 获取任务描述
    /// 详细说明任务的内容和要求
    /// </summary>
    string Description { get; }
}
