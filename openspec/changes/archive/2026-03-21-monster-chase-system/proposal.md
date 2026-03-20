# Monster Chase System — Proposal

## What

实现恐怖游戏中的怪物追逐系统，包含以下完整行为链：

1. **持续追逐**：怪物在追逐阶段激活后持续向玩家移动（NavMeshAgent 驱动）
2. **阶段感知停止**：当前任务阶段结束时怪物停止追逐并退出激活状态
3. **触碰处决**：怪物碰到玩家后触发处决动画（怪物播放攻击动画，玩家视角锁定）
4. **复活 UI**：处决动画播放完毕后显示复活面板（复用 PanelManager 模式）
5. **动画适配**：NavMeshAgent 速度与 Animator 参数同步，保证移动动画匹配

## Why

当前项目已有 TaskManager（任务阶段管理）、PlayerInstance（玩家单例）、GameEventBus（事件总线）、PanelManager（面板管理）等基础设施，可以直接复用。怪物追逐是恐怖游戏核心玩法，需要一套独立、可配置的怪物 AI 系统与现有架构集成。

## Scope

- 新增 `MonsterChase.cs`：怪物 AI 主控脚本（NavMeshAgent + Animator 驱动）
- 新增 `RevivePanel.cs`：复活 UI 面板脚本
- 新增游戏事件：`MonsterCatchEvent`、`ReviveEvent`
- 不修改现有 TaskManager、PlayerMove、PanelManager

## Out of Scope

- 怪物寻路网格烘焙（需在 Unity Editor 手动完成）
- 处决动画资源制作
- 复活后的重生逻辑（仅显示 UI，具体复活行为由调用方决定）
