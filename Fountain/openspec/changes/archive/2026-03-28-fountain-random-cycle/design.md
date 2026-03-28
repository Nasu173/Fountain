# Design: FountainCycleController

## Existing API

`FountainController` (namespace `Fountain.Fountain`) exposes:
- `TurnOn()` — starts steam → water sequence
- `TurnOff()` — stops both effects immediately

## New Component: `FountainCycleController`

Namespace: `Fountain.Fountain`
File: `Assets/Script/Fountain/FountainCycleController.cs`

### Inspector fields

| Field | Type | Default | Purpose |
|---|---|---|---|
| `_idleMin` | float | 3 | Min seconds to wait before erupting |
| `_idleMax` | float | 8 | Max seconds to wait before erupting |
| `_eruptMin` | float | 5 | Min seconds to stay erupting |
| `_eruptMax` | float | 12 | Max seconds to stay erupting |

### Logic

Single coroutine started in `OnEnable`, stopped in `OnDisable`:

```
loop:
  wait Random.Range(_idleMin, _idleMax)
  _controller.TurnOn()
  wait Random.Range(_eruptMin, _eruptMax)
  _controller.TurnOff()
```

`_controller` is a `[SerializeField] FountainController` reference set in Inspector (same GameObject or child).

No state machine needed — a simple `while(true)` coroutine is sufficient.
