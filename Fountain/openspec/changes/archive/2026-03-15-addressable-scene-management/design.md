# 设计方案

## 架构

四个 C# 脚本，放在 `Assets/Script/Scene/` 目录：

### SceneEventList.cs（追加到 GameEventList.cs 或独立文件）
定义三个事件，均实现 `IGameEvent`：
- `LoadSceneEvent`：携带 `string SceneAddress`（Addressable 地址）
- `SceneLoadProgressEvent`：携带 `float Progress`（0~1）
- `SceneLoadedEvent`：携带 `string SceneAddress`（加载完成通知）

### SceneLoader.cs
- 继承 `MonoBehaviour`
- 核心方法：`IEnumerator LoadSceneAsync(string address, Action<float> onProgress, Action onComplete)`
  - 使用 `Addressables.LoadSceneAsync(address, LoadSceneMode.Single)`
  - 每帧通过 `AsyncOperationHandle.PercentComplete` 回调进度
  - 完成后调用 `onComplete`
- 方法：`void UnloadScene(AsyncOperationHandle handle)` — 释放 Addressable 句柄

### GameSceneManager.cs
- 继承 `MonoBehaviour`，实现单例（`Instance` 静态属性）
- 持有 `SceneLoader _loader`（`[SerializeField]`）
- `Awake` 中订阅 `LoadSceneEvent`
- `OnDestroy` 中取消订阅
- 收到事件后调用 `_loader.LoadSceneAsync`，期间发布 `SceneLoadProgressEvent`，完成后发布 `SceneLoadedEvent`
- 持有当前场景句柄 `AsyncOperationHandle _currentHandle`，切换前卸载旧句柄

### SceneAddress.cs
- 静态类，集中定义场景 Addressable 地址常量
- 示例：`public const string MainMenu = "MainMenu";`

## 数据流

```
调用方
  └─ GameEventBus.Publish<LoadSceneEvent>(address)
       └─ GameSceneManager 收到事件
            └─ SceneLoader.LoadSceneAsync()
                 ├─ 每帧 → GameEventBus.Publish<SceneLoadProgressEvent>(progress)
                 └─ 完成 → GameEventBus.Publish<SceneLoadedEvent>(address)
```

## 文件路径

- `Assets/Script/Scene/SceneAddress.cs`
- `Assets/Script/Scene/SceneEventList.cs`
- `Assets/Script/Scene/SceneLoader.cs`
- `Assets/Script/Scene/GameSceneManager.cs`
