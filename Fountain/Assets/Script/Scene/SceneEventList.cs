namespace Foutain.Scene
{
    /// <summary>请求切换场景</summary>
    [System.Serializable]
    public class LoadSceneEvent : IGameEvent
    {
        public string SceneAddress;
        /// <summary>true = Additive 加载，加载完成后卸载 SceneToUnload</summary>
        public bool Additive;
        /// <summary>Additive 模式下要卸载的场景名称（Unity 场景名）</summary>
        public string SceneToUnload;
        /// <summary>true = 加载完成后卸载所有不在 ScenesToKeep 中的场景</summary>
        public bool UnloadAll;
        /// <summary>UnloadAll 模式下要保留的场景名称列表</summary>
        public string[] ScenesToKeep;
    }

    /// <summary>场景加载进度（0~1）</summary>
    [System.Serializable]
    public class SceneLoadProgressEvent : IGameEvent
    {
        public float Progress;
    }

    /// <summary>场景加载完成</summary>
    [System.Serializable]
    public class SceneLoadedEvent : IGameEvent
    {
        public string SceneAddress;
    }
}
