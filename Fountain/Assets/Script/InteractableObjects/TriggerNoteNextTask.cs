using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerNoteNextTask : MonoBehaviour
{
    public int targetAmount;
    private int currentAmount = 0;
    public string taskCompleteId;
    private void OnEnable()
    {
        GameEventBus.Subscribe<TaskProgressEvent>(Count); 
    }
    private void OnDisable()
    {
        GameEventBus.Unsubscribe<TaskProgressEvent>(Count); 
    }
    private void Count(TaskProgressEvent e)
    {
        if (e.TaskId == taskCompleteId)
        {
            currentAmount += e.Amount;
            if (currentAmount >= targetAmount)
            {
                GameEventBus.Publish<ScriptTriggerEvent>
                    (new ScriptTriggerEvent() { TriggerId = "17" });
            }
        }
    }
}
