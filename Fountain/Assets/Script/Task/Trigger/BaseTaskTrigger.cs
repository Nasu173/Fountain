using UnityEngine;

/// <summary>
/// 任务触发器基类，所有具体任务触发器都应继承此类
/// </summary>
public abstract class BaseTaskTrigger : MonoBehaviour, ITaskTrigger
{
    [Header("Task Configuration")]
    [SerializeField] protected string taskId;
    [SerializeField] protected string taskName;
    [SerializeField] protected string taskNumber = "1";
    [SerializeField] protected int targetCount = 1;
    [SerializeField][TextArea] protected string description;

    [Header("任务链配置")]
    [SerializeField] protected BaseTaskTrigger nextTaskTrigger;
    [SerializeField] protected float nextTaskDelay = 0.5f;

    [Header("Debug")]
    [SerializeField] protected bool debugMode = true;

    public virtual string TaskId
    {
        get
        {
            if (string.IsNullOrEmpty(taskId))
                taskId = System.Guid.NewGuid().ToString();
            return taskId;
        }
    }

    public virtual string TaskName => taskName;
    public virtual string TaskNumber => taskNumber;
    public virtual int TargetCount => targetCount;
    public virtual string Description => description;

    protected virtual int CurrentProgress { get; set; }

    protected bool taskStarted = false;
    protected bool taskCompleted = false;

    /// <summary>供外部判断任务是否已开始（避免重复发布 TaskStartEvent）</summary>
    public bool TaskStarted => taskStarted;

    protected virtual void Start()
    {
        if (string.IsNullOrEmpty(taskId))
            taskId = System.Guid.NewGuid().ToString();

        if (debugMode) Debug.Log($"[{GetType().Name}] Started: {taskName}, ID: {taskId}");
    }

    protected abstract bool CheckTriggerCondition();
    protected abstract int GetProgressAmount();
    protected abstract void IncrementProgress();

    protected virtual void Update()
    {
        if (!taskCompleted && CheckTriggerCondition())
        {
            if (!taskStarted)
                StartTask();
            else
                UpdateTaskProgress();
        }
    }

    /// <summary>启动任务，发布 TaskStartEvent</summary>
    protected virtual void StartTask()
    {
        taskStarted = true;
        GameEventBus.Publish(new TaskStartEvent
        {
            TaskId = TaskId,
            TaskName = TaskName,
            TaskNumber = TaskNumber,
            TargetCount = TargetCount,
            Description = Description
        });
        if (debugMode) Debug.Log($"[{GetType().Name}] Task started: {taskName}");
    }

    /// <summary>供外部和任务链调用，替代反射</summary>
    public void TriggerStart()
    {
        if (!taskStarted) StartTask();
    }

    /// <summary>更新任务进度，发布 TaskProgressEvent</summary>
    protected virtual void UpdateTaskProgress()
    {
        if (taskStarted && !taskCompleted)
        {
            int amount = GetProgressAmount();
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                    IncrementProgress();

                GameEventBus.Publish(new TaskProgressEvent { TaskId = TaskId, Amount = amount });
                if (debugMode) Debug.Log($"[{GetType().Name}] Task progress updated: +{amount}");
            }
        }
    }

    protected virtual void OnTaskCompleted()
    {
        taskCompleted = true;
        if (debugMode) Debug.Log($"[{GetType().Name}] Task completed: {taskName}");

        if (nextTaskTrigger != null)
        {
            if (nextTaskDelay > 0)
                Invoke(nameof(StartNextTask), nextTaskDelay);
            else
                StartNextTask();
        }
    }

    private void StartNextTask()
    {
        nextTaskTrigger?.TriggerStart();
        if (debugMode) Debug.Log($"[{GetType().Name}] Triggered next task: {nextTaskTrigger?.TaskName}");
    }

    public BaseTaskTrigger Then(BaseTaskTrigger nextTrigger, float delay = 0.5f)
    {
        nextTaskTrigger = nextTrigger;
        nextTaskDelay = delay;
        return this;
    }
}
