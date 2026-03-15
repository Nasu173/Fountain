# 实现任务

## Task 1: 创建 SceneAddress.cs ✓
- 路径: `Assets/Script/Scene/SceneAddress.cs`
- 命名空间: `Foutain.Scene`
- 静态类，定义场景 Addressable 地址字符串常量
- 示例常量: `MainMenu`, `GameScene`

## Task 2: 创建 SceneEventList.cs ✓
- 路径: `Assets/Script/Scene/SceneEventList.cs`
- 命名空间: `Foutain.Scene`
- 定义三个事件类，均实现 `IGameEvent`（来自 `Foutain` 命名空间）：
  - `LoadSceneEvent`：`string SceneAddress`
  - `SceneLoadProgressEvent`：`float Progress`
  - `SceneLoadedEvent`：`string SceneAddress`

## Task 3: 创建 SceneLoader.cs ✓
- 路径: `Assets/Script/Scene/SceneLoader.cs`
- 命名空间: `Foutain.Scene`
- 继承 `MonoBehaviour`
- 公开方法: `IEnumerator LoadSceneAsync(string address, Action<float> onProgress, Action<AsyncOperationHandle<SceneInstance>> onComplete)`
  - 调用 `Addressables.LoadSceneAsync(address, LoadSceneMode.Single)`
  - 循环等待，每帧通过 `onProgress` 回调 `handle.PercentComplete`
  - 完成后通过 `onComplete` 回调句柄
- 公开方法: `void ReleaseScene(AsyncOperationHandle handle)`
  - 调用 `Addressables.Release(handle)`

## Task 4: 创建 GameSceneManager.cs ✓
- 路径: `Assets/Script/Scene/GameSceneManager.cs`
- 命名空间: `Foutain.Scene`
- 继承 `MonoBehaviour`，单例模式（`public static GameSceneManager Instance`）
- `[SerializeField] SceneLoader _loader`
- `Awake`: 设置单例，`DontDestroyOnLoad`，订阅 `LoadSceneEvent`
- `OnDestroy`: 取消订阅
- 事件处理: 启动协程，调用 `_loader.LoadSceneAsync`
  - 进度回调: `GameEventBus.Publish<SceneLoadProgressEvent>`
  - 完成回调: 保存句柄，`GameEventBus.Publish<SceneLoadedEvent>`
- 持有 `AsyncOperationHandle<SceneInstance> _currentHandle`，切换前若有旧句柄则先 `ReleaseScene`
