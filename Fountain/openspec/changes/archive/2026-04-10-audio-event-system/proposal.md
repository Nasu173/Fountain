# Proposal: Audio Event System

## What

实现基于事件总线的音效播放系统，并提供三种触发脚本：
1. **交互式**（InteractPlaySound）：玩家按键/点击时触发
2. **进入式**（EnterPlaySound）：进入触发区域时触发
3. **事件监听式**（EventPlaySound）：监听指定 `IGameEvent` 时触发

## Why

当前项目中：
- `PlaySoundPerform` 仅有 `Debug.Log` 占位，无实际音频播放能力
- 没有 `AudioManager` 或任何 `AudioSource` 使用
- 已有 `GameEventBus` 事件总线和 `AudioMixerController`（管理 BGM/SFX 两条音轨）

需要一套统一的音效触发机制，让各类场景脚本能以解耦方式播放音效，同时复用已有的 AudioMixer 分组（BGM / SFX）。

## Scope

- 新增 `PlaySoundEvent`（`IGameEvent`）到 `GameEventList.cs`
- 新增 `AudioManager`：单例，监听 `PlaySoundEvent`，在对应音轨上播放 `AudioClip`
- 新增三种触发脚本：`InteractPlaySound`、`EnterPlaySound`、`EventPlaySound`
- 修复 `PlaySoundPerform.Perform()` 改为发布 `PlaySoundEvent`
