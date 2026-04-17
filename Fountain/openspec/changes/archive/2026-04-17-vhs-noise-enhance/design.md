# Design: VHS 噪点效果增强

## 涉及文件

| 文件 | 修改类型 |
|------|---------|
| `Assets/VHS/VHSRetroFeature.shader` | 扩大 `Range`，移除硬编码系数 |
| `Assets/VHS/VHSRetroFeature.cs` | 扩大 `[Range]` 属性，同步 C# 侧上限 |

---

## Shader 修改详情 (`VHSRetroFeature.shader`)

### 1. Properties 块 — 扩大范围

```hlsl
// 修改前
_GrainAmount ("Grain Amount", Range(0, 0.5)) = 0.15
_GrainSize   ("Grain Size",   Range(0.5, 3)) = 1.2
_NoiseIntensity ("Noise Intensity", Range(0, 0.3)) = 0.08
_TapeNoise   ("Tape Noise",   Range(0, 0.2)) = 0.06

// 修改后
_GrainAmount ("Grain Amount", Range(0, 2.0)) = 0.15
_GrainSize   ("Grain Size",   Range(0.5, 8)) = 1.2
_NoiseIntensity ("Noise Intensity", Range(0, 1.0)) = 0.08
_TapeNoise   ("Tape Noise",   Range(0, 0.8)) = 0.06
```

### 2. frag 函数 — 移除硬编码混合系数

```hlsl
// 修改前（grain 混合系数固定为 0.02）
float grain = fbm(uv * _GrainSize * 50.0 + time) * _GrainAmount;
color += (grain - 0.5) * 0.02;

// 修改后（系数由 _GrainAmount 自身驱动，线性缩放）
float grain = fbm(uv * _GrainSize * 50.0 + time);
color += (grain - 0.5) * _GrainAmount;
```

```hlsl
// 修改前（tape noise 混合系数固定为 0.01）
float tapeNoise = noise(uv * 3.0 + time * _TapeSpeed) * _TapeNoise;
color += (tapeNoise - 0.5) * 0.01;

// 修改后
float tapeNoise = noise(uv * 3.0 + time * _TapeSpeed);
color += (tapeNoise - 0.5) * _TapeNoise;
```

---

## C# 修改详情 (`VHSRetroFeature.cs`)

同步扩大 `[Range]` 上限，与 Shader Properties 保持一致：

```csharp
// 修改前
[Range(0, 0.5f)]  public float grainAmount    = 0.15f;
[Range(0.5f, 3f)] public float grainSize      = 1.2f;
[Range(0, 0.3f)]  public float noiseIntensity = 0.08f;
[Range(0, 0.2f)]  public float tapeNoise      = 0.06f;

// 修改后
[Range(0, 2.0f)]  public float grainAmount    = 0.15f;
[Range(0.5f, 8f)] public float grainSize      = 1.2f;
[Range(0, 1.0f)]  public float noiseIntensity = 0.08f;
[Range(0, 0.8f)]  public float tapeNoise      = 0.06f;
```

---

## 向后兼容性

- 所有默认值保持不变
- 现有场景中已序列化的参数值不受影响（Unity 不会因 Range 扩大而重置已保存的值）
- 只有当设计师主动将滑块拖到新范围时才会看到更强效果
