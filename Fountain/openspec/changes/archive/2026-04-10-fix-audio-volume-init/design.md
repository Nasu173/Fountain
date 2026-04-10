# Design: Fix Audio Volume Init

## 根因分析

`Start()` 执行顺序：

```
1. InitializeSliders()       — 设置 slider min/max
2. LoadAllVolumes()          — slider.value = savedValue → 触发 onValueChanged
                               但监听器未注册，SetXxxVolume() 不被调用
                               AudioMixer 保持默认 0dB
3. AddListener(SetMasterVolume / SetMusicVolume / SetSFXVolume)
```

## 修复方案

将 `AddListener` 调用移到 `LoadAllVolumes()` 之前：

```csharp
void Start()
{
    InitializeSliders();

    // 先注册监听器
    if (masterSlider != null) masterSlider.onValueChanged.AddListener(SetMasterVolume);
    if (musicSlider  != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
    if (sfxSlider    != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);

    // 再加载（此时 slider.value = x 会正确触发回调，应用到 AudioMixer）
    LoadAllVolumes();
}
```

这样 `LoadAllVolumes()` 中 `slider.value = savedValue` 触发 `onValueChanged` 时，监听器已注册，`SetMasterVolume` 等方法会被调用，AudioMixer 实际音量得到正确设置。

## 文件清单

| 文件 | 操作 |
|------|------|
| `Assets/Script/Settings/AudioMixerController.cs` | 调整 `Start()` 中 `AddListener` 与 `LoadAllVolumes()` 的调用顺序 |
