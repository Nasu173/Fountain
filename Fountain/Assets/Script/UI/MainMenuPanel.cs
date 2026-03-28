using Fountain.InputManagement;
using Fountain.Player;
using Foutain.Scene;
using UnityEngine;

namespace Foutain.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private string _gameSceneAddress;
        private bool mainMenuEnabled;
        private bool _waitingForScene = false;
        //输入来源
        private PlayerSightInputProvider sightInput;
        private UIInputProvider uiInput;

        private void Start()
        {
            uiInput = GameInputManager.Instance.GetProvider<UIInputProvider>();
            sightInput = GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();
        }

        private void OnEnable()
        {
            // 确保输入管理器已初始化，便于 CursorManager 正常工作
            _ = GameInputManager.Instance;
            uiInput ??= GameInputManager.Instance.GetProvider<UIInputProvider>();
            if (uiInput != null) uiInput.enabled = false;

            GameEventBus.Subscribe<SceneLoadedEvent>(OnSceneLoaded);
            SetMainMenuState(true);
        }

        private void OnDisable()
        {
            GameEventBus.Unsubscribe<SceneLoadedEvent>(OnSceneLoaded);
            SetMainMenuState(false);
        }

        public void OnStartClicked()
        {
            SetMainMenuState(false);
            if (uiInput != null) uiInput.enabled = true;

            _waitingForScene = true;
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = _gameSceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });
        }

        private void OnSceneLoaded(SceneLoadedEvent e)
        {
            if (!_waitingForScene) return;
            _waitingForScene = false;
            GameEventBus.Publish(new GameStartEvent());
        }

        public void OnSettingClicked()
        {
            SetMainMenuState(false);
            GameEventBus.Publish(new SettingEvent());
            gameObject.SetActive(false);
        }

        public void OnQuitClicked()
        {
            Application.Quit();
        }

        private void SetMainMenuState(bool enabled)
        {
            if (mainMenuEnabled == enabled) return;
            mainMenuEnabled = enabled;
            CursorManager.Instance?.SetMainMenuEnabled(enabled);
        }
    }
}
