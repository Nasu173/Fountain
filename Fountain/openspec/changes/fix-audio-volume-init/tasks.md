# Tasks: Fix Audio Volume Init

## ~~Task 1: 调整 Start() 中 AddListener 与 LoadAllVolumes 的调用顺序~~ ✓

**文件**: `Assets/Script/Settings/AudioMixerController.cs`

将 `Start()` 方法中的代码顺序从：

```
InitializeSliders()
LoadAllVolumes()
AddListener(...)
```

改为：

```
InitializeSliders()
AddListener(...)
LoadAllVolumes()
```

具体：把三个 `AddListener` 代码块移到 `LoadAllVolumes()` 调用之前。
