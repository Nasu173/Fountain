# Fix: PanelManager.isStarted Not Reset on Re-entry

## Problem

When the player enters the game, opens the pause panel, and clicks "Main Menu" to return, then re-enters the game, `PanelManager.isStarted` remains `false`. This prevents the pause panel from being summoned on the second (and subsequent) play sessions without restarting the application.

## Root Cause

In `OnMenuClicked()`, `isStarted` is explicitly set to `false` (line 136). When the player re-enters the game, `MainMenuPanel.OnStartClicked()` loads the game scene and then publishes `GameStartEvent` via `OnSceneLoaded`. `PanelManager.OnGameStart` sets `isStarted = true` in response.

However, `PanelManager.OnEnable()` resets `isPaused` state and hides the pause panel, but **does not reset `isStarted`**. The `GameStartEvent` is published by `MainMenuPanel` after the scene loads — but `PanelManager` may already be active (carried over from the previous session or re-enabled before the event fires), and the subscription in `OnEnable` is set up correctly. The real issue is that `isStarted = false` is set in `OnMenuClicked` and is never set back to `true` on re-entry because the `GameStartEvent` is not reliably received.

The flow on re-entry:
1. `PanelManager.OnEnable()` runs — `isStarted` stays `false`
2. Game scene loads
3. `MainMenuPanel.OnSceneLoaded` fires → publishes `GameStartEvent`
4. `PanelManager` is subscribed and `OnGameStart` sets `isStarted = true` ✓

Wait — the subscription IS in place. Let me re-examine: `OnMenuClicked` calls `Pause()` first (which toggles `isPaused` back to `false` and calls `ResumeGame`), then sets `isStarted = false`. On re-entry, `PanelManager.OnEnable` re-subscribes to `GameStartEvent`. When `MainMenuPanel` publishes `GameStartEvent`, `PanelManager.OnGameStart` should fire.

The actual bug: `OnMenuClicked` disables `uiInput` (`uiInput.enabled = false`). On re-entry, `ResumeGame` re-enables it — but `OnMenuClicked` disables it **after** calling `Pause()` (which calls `ResumeGame` which re-enables it). So `uiInput` ends up disabled. On the next session, `PanelManager.Start()` re-fetches references, but if `PanelManager` is **not destroyed and recreated** between sessions (i.e., it persists across scene loads via DontDestroyOnLoad or is in a persistent scene), `Start()` does NOT run again — only `OnEnable` runs.

If `Start()` doesn't run again, `uiInput` reference may be stale or disabled, and `Update()` won't detect the pause key. But more critically: if `GameStartEvent` fires before `PanelManager.OnEnable` re-subscribes (race condition during scene load), `isStarted` never becomes `true`.

## Fix

Reset `isStarted = false` only when appropriate, and ensure it is set to `true` reliably. The safest fix: **do not set `isStarted = false` in `OnMenuClicked`** — instead, reset it in `OnEnable` (already effectively `false` since it's never set there), and rely solely on `GameStartEvent` to set it `true`. Also ensure `uiInput` is re-enabled on re-entry by not disabling it in `OnMenuClicked` after `ResumeGame` already re-enabled it, or by re-enabling it in `OnEnable`/`Start`.

## Scope

- `Assets/Script/Manager/PanelManager.cs` — remove `isStarted = false` from `OnMenuClicked`; reset state cleanly in `OnEnable`
