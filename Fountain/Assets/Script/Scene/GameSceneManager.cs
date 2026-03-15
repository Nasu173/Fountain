using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Foutain.Scene
{
    /// <summary>
    /// 场景管理器单例，订阅 LoadSceneEvent，协调 SceneLoader 完成场景切换
    /// </summary>
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        [SerializeField] private SceneLoader _loader;

        private AsyncOperationHandle<SceneInstance> _currentHandle;
        private bool _hasCurrentHandle;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameEventBus.Subscribe<LoadSceneEvent>(OnLoadScene);
        }

        private void OnDestroy()
        {
            GameEventBus.Unsubscribe<LoadSceneEvent>(OnLoadScene);
        }

        private void OnLoadScene(LoadSceneEvent e)
        {
            StartCoroutine(LoadRoutine(e));
        }

        private IEnumerator LoadRoutine(LoadSceneEvent e)
        {
            if (_hasCurrentHandle)
            {
                _loader.ReleaseScene(_currentHandle);
                _hasCurrentHandle = false;
            }

            var mode = e.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            yield return _loader.LoadSceneAsync(
                e.SceneAddress,
                p => GameEventBus.Publish(new SceneLoadProgressEvent { Progress = p }),
                handle =>
                {
                    _currentHandle = handle;
                    _hasCurrentHandle = true;
                    GameEventBus.Publish(new SceneLoadedEvent { SceneAddress = e.SceneAddress });
                },
                mode
            );

            if (e.Additive && e.UnloadAll)
            {
                var keep = new System.Collections.Generic.HashSet<string>(e.ScenesToKeep ?? System.Array.Empty<string>());
                for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
                {
                    var s = SceneManager.GetSceneAt(i);
                    if (s.isLoaded && s.name != e.SceneAddress && !keep.Contains(s.name))
                        SceneManager.UnloadSceneAsync(s);
                }
            }
            else if (e.Additive && !string.IsNullOrEmpty(e.SceneToUnload))
            {
                var s = SceneManager.GetSceneByName(e.SceneToUnload);
                if (s.isLoaded)
                    SceneManager.UnloadSceneAsync(s);
            }
        }
    }
}
