using UnityEngine;

/// <summary>
/// 区域进入触发器
/// 当玩家进入指定区域时触发任务
/// </summary>
public class AreaEnterTrigger : BaseTaskTrigger
{
    [Header("区域设置")]
    [Tooltip("要检测的目标标签，默认为Player")]
    [SerializeField] private string targetAreaTag = "Player";

    [Tooltip("是否只触发一次，触发后禁用此触发器")]
    [SerializeField] private bool triggerOnce = true;

    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool showDebug = true;

    private bool areaEntered = false;      // 是否已经进入过区域
    private int currentProgress = 0;       // 当前进度

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
    /// 此触发器由OnTriggerEnter驱动，不需要Update检查
    /// </summary>
    /// <returns>始终返回false</returns>
    protected override bool CheckTriggerCondition()
    {
        return false; // 由OnTriggerEnter驱动
    }

    /// <summary>
    /// 获取每次触发的进度增加值
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
    /// 当其他碰撞体进入触发器时调用
    /// </summary>
    /// <param name="other">进入触发器的碰撞体</param>
    private void OnTriggerEnter(Collider other)
    {
        // 如果任务已完成，不再响应
        if (taskCompleted) return;

        // 检查进入的对象是否为目标标签，且尚未进入过区域
        if (other.CompareTag(targetAreaTag) && !areaEntered)
        {
            areaEntered = true; // 标记已进入

            if (showDebug) Debug.Log($"[{TaskName}] 玩家进入区域，触发任务");

            // 启动任务（只触发一次）
            if (!taskStarted)
            {
                StartTask(); // 调用基类的StartTask方法
            }

            // 如果设置为只触发一次，禁用此组件
            if (triggerOnce)
            {
                enabled = false; // 禁用触发器，不再检测
            }
        }
    }
}