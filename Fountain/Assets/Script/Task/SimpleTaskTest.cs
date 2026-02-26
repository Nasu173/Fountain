using UnityEngine;

public class SimpleTaskTest : MonoBehaviour
{
    private int pressCount = 0;
    private string taskId = "test_task_1";

    void OnGUI()
    {
        // 在屏幕上显示当前状态
        GUILayout.BeginArea(new Rect(10, 10, 350, 300));
        GUILayout.Label("=== Task System Test ===", GUI.skin.box);
        GUILayout.Label($"Press T count: {pressCount}");

        if (TaskManager.Instance != null)
        {
            GUILayout.Label("✓ TaskManager: OK");

            // 显示活动任务
            var activeTasks = TaskManager.Instance.GetActiveTasks();
            if (activeTasks != null)
            {
                GUILayout.Label($"Active Tasks: {activeTasks.Count}");

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

            // 显示活动UI
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

        GUILayout.Space(10);
        GUILayout.Label("Controls:", GUI.skin.box);
        GUILayout.Label("T - Add/Update Task");
        GUILayout.Label("R - Reset counter");
        GUILayout.Label("V - Force Show all UIs");
        GUILayout.Label("F9 - Log debug info");

        GUILayout.EndArea();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            pressCount++;
            Debug.Log($"=== Press T #{pressCount} ===");

            if (TaskManager.Instance == null)
            {
                Debug.LogError("TaskManager.Instance is null!");
                return;
            }

            if (pressCount == 1)
            {
                // 第一次：添加任务
                Debug.Log("Adding new task...");
                TaskManager.Instance.AddTask(
                    taskId,
                    "Collect Gems",
                    5,
                    "Collect 5 gems to complete the task",
                    "1"
                );
            }
            else
            {
                // 后续：更新进度
                int amount = 1;
                Debug.Log($"Updating task progress by {amount}...");
                TaskManager.Instance.UpdateTaskProgress(taskId, amount);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            pressCount = 0;
            Debug.Log("Test counter reset");
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            // 强制查找所有TaskUI并显示
            TaskUI[] allUIs = FindObjectsOfType<TaskUI>();
            Debug.Log($"Found {allUIs.Length} TaskUI components");

            for (int i = 0; i < allUIs.Length; i++)
            {
                TaskUI ui = allUIs[i];
                if (ui != null)
                {
                    Debug.Log($"Forcing show for {ui.gameObject.name}");
                    ui.ForceShow();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("=== Debug Info ===");

            // 查找所有Canvas
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

            // 查找所有TaskUI
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