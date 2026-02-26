using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void AddTask(string taskId, string taskName, int targetCount, string description, string taskNumber = "")
    {
        if (debugMode) Debug.Log($"AddTask called: {taskName}, ID: {taskId}");

        if (!activeTasks.ContainsKey(taskId))
        {
            TaskData newTask = new TaskData(taskName, targetCount, description);
            activeTasks.Add(taskId, newTask);

            if (debugMode) Debug.Log($"Task added to dictionary, total tasks: {activeTasks.Count}");

            CreateTaskUI(taskId, newTask, taskNumber);
        }
        else
        {
            if (debugMode) Debug.LogWarning($"Task ID already exists: {taskId}");
        }
    }

    public void UpdateTaskProgress(string taskId, int amount)
    {
        if (debugMode) Debug.Log($"UpdateTaskProgress: {taskId}, amount: {amount}");

        if (activeTasks.ContainsKey(taskId))
        {
            TaskData task = activeTasks[taskId];
            bool wasCompleted = task.isCompleted;

            task.UpdateProgress(amount);

            if (debugMode) Debug.Log($"Task progress updated: {task.currentCount}/{task.targetCount}, completed: {task.isCompleted}");

            if (activeTaskUIs.ContainsKey(taskId))
            {
                TaskUI taskUI = activeTaskUIs[taskId];
                if (taskUI != null)
                {
                    taskUI.UpdateDisplay(task);

                    // 如果任务完成，播放完成动画
                    if (!wasCompleted && task.isCompleted)
                    {
                        if (debugMode) Debug.Log("Task completed, playing completion animation");
                        taskUI.Complete();
                        StartCoroutine(DelayedRemove(taskId));
                    }
                }
            }
        }
        else
        {
            if (debugMode) Debug.LogWarning($"Task ID not found: {taskId}");
        }
    }

    private void CreateTaskUI(string taskId, TaskData taskData, string taskNumber)
    {
        if (debugMode)
        {
            string prefabStatus = "null";
            if (taskUIPrefab != null) prefabStatus = "assigned";

            string containerStatus = "null";
            if (taskUIContainer != null) containerStatus = "assigned";

            Debug.Log($"CreateTaskUI - Prefab: {prefabStatus}, Container: {containerStatus}");
        }

        if (taskUIPrefab != null && taskUIContainer != null)
        {
            GameObject uiObj = Instantiate(taskUIPrefab, taskUIContainer);

            string objStatus = "null";
            if (uiObj != null) objStatus = "created";
            if (debugMode) Debug.Log($"UI instantiated: {objStatus}");

            if (uiObj != null)
            {
                // 确保UI对象是激活的
                uiObj.SetActive(true);

                TaskUI taskUI = uiObj.GetComponent<TaskUI>();

                string uiComponentStatus = "null";
                if (taskUI != null) uiComponentStatus = "found";
                if (debugMode) Debug.Log($"TaskUI component: {uiComponentStatus}");

                if (taskUI != null)
                {
                    string number = taskNumber;
                    if (string.IsNullOrEmpty(number))
                    {
                        number = (activeTasks.Count).ToString();
                    }

                    taskUI.Initialize(taskData, number);
                    activeTaskUIs.Add(taskId, taskUI);

                    if (debugMode) Debug.Log($"UI initialized and added to dictionary. Total UIs: {activeTaskUIs.Count}");
                }
            }
        }
    }

    private IEnumerator DelayedRemove(string taskId)
    {
        if (debugMode) Debug.Log($"DelayedRemove started for {taskId}");

        // 等待一段时间让玩家看到完成动画
        yield return new WaitForSeconds(2f);

        if (debugMode) Debug.Log($"Removing task {taskId}");

        if (activeTaskUIs.ContainsKey(taskId))
        {
            TaskUI ui = activeTaskUIs[taskId];
            if (ui != null)
            {
                ui.Hide();
                yield return new WaitForSeconds(0.5f); // 等待淡出动画

                if (ui.gameObject != null)
                    Destroy(ui.gameObject);
            }
            activeTaskUIs.Remove(taskId);
        }

        if (activeTasks.ContainsKey(taskId))
        {
            activeTasks.Remove(taskId);
        }

        if (debugMode) Debug.Log($"Task {taskId} removed");
    }

    // 调试方法：获取所有活动任务
    public Dictionary<string, TaskData> GetActiveTasks()
    {
        return activeTasks;
    }

    // 调试方法：获取所有活动UI
    public Dictionary<string, TaskUI> GetActiveTaskUIs()
    {
        return activeTaskUIs;
    }
}