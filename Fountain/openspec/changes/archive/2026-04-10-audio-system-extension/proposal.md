# Proposal: Audio System Extension

## What

在现有音频事件系统基础上进行三项扩展：

1. **PlaySoundEvent 新增 isLoop 字段** — 区分单次播放的音效（SFX）与循环播放的 BGM，AudioManager 根据此字段选择 `PlayOneShot` 或 `loop` 播放模式。

2. **AudioManager 新增暂停指定音轨方法** — 通过事件总线发布 `PauseSoundEvent`，AudioManager 订阅后暂停对应音轨的 AudioSource。

3. **玩家脚步音效** — 在 `PlayerMove` 脚本上实现脚步音效逻辑：移动时循环播放 PlayerFootstep 音轨，停止移动时停止，奔跑时倍速播放。

## Why

- 当前 `PlaySoundEvent` 只支持 `PlayOneShot`，无法实现 BGM 循环播放，需要区分两种播放模式。
- 缺少暂停音轨的能力，无法在对话、过场等场景中暂停背景音乐。
- 玩家移动已有完整的状态（`moving`、`running`），但缺少对应的脚步音效反馈，影响游戏沉浸感。

## Scope

- 修改 `GameEventList.cs`：`PlaySoundEvent` 新增 `isLoop`；新增 `PauseSoundEvent`
- 修改 `AudioManager.cs`：处理 loop 播放逻辑；订阅并处理 `PauseSoundEvent`
- 修改 `PlayerMove.cs`：集成脚步音效逻辑（发布 PlaySoundEvent / PauseSoundEvent）
