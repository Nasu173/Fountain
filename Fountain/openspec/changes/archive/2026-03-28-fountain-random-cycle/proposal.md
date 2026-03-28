# Fountain Random Cycle Controller

## What

Add a `FountainCycleController` script that mounts on the fountain GameObject and drives it through a repeating idle → erupting → idle loop with randomized durations.

## Why

The existing `FountainController` only exposes `TurnOn()` / `TurnOff()` — it has no autonomous cycling behavior. The fountain currently requires external input (keyboard via `FountainTestInput`) to activate. For gameplay, the fountain should erupt on its own at unpredictable intervals to create ambient life.

## Behavior

1. Wait a random duration in `[idleMin, idleMax]` seconds
2. Call `FountainController.TurnOn()`
3. Wait a random duration in `[eruptMin, eruptMax]` seconds
4. Call `FountainController.TurnOff()`
5. Repeat from step 1

All four bounds are exposed in the Inspector.

## Scope

- New file: `Assets/Script/Fountain/FountainCycleController.cs`
- No changes to existing scripts
