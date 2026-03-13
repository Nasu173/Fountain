# 实现任务

## Task 1: 创建 FountainSteamEffect.cs
- 路径: `Assets/Script/Fountain/FountainSteamEffect.cs`
- 命名空间: `Foutain.Fountain`
- 持有 `[SerializeField] ParticleSystem _steamParticle`
- `Play()` 调用 `_steamParticle.Play()`
- `Stop()` 调用 `_steamParticle.Stop()`

## Task 2: 创建 FountainWaterEffect.cs
- 路径: `Assets/Script/Fountain/FountainWaterEffect.cs`
- 命名空间: `Foutain.Fountain`
- 持有 `[SerializeField] ParticleSystem _waterParticle`
- `Play()` 调用 `_waterParticle.Play()`
- `Stop()` 调用 `_waterParticle.Stop()`

## Task 3: 创建 FountainController.cs
- 路径: `Assets/Script/Fountain/FountainController.cs`
- 命名空间: `Foutain.Fountain`
- 引用 `FountainSteamEffect` 和 `FountainWaterEffect`
- `[SerializeField] float _steamDuration = 2f`
- `[SerializeField] float _waterDelay = 0.25f`
- `TurnOn()`: 若已开启则忽略，否则启动 Coroutine
- `TurnOff()`: 停止 Coroutine，停止所有粒子，重置状态
- Coroutine 时序: Play steam → WaitForSeconds(steamDuration) → Stop steam → WaitForSeconds(waterDelay) → Play water
