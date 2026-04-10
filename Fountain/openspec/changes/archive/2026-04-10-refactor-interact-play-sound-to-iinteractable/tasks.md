# Tasks: Refactor InteractPlaySound to IInteractable

## Task 1 ✅ — 修改 InteractPlaySound 实现 IInteractable

**文件**：`Assets/Script/Audio/InteractPlaySound.cs`

将现有脚本完整替换为：

```csharp
using Fountain.Common;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 交互时播放音效的可交互物体
    /// </summary>
    public class InteractPlaySound : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private AudioTrack track = AudioTrack.Other;

        [Tooltip("描边效果，可不配置")]
        [SerializeField] private OutlineVisual outlineVisual;

        private bool canInteract = true;
        public bool CanInteract { get => canInteract; set => canInteract = value; }

        public void InteractWith(PlayerInteractor player)
        {
            GameEventBus.Publish(new PlaySoundEvent { Clip = clip, Track = track });
        }

        public void Select()
        {
            outlineVisual?.SetOutline(true);
        }

        public void Deselect()
        {
            outlineVisual?.SetOutline(false);
        }
    }
}
```

---

## 验收标准

- [ ] 编译无错误
- [ ] 脚本挂载后 Inspector 显示 `clip`、`track`、`outlineVisual` 字段，无 `key` 字段
- [ ] 玩家靠近并按交互键后触发 `PlaySoundEvent`
- [ ] 玩家远离时不触发（依赖 `PlayerInteractor` 射线检测）
- [ ] `CanInteract = false` 时不触发交互
