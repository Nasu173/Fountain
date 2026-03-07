using UnityEngine;

/// <summary>
/// 任务系统简单测试脚本
/// 用于在游戏运行时测试任务系统的各项功能
/// </summary>
public class SimpleTaskTest : MonoBehaviour
{
    private int pressCount = 0;          // 按键计数器
    private string taskId = "test_task_1"; // 测试任务ID

    /// <summary>
    /// 绘制GUI界面，显示当前任务状态和操作提示
    /// </summary>
    void OnGUI()
    {
        // 在屏幕左上角创建信息面板
        GUILayout.BeginArea(new Rect(10, 10, 350, 300));

        // 标题
        GUILayout.Label("=== Task System Test ===", GUI.skin.box);
        GUILayout.Label($"Press T count: {pressCount}");

        // 检查TaskManager是否可用
        if (TaskManager.Instance != null)
        {
            GUILayout.Label("✓ TaskManager: OK");

            // 显示活动任务信息
            var activeTasks = TaskManager.Instance.GetActiveTasks();
            if (activeTasks != null)
            {
                GUILayout.Label($"Active Tasks: {activeTasks.Count}");

                // 显示当前测试任务的具体信息
                if (activeTasks.ContainsKey(taskId))
                {
                    TaskData task = activeTasks[taskId];
                    if (task != null)
                    {
                        GUILayout.Label($"Current Task: {task.taskName}");
                        GUILayout.Label($"Progress: {task.currentCount}/{task.targetCount}");
                        GUILayout.Label($"Completed: {task.isCompleted}");
                    }
                }
            }

            // 显示活动UI数量
            var activeUIs = TaskManager.Instance.GetActiveTaskUIs();
            if (activeUIs != null)
            {
                GUILayout.Label($"Active UIs: {activeUIs.Count}");
            }
        }
        else
        {
            GUILayout.Label("✗ TaskManager: NULL!", GUI.skin.box);
        }

        // 操作提示
        GUILayout.Space(10);
        GUILayout.Label("Controls:", GUI.skin.box);
        GUILayout.Label("T - Add/Update Task");
        GUILayout.Label("R - Reset counter");
        GUILayout.Label("V - Force Show all UIs");
        GUILayout.Label("F9 - Log debug info");

        GUILayout.EndArea();
    }

    /// <summary>
    /// 每帧更新，处理键盘输入
    /// </summary>
    void Update()
    {
        // T键：添加/更新任务
        if (Input.GetKeyDown(KeyCode.T))
        {
            pressCount++;
            Debug.Log($"=== Press T #{pressCount} ===");

            // 检查TaskManager是否存在
            if (TaskManager.Instance == null)
            {
                Debug.LogError("TaskManager.Instance is null!");
                return;
            }

            if (pressCount == 1)
            {
                // 第一次按下：添加新任务
                Debug.Log("Adding new task...");
                TaskManager.Instance.AddTask(
                    taskId,                    // 任务ID
                    "Collect Gems",             // 任务名称
                    5,                          // 目标数量
                    "Collect 5 gems to complete the task", // 任务描述
                    "1"                         // 任务编号
                );
            }
            else
            {
                // 后续按下：更新任务进度
                int amount = 1;
                Debug.Log($"Updating task progress by {amount}...");
                TaskManager.Instance.UpdateTaskProgress(taskId, amount);
            }
        }

        // R键：重置计数器
        if (Input.GetKeyDown(KeyCode.R))
        {
            pressCount = 0;
            Debug.Log("Test counter reset");
        }

        // V键：强制显示所有TaskUI（用于调试显示问题）
        if (Input.GetKeyDown(KeyCode.V))
        {
            // 查找场景中所有的TaskUI组件
            TaskUI[] allUIs = FindObjectsOfType<TaskUI>();
            Debug.Log($"Found {allUIs.Length} TaskUI components");

            for (int i = 0; i < allUIs.Length; i++)
            {
                TaskUI ui = allUIs[i];
                if (ui != null)
                {
                    Debug.Log($"Forcing show for {ui.gameObject.name}");
                    ui.ForceShow(); // 调用TaskUI的强制显示方法
                }
            }
        }

        // F9键：输出详细的调试信息
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("=== Debug Info ===");

            // 查找所有Canvas并输出信息
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            Debug.Log($"Canvases found: {canvases.Length}");

            for (int i = 0; i < canvases.Length; i++)
            {
                Canvas canvas = canvases[i];
                if (canvas != null)
                {
                    Debug.Log($"Canvas {i}: {canvas.name}, RenderMode: {canvas.renderMode}, SortOrder: {canvas.sortingOrder}");
                }
            }

            // 查找所有TaskUI并输出信息
            TaskUI[] taskUIs = FindObjectsOfType<TaskUI>();
            Debug.Log($"TaskUIs found: {taskUIs.Length}");

            for (int i = 0; i < taskUIs.Length; i++)
            {
                TaskUI ui = taskUIs[i];
                if (ui != null)
                {
                    Debug.Log($"TaskUI {i}: {ui.gameObject.name}, Active: {ui.gameObject.activeInHierarchy}, Position: {ui.GetComponent<RectTransform>().anchoredPosition}");
                }
            }
        }
    }
}