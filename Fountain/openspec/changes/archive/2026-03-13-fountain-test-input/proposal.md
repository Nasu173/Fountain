# 喷泉测试输入脚本

## 问题/需求

开发阶段需要在场景中快速测试喷泉效果的开关，无需通过 UI 或其他系统触发，直接用键盘按键控制。

## 目标

提供一个轻量测试脚本，挂载到场景中任意 GameObject，通过 O/P 键控制指定喷泉的开关。

## 方案

- `FountainTestInput`：MonoBehaviour，Inspector 中引用一个 `FountainController`
- 每帧检测 O 键 → 调用 `TurnOn()`，P 键 → 调用 `TurnOff()`
- 脚本放置在 `Assets/Script/Fountain/`
