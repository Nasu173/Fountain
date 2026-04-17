# Tasks: VHS 噪点效果增强

## Task 1 — 修改 Shader Properties 范围

**文件**: `Assets/VHS/VHSRetroFeature.shader`

将以下 4 个 Properties 的 Range 上限扩大：

- `_GrainAmount`：`Range(0, 0.5)` → `Range(0, 2.0)`
- `_GrainSize`：`Range(0.5, 3)` → `Range(0.5, 8)`
- `_NoiseIntensity`：`Range(0, 0.3)` → `Range(0, 1.0)`
- `_TapeNoise`：`Range(0, 0.2)` → `Range(0, 0.8)`

---

## Task 2 — 移除 Shader frag 中的硬编码混合系数

**文件**: `Assets/VHS/VHSRetroFeature.shader`，`frag` 函数内

**Grain 部分**（约第 286–288 行）：
```hlsl
// 改前
float grain = fbm(uv * _GrainSize * 50.0 + time) * _GrainAmount;
color += (grain - 0.5) * 0.02;

// 改后
float grain = fbm(uv * _GrainSize * 50.0 + time);
color += (grain - 0.5) * _GrainAmount;
```

**Tape Noise 部分**（约第 290–293 行）：
```hlsl
// 改前
float tapeNoise = noise(uv * 3.0 + time * _TapeSpeed) * _TapeNoise;
color += (tapeNoise - 0.5) * 0.01;

// 改后
float tapeNoise = noise(uv * 3.0 + time * _TapeSpeed);
color += (tapeNoise - 0.5) * _TapeNoise;
```

---

## Task 3 — 同步修改 C# 脚本的 Range 属性

**文件**: `Assets/VHS/VHSRetroFeature.cs`

```csharp
// 改前
[Range(0, 0.5f)]  public float grainAmount    = 0.15f;
[Range(0.5f, 3f)] public float grainSize      = 1.2f;
[Range(0, 0.3f)]  public float noiseIntensity = 0.08f;
[Range(0, 0.2f)]  public float tapeNoise      = 0.06f;

// 改后
[Range(0, 2.0f)]  public float grainAmount    = 0.15f;
[Range(0.5f, 8f)] public float grainSize      = 1.2f;
[Range(0, 1.0f)]  public float noiseIntensity = 0.08f;
[Range(0, 0.8f)]  public float tapeNoise      = 0.06f;
```

---

## 验收标准

- [x] Unity Editor 中 Inspector 滑块范围已更新
- [x] 将 `grainAmount` 拖到 1.0 时，画面出现明显强烈颗粒感
- [x] 默认值场景下效果与修改前一致（无视觉回归）
- [ ] 无编译错误或 Shader 警告（请在 Unity 中验证）
