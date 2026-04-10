# Design: Audio Event System

## Architecture

```
触发脚本                    事件总线              AudioManager
InteractPlaySound  ──┐
EnterPlaySound     ──┼──► GameEventBus.Publish<PlaySoundEvent> ──► AudioManager.OnPlaySound()
EventPlaySound     ──┘                                                    │
PlaySoundPerform   ──┘                                              AudioSource.PlayOneShot()
                                                                    (BGM / PlayerFootstep / Fountain1 / Other / MonsterFootstep / Fountain2 音轨)
```

## 事件定义

在 `GameEventList.cs` 末尾追加：

```csharp
public enum AudioTrack { BGM, PlayerFootstep, Fountain1, Other, MonsterFootstep, Fountain2 }

[System.Serializable]
public class PlaySoundEvent : IGameEvent
{
    public AudioClip Clip;
    public AudioTrack Track;
    public float Volume; // 0~1，默认 1
}
```

## AudioManager

路径：`Assets/Script/Audio/AudioManager.cs`

- 单例 MonoBehaviour，`DontDestroyOnLoad`
- 持有 6 个 `AudioSource`，分别对应 BGM、PlayerFootstep、Fountain1、Other、MonsterFootstep、Fountain2 的 AudioMixerGroup
- `Awake`：Subscribe `PlaySoundEvent`
- `OnDestroy`：Unsubscribe
- `OnPlaySound(PlaySoundEvent e)`：按 `e.Track` 选择对应 source，调用 `PlayOneShot(e.Clip, e.Volume)`

## 三种触发脚本

### 1. InteractPlaySound（交互式）
路径：`Assets/Script/Audio/InteractPlaySound.cs`

- Inspector 字段：`AudioClip clip`、`AudioTrack track`、`KeyCode key`（默认 E）
- `Update()`：检测按键，发布 `PlaySoundEvent`

### 2. EnterPlaySound（进入式）
路径：`Assets/Script/Audio/EnterPlaySound.cs`

- Inspector 字段：`AudioClip clip`、`AudioTrack track`、`string targetTag`（默认 "Player"）
- `OnTriggerEnter(Collider other)`：tag 匹配则发布 `PlaySoundEvent`

### 3. EventPlaySound（事件监听式）
路径：`Assets/Script/Audio/EventPlaySound.cs`

- Inspector 字段：`AudioClip clip`、`AudioTrack track`、`string listenEventName`
- 通过反射按名称订阅 `IGameEvent`，收到事件时发布 `PlaySoundEvent`
- 支持的事件名：`GameEventList.cs` 中任意已定义事件类名

> 注：EventPlaySound 使用反射订阅，避免为每种事件写重复代码。

## PlaySoundPerform 修复

将 `Perform()` 中的 `Debug.LogFormat` 替换为：
```csharp
GameEventBus.Publish(new PlaySoundEvent { Clip = /* 按名加载 */, Track = AudioTrack.SFX, Volume = 1f });
```
由于 `SoundData.soundStr` 是字符串引用，需用 `Resources.Load<AudioClip>` 加载。

## 文件清单

| 文件 | 操作 |
|------|------|
| `Assets/Script/GameEvent/GameEventList.cs` | 追加 `AudioTrack` 枚举 + `PlaySoundEvent` |
| `Assets/Script/Audio/AudioManager.cs` | 新建 |
| `Assets/Script/Audio/InteractPlaySound.cs` | 新建 |
| `Assets/Script/Audio/EnterPlaySound.cs` | 新建 |
| `Assets/Script/Audio/EventPlaySound.cs` | 新建 |
| `Assets/Script/Dialogue/Perform/PlaySoundPerform.cs` | 修改 `Perform()` |
