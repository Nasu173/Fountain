# Design: VHS 像素颗粒噪点替换

## 涉及文件

| 文件 | 修改类型 |
|------|---------|
| `Assets/VHS/VHSRetroFeature.shader` | 替换 grain 实现逻辑 |

---

## 核心思路

用**逐像素哈希**替代 `fbm`：

```hlsl
// 旧方案（连续噪波）
float grain = fbm(uv * _GrainSize * 50.0 + time);
color += (grain - 0.5) * _GrainAmount;

// 新方案（离散像素颗粒）
float2 pixelCoord = floor(uv * _ScreenParams.xy / _GrainSize);
float grain = hash(pixelCoord + floor(time * 30.0));  // 30fps 闪烁
float grainMask = step(1.0 - _GrainAmount * 0.5, grain);  // 超阈值才显示
color += (grain * 2.0 - 1.0) * grainMask * _GrainAmount;
```

**关键点说明：**

1. `floor(uv * _ScreenParams.xy / _GrainSize)` — 将屏幕划分为 `_GrainSize` 大小的像素块，每块内所有像素共享同一随机值，形成"颗粒块"效果
2. `floor(time * 30.0)` — 以 30fps 步进时间，使颗粒每帧跳变而非连续变化，产生闪烁感
3. `step(threshold, grain)` — 只有随机值超过阈值的像素才显示颗粒，`_GrainAmount` 越大阈值越低，颗粒越密
4. `(grain * 2.0 - 1.0)` — 将 [0,1] 映射到 [-1,1]，颗粒有亮有暗

---

## Shader 修改详情

### frag 函数中的 grain 块（约第 284–288 行）

```hlsl
// 改前
if (_GrainAmount > 0.001)
{
    float grain = fbm(uv * _GrainSize * 50.0 + time);
    color += (grain - 0.5) * _GrainAmount;
}

// 改后
if (_GrainAmount > 0.001)
{
    float2 pixelCoord = floor(uv * _ScreenParams.xy / max(_GrainSize, 0.5));
    float grain = hash(pixelCoord + floor(time * 30.0));
    float grainMask = step(1.0 - saturate(_GrainAmount * 0.5), grain);
    color += (grain * 2.0 - 1.0) * grainMask * _GrainAmount * 0.8;
}
```

### `_ScreenParams` 可用性

`_ScreenParams` 是 Unity 内置变量（`UnityCG.cginc` 已包含），`xy` 分量为屏幕宽高（像素），无需额外声明。

---

## 参数行为变化对比

| 参数 | 旧行为 | 新行为 |
|------|--------|--------|
| `_GrainAmount` | 控制噪波叠加幅度 | 控制颗粒密度（值越大颗粒越多）+ 亮度幅度 |
| `_GrainSize` | 控制噪波采样频率 | 控制颗粒像素块大小（值越大颗粒越粗） |

默认值（`grainAmount=0.15, grainSize=1.2`）下效果与之前接近，但颗粒边界更清晰。
