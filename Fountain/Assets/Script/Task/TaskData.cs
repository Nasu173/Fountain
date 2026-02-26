using System;
using UnityEngine;

[Serializable]
public class TaskData
{
    public string taskName;
    public int targetCount;
    public int currentCount;
    public string description;
    public bool isCompleted;

    public TaskData(string name, int target, string desc)
    {
        taskName = name;
        targetCount = target;
        currentCount = 0;
        description = desc;
        isCompleted = false;
    }

    public void UpdateProgress(int amount)
    {
        if (!isCompleted)
        {
            currentCount = Mathf.Min(currentCount + amount, targetCount);
            if (currentCount >= targetCount)
            {
                isCompleted = true;
            }
        }
    }

    public float GetProgressPercentage()
    {
        if (targetCount <= 0) return 0f;
        return (float)currentCount / targetCount;
    }

    public string GetProgressText()
    {
        return currentCount + "/" + targetCount;
    }
}