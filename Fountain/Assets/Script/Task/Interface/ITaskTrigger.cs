public interface ITaskTrigger
{
    string TaskId { get; }
    string TaskName { get; }
    string TaskNumber { get; }
    int TargetCount { get; }
    string Description { get; }
}