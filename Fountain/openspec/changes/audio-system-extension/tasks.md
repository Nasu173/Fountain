# Tasks: Audio System Extension

## ~~Task 1: 修改 PlaySoundEvent，新增 PauseSoundEvent~~ ✓

**文件**: `Assets/Script/GameEvent/GameEventList.cs`

- 在 `PlaySoundEvent` 中新增 `public bool IsLoop = false;` 和 `public float Pitch = 1f;`
- 在文件末尾新增：
  ```csharp
  [System.Serializable]
  public class PauseSoundEvent : IGameEvent
  {
      public AudioTrack Track;
  }
  ```

---

## ~~Task 2: 修改 AudioManager，支持循环播放和暂停~~ ✓

**文件**: `Assets/Script/Audio/AudioManager.cs`

- `Awake` 中追加 `GameEventBus.Subscribe<PauseSoundEvent>(OnPauseSound);`
- `OnDestroy` 中追加 `GameEventBus.Unsubscribe<PauseSoundEvent>(OnPauseSound);`
- 修改 `OnPlaySound`：
  ```csharp
  private void OnPlaySound(PlaySoundEvent e)
  {
      if (e.Clip == null) return;
      var src = _sources[e.Track];
      if (e.IsLoop)
      {
          src.clip = e.Clip;
          src.volume = e.Volume;
          src.pitch = e.Pitch;
          src.loop = true;
          src.Play();
      }
      else
      {
          src.pitch = e.Pitch;
          src.PlayOneShot(e.Clip, e.Volume);
      }
  }
  ```
- 新增方法：
  ```csharp
  private void OnPauseSound(PauseSoundEvent e) => _sources[e.Track].Pause();
  ```

---

## ~~Task 3: 在 PlayerMove 中实现脚步音效~~ ✓

**文件**: `Assets/Script/Player/PlayerMove.cs`

- 新增 Inspector 字段（在 `[Header("移动设置")]` 下方新增 `[Header("脚步音效")]` 区块）：
  ```csharp
  [Header("脚步音效")]
  [SerializeField] private AudioClip footstepClip;
  [SerializeField] private float runPitchMultiplier = 1.5f;
  ```
- 新增私有字段：
  ```csharp
  private bool _footstepPlaying;
  private bool _wasRunning;
  ```
- 在 `Move()` 方法中，`moving = false; return;` 之前插入停止脚步逻辑：
  ```csharp
  if (_footstepPlaying)
  {
      GameEventBus.Publish(new PauseSoundEvent { Track = AudioTrack.PlayerFootstep });
      _footstepPlaying = false;
  }
  ```
- 在 `Move()` 方法中，`moving = true;` 之后插入播放/更新脚步逻辑：
  ```csharp
  bool nowRunning = running && !crouching;
  if (!_footstepPlaying || nowRunning != _wasRunning)
  {
      float pitch = nowRunning ? runPitchMultiplier : 1f;
      GameEventBus.Publish(new PlaySoundEvent
      {
          Clip = footstepClip,
          Track = AudioTrack.PlayerFootstep,
          IsLoop = true,
          Pitch = pitch
      });
      _footstepPlaying = true;
      _wasRunning = nowRunning;
  }
  ```
