using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务管理器（单例）
/// 监听 TaskStartEvent / TaskProgressEvent，负责任务数据和 UI 生命周期
/// </summary>
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject taskUIPrefab;
    [SerializeField] private Transform taskUIContainer;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private Dictionary<string, TaskData> activeTasks = new Dictionary<string, TaskData>();
    private Dictionary<string, TaskUI> activeTaskUIs = new Dictionary<string, TaskUI>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEventBus.Subscribe<TaskStartEvent>(OnTaskStart);
        GameEventBus.Subscribe<TaskProgressEvent>(OnTaskProgress);
        GameEventBus.Subscribe<MenuEvent>(OnMenuEvent);
        GameEventBus.Subscribe<GameStartEvent>(OnGameStart);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<TaskStartEvent>(OnTaskStart);
        GameEventBus.Unsubscribe<TaskProgressEvent>(OnTaskProgress);
        GameEventBus.Unsubscribe<MenuEvent>(OnMenuEvent);
        GameEventBus.Unsubscribe<GameStartEvent>(OnGameStart);
    }

    private void OnTaskStart(TaskStartEvent e)
    {
        if (debugMode) Debug.Log($"TaskManager: OnTaskStart {e.TaskName}, ID: {e.TaskId}");

        if (activeTasks.ContainsKey(e.TaskId))
        {
            if (debugMode) Debug.LogWarning($"Task ID already exists: {e.TaskId}");
            return;
        }

        TaskData newTask = new TaskData(e.TaskName, e.TargetCount, e.Description);
        activeTasks.Add(e.TaskId, newTask);
        CreateTaskUI(e.TaskId, newTask, e.TaskNumber);
    }

    private void OnTaskProgress(TaskProgressEvent e)
    {
        if (debugMode) Debug.Log($"TaskManager: OnTaskProgress {e.TaskId}, amount: {e.Amount}");

        if (!activeTasks.TryGetValue(e.TaskId, out TaskData task))
        {
            if (debugMode) Debug.LogWarning($"Task ID not found: {e.TaskId}");
            return;
        }

        bool wasCompleted = task.isCompleted;
        task.UpdateProgress(e.Amount);

        if (activeTaskUIs.TryGetValue(e.TaskId, out TaskUI taskUI) && taskUI != null)
        {
            taskUI.UpdateDisplay(task);

            if (!wasCompleted && task.isCompleted)
            {
                if (debugMode) Debug.Log("Task completed, playing completion animation");
                taskUI.Complete();
                StartCoroutine(DelayedRemove(e.TaskId));
            }
        }
    }

    private void CreateTaskUI(string taskId, TaskData taskData, string taskNumber)
    {
        if (taskUIPrefab == null || taskUIContainer == null) return;

        taskUIContainer.gameObject.SetActive(true);
        GameObject uiObj = Instantiate(taskUIPrefab, taskUIContainer);
        uiObj.SetActive(true);

        TaskUI taskUI = uiObj.GetComponent<TaskUI>();
        if (taskUI != null)
        {
            string number = string.IsNullOrEmpty(taskNumber) ? activeTasks.Count.ToString() : taskNumber;
            taskUI.Initialize(taskData, number);
            activeTaskUIs.Add(taskId, taskUI);
        }
    }

    private IEnumerator DelayedRemove(string taskId)
    {
        yield return new WaitForSeconds(2f);

        if (activeTaskUIs.TryGetValue(taskId, out TaskUI ui) && ui != null)
        {
            if (ui.gameObject.activeInHierarchy)
            {
                ui.Hide();
                yield return new WaitForSeconds(0.5f);
            }
            if (ui != null) Destroy(ui.gameObject);
        }
        activeTaskUIs.Remove(taskId);
        activeTasks.Remove(taskId);

        if (debugMode) Debug.Log($"Task {taskId} removed");
    }

    public Dictionary<string, TaskData> GetActiveTasks() => new Dictionary<string, TaskData>(activeTasks);
    public Dictionary<string, TaskUI> GetActiveTaskUIs() => new Dictionary<string, TaskUI>(activeTaskUIs);

    private void OnMenuEvent(MenuEvent e)
    {
        foreach (var ui in activeTaskUIs.Values)
            if (ui != null) Destroy(ui.gameObject);

        activeTaskUIs.Clear();
        activeTasks.Clear();

        if (taskUIContainer != null)
            taskUIContainer.gameObject.SetActive(false);
    }

    private void OnGameStart(GameStartEvent e)
    {
        if (taskUIContainer != null)
            taskUIContainer.gameObject.SetActive(true);
    }
}
