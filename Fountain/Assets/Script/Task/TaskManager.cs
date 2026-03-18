using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务管理器
/// 单例模式，负责任务的激活、创建、删除和UI显示
/// </summary>
public class TaskManager : MonoBehaviour
{
    // 单例实例
    public static TaskManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("任务UI预制体")]
    [SerializeField] private GameObject taskUIPrefab;

    [Tooltip("任务UI的父容器")]
    [SerializeField] private Transform taskUIContainer;

    [Header("Debug")]
    [Tooltip("是否显示调试日志")]
    [SerializeField] private bool debugMode = true;

    // 存储所有活跃任务，key为taskId
    private Dictionary<string, TaskData> activeTasks = new Dictionary<string, TaskData>();

    // 存储所有活跃任务对应的UI，key为taskId
    private Dictionary<string, TaskUI> activeTaskUIs = new Dictionary<string, TaskUI>();

    /// <summary>
    /// 初始化单例实例
    /// </summary>
    private void Awake()
    {
        // 确保实例唯一性
        if (Instance == null)
        {
            Instance = this;           // 设置当前实例为单例
            DontDestroyOnLoad(gameObject); // 切换场景时不销毁
        }
        else
        {
            Destroy(gameObject);        // 已存在实例则销毁当前对象
        }
    }

    /// <summary>
    /// 添加新任务
    /// </summary>
    /// <param name="taskId">任务唯一标识符</param>
    /// <param name="taskName">任务名称</param>
    /// <param name="targetCount">目标数量</param>
    /// <param name="description">任务描述</param>
    /// <param name="taskNumber">任务编号（如"1"、"2"）</param>
    public void AddTask(string taskId, string taskName, int targetCount, string description, string taskNumber = "")
    {
        if (debugMode) Debug.Log($"AddTask called: {taskName}, ID: {taskId}");

        // 检查任务ID是否已存在
        if (!activeTasks.ContainsKey(taskId))
        {
            // 创建新的任务数据
            TaskData newTask = new TaskData(taskName, targetCount, description);
            activeTasks.Add(taskId, newTask); // 添加到活跃任务字典

            if (debugMode) Debug.Log($"Task added to dictionary, total tasks: {activeTasks.Count}");

            // 创建对应的任务UI
            CreateTaskUI(taskId, newTask, taskNumber);
        }
        else
        {
            if (debugMode) Debug.LogWarning($"Task ID already exists: {taskId}");
        }
    }

    /// <summary>
    /// 更新任务进度
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="amount">增加的进度值</param>
    public void UpdateTaskProgress(string taskId, int amount)
    {
        if (debugMode) Debug.Log($"UpdateTaskProgress: {taskId}, amount: {amount}");

        // 检查任务是否存在
        if (activeTasks.ContainsKey(taskId))
        {
            TaskData task = activeTasks[taskId];
            bool wasCompleted = task.isCompleted; // 记录更新前的状态

            // 更新任务进度
            task.UpdateProgress(amount);

            if (debugMode) Debug.Log($"Task progress updated: {task.currentCount}/{task.targetCount}, completed: {task.isCompleted}");

            // 更新对应的UI显示
            if (activeTaskUIs.ContainsKey(taskId))
            {
                TaskUI taskUI = activeTaskUIs[taskId];
                if (taskUI != null)
                {
                    taskUI.UpdateDisplay(task); // 刷新UI显示

                    // 如果刚刚更新完成，播放完成动画
                    if (!wasCompleted && task.isCompleted)
                    {
                        if (debugMode) Debug.Log("Task completed, playing completion animation");
                        taskUI.Complete(); // 播放完成动画
                        StartCoroutine(DelayedRemove(taskId)); // 延迟移除任务
                    }
                }
            }
        }
        else
        {
            if (debugMode) Debug.LogWarning($"Task ID not found: {taskId}");
        }
    }

    /// <summary>
    /// 创建任务UI
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="taskData">任务数据</param>
    /// <param name="taskNumber">任务编号</param>
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

        // 检查预制体和容器是否已配置
        if (taskUIPrefab != null && taskUIContainer != null)
        {
            // 实例化UI预制体
            GameObject uiObj = Instantiate(taskUIPrefab, taskUIContainer);

            string objStatus = "null";
            if (uiObj != null) objStatus = "created";
            if (debugMode) Debug.Log($"UI instantiated: {objStatus}");

            if (uiObj != null)
            {
                // 确保UI对象是激活的
                uiObj.SetActive(true);

                // 获取TaskUI组件
                TaskUI taskUI = uiObj.GetComponent<TaskUI>();

                string uiComponentStatus = "null";
                if (taskUI != null) uiComponentStatus = "found";
                if (debugMode) Debug.Log($"TaskUI component: {uiComponentStatus}");

                if (taskUI != null)
                {
                    // 若未指定任务编号，使用当前任务总数作为编号
                    string number = taskNumber;
                    if (string.IsNullOrEmpty(number))
                    {
                        number = (activeTasks.Count).ToString();
                    }

                    // 初始化UI
                    taskUI.Initialize(taskData, number);

                    // 添加到UI字典
                    activeTaskUIs.Add(taskId, taskUI);

                    if (debugMode) Debug.Log($"UI initialized and added to dictionary. Total UIs: {activeTaskUIs.Count}");
                }
            }
        }
    }

    /// <summary>
    /// 延迟移除已完成的任务
    /// </summary>
    /// <param name="taskId">要移除的任务ID</param>
    /// <returns>协程的迭代器</returns>
    private IEnumerator DelayedRemove(string taskId)
    {
        if (debugMode) Debug.Log($"DelayedRemove started for {taskId}");

        // 等待一段时间以播放完成动画
        yield return new WaitForSeconds(2f);

        if (debugMode) Debug.Log($"Removing task {taskId}");

        // 移除UI
        if (activeTaskUIs.ContainsKey(taskId))
        {
            TaskUI ui = activeTaskUIs[taskId];
            if (ui != null)
            {
                ui.Hide(); // 播放隐藏动画
                yield return new WaitForSeconds(0.5f); // 等待动画完成

                if (ui.gameObject != null)
                    Destroy(ui.gameObject); // 销毁UI对象
            }
            activeTaskUIs.Remove(taskId); // 从UI字典中移除
        }

        // 移除任务数据
        if (activeTasks.ContainsKey(taskId))
        {
            activeTasks.Remove(taskId);
        }

        if (debugMode) Debug.Log($"Task {taskId} removed");
    }

    /// <summary>
    /// 供外部访问，获取所有活跃任务
    /// </summary>
    /// <returns>任务字典</returns>
    public Dictionary<string, TaskData> GetActiveTasks()
    {
        return activeTasks;
    }

    /// <summary>
    /// 供外部访问，获取所有活跃UI
    /// </summary>
    /// <returns>任务UI字典</returns>
    public Dictionary<string, TaskUI> GetActiveTaskUIs()
    {
        return activeTaskUIs;
    }
}
