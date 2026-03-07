using UnityEngine;

/// <summary>
/// 物品收集触发器
/// 当玩家进入触发器并收集指定标签的物品时触发任务进度更新
/// </summary>
public class CollectItemTrigger : BaseTaskTrigger
{
    [Header("Collect Settings")]
    [Tooltip("可收集物品的标签，只有带有此标签的物品才会被收集")]
    [SerializeField] private string itemTag = "Collectible";

    [Tooltip("收集后是否销毁物品对象")]
    [SerializeField] private bool destroyOnCollect = true;

    private int itemsCollected = 0;          // 总共收集的物品数量
    private int lastCollectedCount = 0;       // 上次更新时的收集数量
    private int currentProgress = 0;          // 当前任务进度

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
    /// 获取自上次更新以来收集的物品数量
    /// </summary>
    /// <returns>本次要增加的进度值</returns>
    protected override int GetProgressAmount()
    {
        // 计算新收集的物品数量
        int collected = itemsCollected - lastCollectedCount;
        if (collected > 0)
        {
            lastCollectedCount = itemsCollected; // 更新上次计数
        }
        return collected;
    }

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
        // 检查进入的对象是否为可收集物品
        if (other != null && other.CompareTag(itemTag))
        {
            itemsCollected++; // 增加收集计数

            // 如果任务尚未开始，则启动任务
            if (!taskStarted)
            {
                StartTask();
            }

            // 如果任务尚未完成，更新进度
            if (!taskCompleted)
            {
                UpdateTaskProgress();
            }

            // 如果设置为收集后销毁，则销毁物品
            if (destroyOnCollect && other.gameObject != null)
            {
                Destroy(other.gameObject);
            }

            // 检查任务是否达到完成条件
            if (currentProgress >= TargetCount)
            {
                OnTaskCompleted(); // 触发任务完成回调
            }
        }
    }
}