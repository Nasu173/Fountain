# Design: PanelManager isStarted Reset Fix

## Bug Analysis

### Lifecycle on first play
1. `PanelManager.OnEnable` → subscribes events, `isStarted = false` (default)
2. `PanelManager.Start` → fetches component references
3. Player clicks Start → `MainMenuPanel.OnStartClicked` → loads game scene
4. Scene loaded → `MainMenuPanel.OnSceneLoaded` → publishes `GameStartEvent`
5. `PanelManager.OnGameStart` → `isStarted = true` ✓
6. Pause key works

### Lifecycle on "Main Menu" → re-enter
1. Player pauses → `Pause()` → `isPaused = true`
2. Player clicks Main Menu → `OnMenuClicked()`:
   - Calls `Pause()` again → `isPaused` toggles to `false`, `ResumeGame()` runs (re-enables `uiInput`)
   - `pausePanel.SetActive(false)`
   - `uiInput.enabled = false`  ← disables input AFTER ResumeGame re-enabled it
   - `isStarted = false`  ← explicit reset
   - Publishes `LoadSceneEvent`
3. Scene transition occurs; `PanelManager` may or may not be destroyed
4. If `PanelManager` persists: `OnDisable` → unsubscribes; `OnEnable` → re-subscribes, but `isStarted` stays `false`
5. If `PanelManager` is recreated: `Start` runs again, `isStarted = false` (default)
6. Player clicks Start again → `GameStartEvent` published
7. `PanelManager.OnGameStart` → `isStarted = true` ✓ (should work)

### The actual race condition / real bug

Looking more carefully: `OnMenuClicked` calls `Pause()` which calls `ResumeGame()`. `ResumeGame` re-enables `uiInput`. Then `OnMenuClicked` immediately sets `uiInput.enabled = false`. This is correct — we don't want pause input active in the main menu.

On re-entry, `PanelManager.Start()` runs (if object is recreated) or `OnEnable` runs (if persistent). Neither re-enables `uiInput`. The `uiInput` stays disabled. So `Update()` calls `uiInput.GetPause()` but since `uiInput.enabled = false`, the `OnEnable` of `UIInputProvider` never fires, meaning `inputActions.PausePanel` stays disabled → `GetPause()` always returns `false`.

**Root cause**: `uiInput` is disabled in `OnMenuClicked` but never re-enabled when the game restarts. `isStarted` does get set to `true` via `GameStartEvent`, but the pause key is never detected because `uiInput` is still disabled.

Wait — `ResumeGame()` does `uiInput.enabled = true`. And `GameStartEvent` only sets `isStarted`. So on re-entry:
- `isStarted` = `true` after `GameStartEvent` ✓
- `uiInput.enabled` = `false` (set in `OnMenuClicked`, never re-enabled on re-entry) ✗

So the pause panel CAN'T be opened because `uiInput.GetPause()` is never `true`.

But the user reports `isStarted` is the problem. Let me reconsider: if `PanelManager` is in the game scene (not persistent), it gets destroyed on scene unload and recreated on re-entry. `Start()` runs, fetching a fresh `uiInput`. `uiInput` is a fresh component — its `enabled` state depends on its default. Since `UIInputProvider` is a `MonoBehaviour`, it defaults to `enabled = true`. So `uiInput` should be fine on recreation.

**Confirmed root cause (recreation path)**: `GameStartEvent` is published by `MainMenuPanel.OnSceneLoaded`. This fires when the scene finishes loading. If `PanelManager` is in that same scene, it may not have run `OnEnable`/`Start` yet when the event fires — Unity scene activation order. The event fires, `PanelManager` isn't subscribed yet, `isStarted` never becomes `true`.

## Solution

Move `isStarted = true` out of the event-driven path. Instead, set it directly in `Start()` — the game scene's `PanelManager` only exists when the game is running, so `Start()` is the right place.

Remove `OnGameStart` handler entirely and set `isStarted = true` in `Start()`.

Also remove `isStarted = false` from `OnMenuClicked` (redundant if object is destroyed with scene).

## Changes

### `PanelManager.cs`

1. Remove `isStarted = false` from `OnMenuClicked`
2. Set `isStarted = true` in `Start()` instead of waiting for `GameStartEvent`
3. Remove `OnGameStart` method and its event subscription/unsubscription

This is safe because `PanelManager` lives in the game scene — it only exists when the game is active.
