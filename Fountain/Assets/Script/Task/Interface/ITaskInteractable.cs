using UnityEngine;

public interface ITaskInteractable
{
    string GetTaskId();
    InteractConfig GetInteractConfig();
}

[System.Serializable]
public class InteractConfig
{
    public bool canInteractMultipleTimes = false;
    public bool progressOnlyFirstTime = true;
    public bool destroyOnInteract = false;
    public int progressPerInteract = 1;
}