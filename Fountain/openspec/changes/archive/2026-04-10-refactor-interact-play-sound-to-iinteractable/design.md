# Design: Refactor InteractPlaySound to IInteractable

## Architecture

```
PlayerInteractor (射线检测)
    │
    ├── Select()      → InteractPlaySound.Select()    → OutlineVisual.SetOutline(true)  [可选]
    ├── Deselect()    → InteractPlaySound.Deselect()  → OutlineVisual.SetOutline(false) [可选]
    └── InteractWith()→ InteractPlaySound.InteractWith() → GameEventBus.Publish(PlaySoundEvent)
```

## 接口实现

`InteractPlaySound` 实现 `IInteractable`：

| 成员 | 实现 |
|------|------|
| `CanInteract` | `bool` 属性，默认 `true` |
| `InteractWith(PlayerInteractor)` | 发布 `PlaySoundEvent { Clip, Track }` |
| `Select()` | 若有 `OutlineVisual` 则显示描边（可选，字段为 null 时跳过） |
| `Deselect()` | 若有 `OutlineVisual` 则隐藏描边（可选） |

## 字段变更

| 字段 | 变更 |
|------|------|
| `AudioClip clip` | 保留 |
| `AudioTrack track` | 保留 |
| `KeyCode key` | **删除** |
| `OutlineVisual outlineVisual` | **新增**（可选，允许为 null） |
| `bool canInteract` | **新增**（默认 `true`） |

## 命名空间

加入 `Fountain.Player` 命名空间，与其他 `IInteractable` 实现保持一致。

## 注意事项

- `OutlineVisual` 字段标注 `[Tooltip]`，允许不配置（null 安全）
- 删除 `Update()` 方法，不再依赖 `Input` 类
- 物体需挂载 Collider 并设置正确 Layer，才能被 `PlayerInteractor` 射线检测到（这是场景配置问题，不在代码范围内）
