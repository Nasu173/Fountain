# Design: Audio System Extension

## Architecture

```
PlayerMove                          事件总线                    AudioManager
  移动中 ──► PlaySoundEvent(isLoop=true)  ──► GameEventBus ──► source.clip = clip; source.loop = true; source.Play()
  停止   ──► PauseSoundEvent(track)       ──► GameEventBus ──► source.Pause()
  奔跑   ──► PlaySoundEvent(pitch=runPitch)                ──► source.pitch = pitch
```

## 1. GameEventList.cs 修改

### PlaySoundEvent 新增字段

```csharp
[System.Serializable]
public class PlaySoundEvent : IGameEvent
{
    public AudioClip Clip;
    public AudioTrack Track;
    public float Volume = 1f;
    public bool IsLoop = false;   // 新增：true = 循环播放(BGM)，false = 单次播放(SFX)
    public float Pitch = 1f;      // 新增：播放速度，奔跑时传入倍速值
}
```

### 新增 PauseSoundEvent

```csharp
[System.Serializable]
public class PauseSoundEvent : IGameEvent
{
    public AudioTrack Track;
}
```

## 2. AudioManager.cs 修改

- `Awake`：额外订阅 `PauseSoundEvent`
- `OnDestroy`：额外取消订阅 `PauseSoundEvent`
- `OnPlaySound`：
  - `IsLoop = false`：保持原有 `PlayOneShot(clip, volume)`
  - `IsLoop = true`：`source.clip = clip; source.volume = volume; source.pitch = pitch; source.loop = true; source.Play()`
- 新增 `OnPauseSound(PauseSoundEvent e)`：`_sources[e.Track].Pause()`

## 3. PlayerMove.cs 修改

新增 Inspector 字段：
```csharp
[Header("脚步音效")]
[SerializeField] private AudioClip footstepClip;
[SerializeField] private float runPitchMultiplier = 1.5f;
```

逻辑（在 `Move()` 方法中）：
- `inputDirection == Vector3.zero`（停止）：发布 `PauseSoundEvent { Track = AudioTrack.PlayerFootstep }`
- 移动中且脚步未在播放：发布 `PlaySoundEvent { Clip = footstepClip, Track = PlayerFootstep, IsLoop = true, Pitch = running ? runPitchMultiplier : 1f }`
- 奔跑状态切换时更新 pitch：重新发布 `PlaySoundEvent` 以更新 pitch

### 状态追踪

新增私有字段 `private bool _footstepPlaying` 和 `private bool _wasRunning`，避免每帧重复发布事件。

## 文件清单

| 文件 | 操作 |
|------|------|
| `Assets/Script/GameEvent/GameEventList.cs` | 修改 `PlaySoundEvent`（加 `IsLoop`、`Pitch`）；新增 `PauseSoundEvent` |
| `Assets/Script/Audio/AudioManager.cs` | 修改 `OnPlaySound` 处理 loop；新增 `OnPauseSound` |
| `Assets/Script/Player/PlayerMove.cs` | 新增脚步音效字段和发布逻辑 |
