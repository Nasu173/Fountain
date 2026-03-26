using UnityEngine;

/// <summary>
/// 脚本广播触发器
/// 由其他脚本通过GameEventBus广播ScriptTriggerEvent来触发任务
/// </summary>
public class ScriptTrigger : BaseTaskTrigger
{
    [Header("脚本触发配置")]
    [Tooltip("触发器标识符，用于区分多个ScriptTrigger（留空则响应所有广播）")]
    [SerializeField] private string triggerId;
    [Tooltip("可选：关联的对话ID，如果设置了则只有广播中DialogueID匹配时才触发")]
    [SerializeField] private string dialogueId; 

    private int currentProgress = 0;

    protected override int CurrentProgress
    {
        get => currentProgress;
        set => currentProgress = value;
    }

    protected override bool CheckTriggerCondition() => false;

    protected override void Update()
    {
        base.Update();
        if (debugMode && Input.GetKeyDown(KeyCode.L))
        {
            if (!taskStarted) StartTask();
        }
    }

    protected override int GetProgressAmount() => 1;

    protected override void IncrementProgress()
    {
        currentProgress++;
    }

    protected override void Start()
    {
        base.Start();
        GameEventBus.Subscribe<ScriptTriggerEvent>(OnScriptTriggerEvent);
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<ScriptTriggerEvent>(OnScriptTriggerEvent);
    }

    private void OnScriptTriggerEvent(ScriptTriggerEvent e)
    {
        if (taskCompleted) return;

        // 如果设置了triggerId，只响应匹配的广播
        if (!string.IsNullOrEmpty(triggerId) && e.TriggerId != triggerId) return;
        // 如果设置了dialogueId，只响应匹配的广播
        if (!string.IsNullOrEmpty(dialogueId) && e.DialogueID != dialogueId) return;

        if (!taskStarted)
        {
            StartTask();
        }

        //UpdateTaskProgress();

        if (currentProgress >= TargetCount)
        {
            OnTaskCompleted();
        }
    }
}
