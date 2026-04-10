using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 监听指定 IGameEvent（按类名），收到时播放音效。
/// listenEventName 填 GameEventList.cs 中任意事件类名，如 "TaskCompleteEvent"。
/// </summary>
public class EventPlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioTrack track = AudioTrack.Other;
    [SerializeField] private string listenEventName;

    private Action _unsubscribe;

    private void OnEnable()
    {
        Type eventType = Type.GetType(listenEventName);
        if (eventType == null || !typeof(IGameEvent).IsAssignableFrom(eventType))
        {
            Debug.LogWarning($"[EventPlaySound] 找不到事件类型: {listenEventName}");
            return;
        }

        var subscribeMethod = typeof(GameEventBus)
            .GetMethod(nameof(GameEventBus.Subscribe))
            .MakeGenericMethod(eventType);

        var handlerType = typeof(Action<>).MakeGenericType(eventType);
        var playMethod = typeof(EventPlaySound)
            .GetMethod(nameof(Play), BindingFlags.NonPublic | BindingFlags.Instance)
            .MakeGenericMethod(eventType);
        var handler = Delegate.CreateDelegate(handlerType, this, playMethod);

        subscribeMethod.Invoke(null, new object[] { handler });

        var unsubscribeMethod = typeof(GameEventBus)
            .GetMethod(nameof(GameEventBus.Unsubscribe))
            .MakeGenericMethod(eventType);
        _unsubscribe = () => unsubscribeMethod.Invoke(null, new object[] { handler });
    }

    private void OnDisable() => _unsubscribe?.Invoke();

    private void Play<T>(T _) where T : IGameEvent =>
        GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });
}
