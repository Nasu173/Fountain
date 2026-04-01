# 设计：返回游戏时清除旧任务进度

## 变更范围

仅修改 `Assets/Script/Task/TaskManager.cs`，在 `OnMenuEvent` 方法中追加清理逻辑。

## 实现细节

### 修改 `OnMenuEvent`

当前实现只隐藏 `taskUIContainer`：

```csharp
private void OnMenuEvent(MenuEvent e)
{
    if (taskUIContainer != null)
        taskUIContainer.gameObject.SetActive(false);
}
```

修改后，额外销毁所有活跃任务 UI 并清空字典：

```csharp
private void OnMenuEvent(MenuEvent e)
{
    foreach (var ui in activeTaskUIs.Values)
        if (ui != null) Destroy(ui.gameObject);

    activeTaskUIs.Clear();
    activeTasks.Clear();

    if (taskUIContainer != null)
        taskUIContainer.gameObject.SetActive(false);
}
```

### 注意事项

- `DelayedRemove` 协程可能在 `OnMenuEvent` 触发后仍在运行。由于协程中会检查 `ui.gameObject != null` 再 `Destroy`，且字典已清空，`TryGetValue` 会返回 false，协程会安全地提前退出（`activeTasks.Remove` 对空字典无副作用）。
- 无需停止协程，现有的 null 检查已足够防御。
