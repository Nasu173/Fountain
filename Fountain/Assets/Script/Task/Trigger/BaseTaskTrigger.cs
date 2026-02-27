using UnityEngine;

public abstract class BaseTaskTrigger : MonoBehaviour, ITaskTrigger
{
    [Header("Task Configuration")]
    [SerializeField] protected string taskId;
    [SerializeField] protected string taskName;
    [SerializeField] protected string taskNumber = "1";
    [SerializeField] protected int targetCount = 1;
    [SerializeField][TextArea] protected string description;

    [Header("Debug")]
    [SerializeField] protected bool debugMode = true;

    // 使用virtual允许子类重写
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

    protected virtual void Start()
    {
        if (string.IsNullOrEmpty(taskId))
        {
            taskId = System.Guid.NewGuid().ToString();
        }

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
            {
                StartTask();
            }
            else
            {
                UpdateTaskProgress();
            }
        }
    }

    protected virtual void StartTask()
    {
        taskStarted = true;
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.AddTask(TaskId, TaskName, TargetCount, Description, TaskNumber);
            if (debugMode) Debug.Log($"[{GetType().Name}] Task started: {taskName}");
        }
        else
        {
            if (debugMode) Debug.LogError($"[{GetType().Name}] TaskManager.Instance is null!");
        }
    }

    protected virtual void UpdateTaskProgress()
    {
        if (taskStarted && !taskCompleted)
        {
            int amount = GetProgressAmount();
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    IncrementProgress();
                }

                if (TaskManager.Instance != null)
                {
                    TaskManager.Instance.UpdateTaskProgress(TaskId, amount);
                    if (debugMode) Debug.Log($"[{GetType().Name}] Task progress updated: +{amount}");
                }
            }
        }
    }

    protected virtual void OnTaskCompleted()
    {
        taskCompleted = true;
        if (debugMode) Debug.Log($"[{GetType().Name}] Task completed: {taskName}");
    }
}