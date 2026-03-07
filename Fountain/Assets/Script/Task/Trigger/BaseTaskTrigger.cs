using UnityEngine;

/// <summary>
/// 任务触发器基类，所有具体任务触发器都应继承自此类
/// </summary>
public abstract class BaseTaskTrigger : MonoBehaviour, ITaskTrigger
{
    [Header("Task Configuration")]
    [Tooltip("任务的唯一标识符，如果留空会自动生成GUID")]
    [SerializeField] protected string taskId;

    [Tooltip("任务名称，显示在UI上")]
    [SerializeField] protected string taskName;

    [Tooltip("任务编号，显示在任务名称前，如'#1'")]
    [SerializeField] protected string taskNumber = "1";

    [Tooltip("任务目标数量，达到此数量任务完成")]
    [SerializeField] protected int targetCount = 1;

    [Tooltip("任务描述，显示在UI上")]
    [SerializeField][TextArea] protected string description;

    [Header("任务链设置")]
    [Tooltip("任务完成后自动触发的下一个任务触发器")]
    [SerializeField] protected BaseTaskTrigger nextTaskTrigger;

    [Tooltip("延迟多少秒后触发下一个任务")]
    [SerializeField] protected float nextTaskDelay = 0.5f;

    [Header("Debug")]
    [Tooltip("是否输出调试日志")]
    [SerializeField] protected bool debugMode = true;

    /// <summary>
    /// 获取任务ID，如果为空则自动生成
    /// </summary>
    public virtual string TaskId
    {
        get
        {
            if (string.IsNullOrEmpty(taskId))
                taskId = System.Guid.NewGuid().ToString(); // 生成唯一ID
            return taskId;
        }
    }

    /// <summary>
    /// 获取任务名称
    /// </summary>
    public virtual string TaskName => taskName;

    /// <summary>
    /// 获取任务编号
    /// </summary>
    public virtual string TaskNumber => taskNumber;

    /// <summary>
    /// 获取目标数量
    /// </summary>
    public virtual int TargetCount => targetCount;

    /// <summary>
    /// 获取任务描述
    /// </summary>
    public virtual string Description => description;

    /// <summary>
    /// 当前进度，由子类实现具体存储方式
    /// </summary>
    protected virtual int CurrentProgress { get; set; }

    // 任务状态标志
    protected bool taskStarted = false;      // 任务是否已开始
    protected bool taskCompleted = false;    // 任务是否已完成

    /// <summary>
    /// 初始化任务ID和调试信息
    /// </summary>
    protected virtual void Start()
    {
        // 确保taskId不为空
        if (string.IsNullOrEmpty(taskId))
        {
            taskId = System.Guid.NewGuid().ToString();
        }

        if (debugMode) Debug.Log($"[{GetType().Name}] Started: {taskName}, ID: {taskId}");
    }

    /// <summary>
    /// 检查触发条件是否满足
    /// </summary>
    /// <returns>是否满足触发条件</returns>
    protected abstract bool CheckTriggerCondition();

    /// <summary>
    /// 获取本次要增加的进度值
    /// </summary>
    /// <returns>增加的进度数量</returns>
    protected abstract int GetProgressAmount();

    /// <summary>
    /// 增加进度计数（子类需实现具体的进度存储方式）
    /// </summary>
    protected abstract void IncrementProgress();

    /// <summary>
    /// 每帧检查触发条件并更新任务状态
    /// </summary>
    protected virtual void Update()
    {
        // 任务未完成且满足触发条件时
        if (!taskCompleted && CheckTriggerCondition())
        {
            if (!taskStarted)
            {
                StartTask(); // 首次触发：开始任务
            }
            else
            {
                UpdateTaskProgress(); // 后续触发：更新进度
            }
        }
    }

    /// <summary>
    /// 开始任务，向TaskManager注册新任务
    /// </summary>
    protected virtual void StartTask()
    {
        taskStarted = true;
        if (TaskManager.Instance != null)
        {
            // 通过TaskManager添加任务UI
            TaskManager.Instance.AddTask(TaskId, TaskName, TargetCount, Description, TaskNumber);
            if (debugMode) Debug.Log($"[{GetType().Name}] Task started: {taskName}");
        }
        else
        {
            if (debugMode) Debug.LogError($"[{GetType().Name}] TaskManager.Instance is null!");
        }
    }

    /// <summary>
    /// 更新任务进度
    /// </summary>
    protected virtual void UpdateTaskProgress()
    {
        if (taskStarted && !taskCompleted)
        {
            int amount = GetProgressAmount(); // 获取本次要增加的进度
            if (amount > 0)
            {
                // 循环增加进度（支持一次增加多个进度）
                for (int i = 0; i < amount; i++)
                {
                    IncrementProgress();
                }

                // 通知TaskManager更新UI
                if (TaskManager.Instance != null)
                {
                    TaskManager.Instance.UpdateTaskProgress(TaskId, amount);
                    if (debugMode) Debug.Log($"[{GetType().Name}] Task progress updated: +{amount}");
                }
            }
        }
    }

    /// <summary>
    /// 任务完成时的回调
    /// </summary>
    protected virtual void OnTaskCompleted()
    {
        taskCompleted = true;
        if (debugMode) Debug.Log($"[{GetType().Name}] Task completed: {taskName}");

        // 任务链：触发下一个任务
        if (nextTaskTrigger != null)
        {
            if (nextTaskDelay > 0)
            {
                Invoke(nameof(StartNextTask), nextTaskDelay); // 延迟启动
            }
            else
            {
                StartNextTask(); // 立即启动
            }
        }
    }

    /// <summary>
    /// 启动下一个任务（由Invoke调用）
    /// </summary>
    private void StartNextTask()
    {
        if (nextTaskTrigger != null)
        {
            // 通过反射调用下一个任务的StartTask方法（protected方法）
            var method = typeof(BaseTaskTrigger).GetMethod("StartTask",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(nextTaskTrigger, null); // 调用下一个任务的StartTask
                if (debugMode) Debug.Log($"[{GetType().Name}] Triggered next task: {nextTaskTrigger.TaskName}");
            }
        }
    }

    /// <summary>
    /// 设置下一个任务（支持链式调用）
    /// </summary>
    /// <param name="nextTrigger">下一个任务的触发器</param>
    /// <param name="delay">延迟时间（秒）</param>
    /// <returns>返回当前触发器，支持链式调用</returns>
    public BaseTaskTrigger Then(BaseTaskTrigger nextTrigger, float delay = 0.5f)
    {
        nextTaskTrigger = nextTrigger;
        nextTaskDelay = delay;
        return this; // 返回自身以便继续链式调用
    }
}