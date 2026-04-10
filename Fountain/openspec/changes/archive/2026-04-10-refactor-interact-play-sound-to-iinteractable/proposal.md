# Proposal: Refactor InteractPlaySound to IInteractable

## What

将 `InteractPlaySound` 脚本从基于 `Input.GetKeyDown` 的轮询方式，改为实现 `IInteractable` 接口，融入项目已有的交互系统。

## Why

当前 `InteractPlaySound` 使用 `Update()` + `Input.GetKeyDown(key)` 直接监听按键，与项目中其他可交互对象（`DialogueInteractable`、`TaskInteractable` 等）的交互方式不一致：

- 绕过了 `PlayerInteractor` 的射线检测和距离限制，玩家不需要靠近物体就能触发
- 无法与 `ControlInteractable` 配合，不支持按任务状态启用/禁用交互
- 无法显示交互提示（`InteractPrompt`），因为 `Select()`/`Deselect()` 未被调用
- 硬编码 `KeyCode`，与项目使用 Unity Input System 的方向不符

## Scope

- 修改 `Assets/Script/Audio/InteractPlaySound.cs`：实现 `IInteractable` 接口，删除 `Update()` 轮询
- 不新增文件，不修改其他脚本
