# 设计方案

## 架构

单个脚本 `FountainTestInput.cs`，放在 `Assets/Script/Fountain/`。

### FountainTestInput.cs
- 继承 `MonoBehaviour`
- `[SerializeField] FountainController _fountain` — Inspector 中指定目标喷泉
- `Update()` 中检测按键：
  - `KeyCode.O` → `_fountain.TurnOn()`
  - `KeyCode.P` → `_fountain.TurnOff()`

## 文件路径

- `Assets/Script/Fountain/FountainTestInput.cs`
