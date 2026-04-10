# Proposal: Fix Audio Volume Init

## What

修复进入游戏后实际音量与 UI Slider 显示不一致的 bug：每次启动游戏，AudioMixer 的实际音量都是 100%（0dB 默认值），而 Slider 显示的是上次保存的值。

## Why

**根因**：`AudioMixerController.Start()` 中，`LoadAllVolumes()` 在 `AddListener` **之前**调用。

```
Start()
  ├─ InitializeSliders()
  ├─ LoadAllVolumes()          ← slider.value = savedValue 触发 onValueChanged
  │                              但此时监听器还没注册，SetMasterVolume 不会被调用
  │                              AudioMixer 实际音量 = 默认 0dB (100%)
  └─ AddListener(SetMasterVolume)  ← 太晚了
```

结果：Slider 显示正确（PlayerPrefs 值），但 AudioMixer 实际音量未被应用。

## Scope

仅修改 `Assets/Script/Settings/AudioMixerController.cs`：
- 将 `AddListener` 调用移到 `LoadAllVolumes()` 之前，确保加载时能正确触发回调应用到 AudioMixer。
