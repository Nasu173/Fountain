# Tasks: Fix PanelManager isStarted Reset

## [x] Task 1 — Set `isStarted = true` in `Start()`

**File**: `Assets/Script/Manager/PanelManager.cs`

In `Start()`, add `isStarted = true;` at the end of the method.

`PanelManager` only exists in the game scene, so `Start()` running means the game is active and the pause panel should be usable.

---

## [x] Task 2 — Remove `isStarted = false` from `OnMenuClicked`

**File**: `Assets/Script/Manager/PanelManager.cs`

Remove line 136: `isStarted = false;`

This line is the direct cause of the bug on the persistent-object path. On the scene-recreation path it's harmless but misleading.

---

## [x] Task 3 — Remove `GameStartEvent` dependency from `PanelManager`

**File**: `Assets/Script/Manager/PanelManager.cs`

- Remove `GameEventBus.Subscribe<GameStartEvent>(OnGameStart);` from `OnEnable`
- Remove `GameEventBus.Unsubscribe<GameStartEvent>(OnGameStart);` from `OnDisable`
- Remove the `OnGameStart(GameStartEvent @event)` method

`GameStartEvent` was the only mechanism setting `isStarted = true`. Since Task 1 replaces it with `Start()`, this event handler is now dead code.

---

## Verification

After applying:
1. Enter game → pause → click Main Menu → re-enter game
2. Confirm pause panel opens on second play session
3. Confirm pause panel opens on third+ play sessions
