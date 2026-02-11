using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventBus : MonoBehaviour
{
    // 存储所有事件类型和对应的处理器
    private static readonly Dictionary<Type, List<Delegate>> _handlers = new();

    // 用于调试：记录最近的事件
    private static readonly List<string> _eventHistory = new();
    private const int MAX_HISTORY = 50;

    // ==================== 订阅事件 ====================
    /// <summary>
    /// 订阅事件（告诉事件总线："当XX事件发生时，调用这个方法"）
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (!_handlers.ContainsKey(eventType))
        {
            _handlers[eventType] = new List<Delegate>();
        }

        // 检查是否已经订阅过
        if (!_handlers[eventType].Contains(handler))
        {
            _handlers[eventType].Add(handler);
            Debug.Log($"✅ 已订阅事件: {eventType.Name}");
        }
    }

    // ==================== 取消订阅 ====================
    /// <summary>
    /// 取消订阅事件（当物体被销毁或不需要接收事件时调用）
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
    {
        Type eventType = typeof(T);

        if (_handlers.ContainsKey(eventType))
        {
            _handlers[eventType].Remove(handler);
            Debug.Log($"❌ 已取消订阅: {eventType.Name}");

            // 如果这个事件类型没有订阅者了，删除这个条目
            if (_handlers[eventType].Count == 0)
            {
                _handlers.Remove(eventType);
            }
        }
    }

    // ==================== 发布事件 ====================
    /// <summary>
    /// 发布事件（告诉事件总线："XX事件发生了！请通知所有关心的人"）
    /// </summary>
    public static void Publish<T>(T eventData) where T : IGameEvent
    {
        Type eventType = typeof(T);

        // 记录事件历史（用于调试）
        string log = $"[{DateTime.Now:HH:mm:ss}] {eventType.Name}";
        _eventHistory.Add(log);

        // 限制历史记录数量
        if (_eventHistory.Count > MAX_HISTORY)
        {
            _eventHistory.RemoveAt(0);
        }

        // 检查是否有订阅者
        if (!_handlers.ContainsKey(eventType))
        {
            Debug.Log($"📭 事件 {eventType.Name} 发布了，但没有订阅者");
            return;
        }

        // 通知所有订阅者
        List<Delegate> handlers = _handlers[eventType];
        int handlerCount = handlers.Count;

        Debug.Log($"📢 发布事件: {eventType.Name} (有 {handlerCount} 个订阅者)");

        // 从后往前遍历，这样在遍历过程中可以安全地移除订阅者
        for (int i = handlers.Count - 1; i >= 0; i--)
        {
            try
            {
                Action<T> handler = handlers[i] as Action<T>;
                handler?.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogError($"⚠️ 执行事件处理器时出错 ({eventType.Name}): {e.Message}");
            }
        }
    }

    // ==================== 调试功能 ====================
    /// <summary>
    /// 获取事件历史记录（用于调试）
    /// </summary>
    public static List<string> GetEventHistory()
    {
        return new List<string>(_eventHistory);
    }

    /// <summary>
    /// 清空所有订阅（切换场景或重置游戏时调用）
    /// </summary>
    public static void ClearAllSubscriptions()
    {
        _handlers.Clear();
        _eventHistory.Clear();
        Debug.Log("🧹 已清空所有事件订阅");
    }

    /// <summary>
    /// 获取当前所有活跃的订阅信息
    /// </summary>
    public static Dictionary<string, int> GetSubscriptionStats()
    {
        var stats = new Dictionary<string, int>();

        foreach (var kvp in _handlers)
        {
            stats[kvp.Key.Name] = kvp.Value.Count;
        }

        return stats;
    }
}
