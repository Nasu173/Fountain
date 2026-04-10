# Tasks: Audio Event System

## Task 1 ✅ — 添加 PlaySoundEvent 到事件列表
**文件**：`Assets/Script/GameEvent/GameEventList.cs`

在文件末尾追加：
```csharp
public enum AudioTrack { BGM, PlayerFootstep, Fountain1, Other, MonsterFootstep, Fountain2 }

[System.Serializable]
public class PlaySoundEvent : IGameEvent
{
    public AudioClip Clip;
    public AudioTrack Track;
    public float Volume = 1f;
}
```

---

## Task 2 ✅ — 创建 AudioManager
**文件**：`Assets/Script/Audio/AudioManager.cs`（新建目录 `Audio/`）

```csharp
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup playerFootstepGroup;
    [SerializeField] private AudioMixerGroup fountain1Group;
    [SerializeField] private AudioMixerGroup otherGroup;
    [SerializeField] private AudioMixerGroup monsterFootstepGroup;
    [SerializeField] private AudioMixerGroup fountain2Group;

    private readonly Dictionary<AudioTrack, AudioSource> _sources = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sources[AudioTrack.BGM]             = CreateSource(bgmGroup);
        _sources[AudioTrack.PlayerFootstep]  = CreateSource(playerFootstepGroup);
        _sources[AudioTrack.Fountain1]       = CreateSource(fountain1Group);
        _sources[AudioTrack.Other]           = CreateSource(otherGroup);
        _sources[AudioTrack.MonsterFootstep] = CreateSource(monsterFootstepGroup);
        _sources[AudioTrack.Fountain2]       = CreateSource(fountain2Group);

        GameEventBus.Subscribe<PlaySoundEvent>(OnPlaySound);
    }

    private void OnDestroy() => GameEventBus.Unsubscribe<PlaySoundEvent>(OnPlaySound);

    private void OnPlaySound(PlaySoundEvent e)
    {
        if (e.Clip == null) return;
        _sources[e.Track].PlayOneShot(e.Clip, e.Volume);
    }

    private AudioSource CreateSource(AudioMixerGroup group)
    {
        var src = gameObject.AddComponent<AudioSource>();
        src.outputAudioMixerGroup = group;
        src.playOnAwake = false;
        return src;
    }
}
```

**Inspector 配置**：将 AudioManager GameObject 放入常驻场景，分别拖入 BGM、PlayerFootstep、Fountain1、Other、MonsterFootstep、Fountain2 的 AudioMixerGroup（均为 SFX 的子音轨，BGM 除外）。

---

## Task 3 ✅ — 创建 InteractPlaySound（交互式）
**文件**：`Assets/Script/Audio/InteractPlaySound.cs`

```csharp
using UnityEngine;

public class InteractPlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioTrack track = AudioTrack.Other;
    [SerializeField] private KeyCode key = KeyCode.E;

    private void Update()
    {
        if (Input.GetKeyDown(key))
            GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });
    }
}
```

---

## Task 4 ✅ — 创建 EnterPlaySound（进入式）
**文件**：`Assets/Script/Audio/EnterPlaySound.cs`

```csharp
using UnityEngine;

public class EnterPlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioTrack track = AudioTrack.Other;
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
            GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });
    }
}
```

---

## Task 5 ✅ — 创建 EventPlaySound（事件监听式）
**文件**：`Assets/Script/Audio/EventPlaySound.cs`

```csharp
using System;
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

        // 用反射构造泛型 Subscribe<T> 调用
        var subscribeMethod = typeof(GameEventBus)
            .GetMethod(nameof(GameEventBus.Subscribe))
            .MakeGenericMethod(eventType);

        var handlerType = typeof(Action<>).MakeGenericType(eventType);
        var playMethod = typeof(EventPlaySound).GetMethod(nameof(Play), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var handler = Delegate.CreateDelegate(handlerType, this, playMethod.MakeGenericMethod(eventType));

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
```

---

## Task 6 ✅ — 修复 PlaySoundPerform
**文件**：`Assets/Script/Dialogue/Perform/PlaySoundPerform.cs`

将 `Perform()` 改为：
```csharp
public override void Perform()
{
    var clip = Resources.Load<AudioClip>(data.soundStr);
    if (clip == null) { Debug.LogWarning($"[PlaySoundPerform] 找不到音频: {data.soundStr}"); return; }
    GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = AudioTrack.Other });
}
```

> 注：音频文件需放在 `Assets/Resources/` 目录下，`soundStr` 填相对于 Resources 的路径（不含扩展名）。

---

## 验收标准

- [ ] 编译无错误
- [ ] AudioManager 在场景中存在且 DontDestroyOnLoad
- [ ] 三种触发脚本挂载后在 Inspector 可配置 clip / track
- [ ] 发布 `PlaySoundEvent` 后对应 AudioSource 播放音频
- [ ] `PlaySoundPerform` 能通过对话系统触发音效
