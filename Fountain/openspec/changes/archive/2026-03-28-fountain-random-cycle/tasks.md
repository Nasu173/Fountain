# Tasks: Fountain Random Cycle

## [x] Task 1 — Create `FountainCycleController.cs`

**File**: `Assets/Script/Fountain/FountainCycleController.cs`

Create the script with:
- Namespace `Fountain.Fountain`
- `[SerializeField] FountainController _controller`
- Four float fields: `_idleMin` (3), `_idleMax` (8), `_eruptMin` (5), `_eruptMax` (12)
- `OnEnable` starts the cycle coroutine
- `OnDisable` stops it and calls `_controller.TurnOff()`
- Coroutine: `while(true)` → wait idle → TurnOn → wait erupt → TurnOff

---

## Verification

1. Add `FountainCycleController` to the Fountain prefab/GameObject alongside `FountainController`
2. Assign `_controller` reference in Inspector
3. Enter Play mode — fountain should erupt and stop automatically on random intervals
4. Adjust min/max values in Inspector and confirm behavior changes
