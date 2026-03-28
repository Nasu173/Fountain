using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTask : MonoBehaviour
{
    public TaskInteractable interactable;
    /// <summary>
    /// 这两个的下标一一对应
    /// </summary>
    public string[] taskID;
    public BaseTaskTrigger[] tasks;
    private void OnEnable()
    {
        GameEventBus.Subscribe<TaskStartEvent>(Switch);
    }
    private void OnDisable()
    {
        GameEventBus.Unsubscribe<TaskStartEvent>(Switch);
    }
    private void Switch(TaskStartEvent e)
    {
        int i = 0;
        for (i = 0; i < taskID.Length; i++)
        {
            if (taskID[i] == e.TaskId)
            {
                interactable.taskTrigger = this.tasks[i];
                break;
            }
        }

    }
}
