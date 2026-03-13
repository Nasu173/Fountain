# 设计方案

## 架构

三个 C# 脚本，均放在 `Assets/Script/Fountain/` 目录：

### FountainSteamEffect.cs
- 继承 `MonoBehaviour`
- 持有 `ParticleSystem` 引用（白汽粒子，低高度，白色）
- 提供 `Play()` / `Stop()` 方法

### FountainWaterEffect.cs
- 继承 `MonoBehaviour`
- 持有 `ParticleSystem` 引用（水柱粒子，红色）
- 提供 `Play()` / `Stop()` 方法

### FountainController.cs
- 继承 `MonoBehaviour`
- 持有 `FountainSteamEffect` 和 `FountainWaterEffect` 引用
- `[SerializeField] float steamDuration = 2f` — 白汽存在时间
- `[SerializeField] float waterDelay = 0.25f` — 白汽消失到水柱出现的延迟
- `bool _isOn` 记录状态
- `TurnOn()` / `TurnOff()` 公开方法
- 内部用 Coroutine 控制时序：播白汽 → 等 steamDuration → 停白汽 → 等 waterDelay → 播水柱

## 粒子系统配置（由代码在 Awake 中设置）

白汽粒子：
- `startColor = Color.white`
- `startSpeed` 较低（约 1-2）
- `startLifetime` 约 1s
- `gravityModifier = 0`（向上漂浮）

红色水柱粒子：
- `startColor = Color.red`
- `startSpeed` 较高（约 5-8）
- `startLifetime` 约 1.5s

## 文件路径

- `Assets/Script/Fountain/FountainController.cs`
- `Assets/Script/Fountain/FountainSteamEffect.cs`
- `Assets/Script/Fountain/FountainWaterEffect.cs`
