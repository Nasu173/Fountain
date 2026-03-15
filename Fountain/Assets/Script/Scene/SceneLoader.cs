using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Foutain.Scene
{
    /// <summary>
    /// 封装 Addressable 场景的异步加载与释放
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        /// <summary>
        /// 异步加载场景，每帧回调进度，完成后通过 onComplete 返回句柄
        /// </summary>
        public IEnumerator LoadSceneAsync(string address, Action<float> onProgress,
            Action<AsyncOperationHandle<SceneInstance>> onComplete, LoadSceneMode mode = LoadSceneMode.Single)
        {
            var handle = Addressables.LoadSceneAsync(address, mode);

            while (!handle.IsDone)
            {
                onProgress?.Invoke(handle.PercentComplete);
                yield return null;
            }

            onProgress?.Invoke(1f);
            onComplete?.Invoke(handle);
        }

        /// <summary>
        /// 释放场景句柄，卸载场景资源
        /// </summary>
        public void ReleaseScene(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }
}
