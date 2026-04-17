# Tasks: VHS 像素颗粒噪点替换

## Task 1 — 替换 Shader 中的 grain 实现

**文件**: `Assets/VHS/VHSRetroFeature.shader`，`frag` 函数内

找到 grain 块并替换：

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

---

## 验收标准

- [x] 颗粒效果为离散像素点，边界清晰，无模糊过渡
- [ ] `_GrainSize` 增大时颗粒块变粗
- [ ] `_GrainAmount` 增大时颗粒密度增加
- [ ] 颗粒随时间闪烁（不是静止的）
- [ ] 无编译错误
