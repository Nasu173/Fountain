using Fountain.Common;
using UnityEngine;

namespace Fountain.InputManagement
{
    /// <summary>
    /// 统一管理鼠标显示/隐藏状态
    /// </summary>
    public class CursorManager : MonoSingleton<CursorManager>
    {
        private PlayerSightInputProvider sightInput;

        private bool mainMenuEnabled;
        private bool settingPanelEnabled;
        private bool pausePanelEnabled;
        private bool respawnPanelEnabled;
        protected override void Init()
        {
            base.Init();
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            sightInput = GameInputManager.Instance?.GetProvider<PlayerSightInputProvider>();
            UpdateCursorVisibility();
        }

        public void SetMainMenuEnabled(bool enabled)
        {
            mainMenuEnabled = enabled;
            UpdateCursorVisibility();
        }

        public void SetSettingPanelEnabled(bool enabled)
        {
            settingPanelEnabled = enabled;
            UpdateCursorVisibility();
        }

        public void SetPausePanelEnabled(bool enabled)
        {
            pausePanelEnabled = enabled;
            UpdateCursorVisibility();
        }
        public void SetRespawnPanelEnabled(bool enabled)
        {
            respawnPanelEnabled= enabled;
            UpdateCursorVisibility();
        }
        private void UpdateCursorVisibility()
        {
            if (Instance != this) return;

            if (sightInput == null)
            {
                sightInput = GameInputManager.Instance?.GetProvider<PlayerSightInputProvider>();
            }

            bool shouldShowCursor = mainMenuEnabled || settingPanelEnabled || pausePanelEnabled || respawnPanelEnabled;
            if (shouldShowCursor)
            {
                if (sightInput != null) sightInput.ShowCursor();
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
            else
            {
                if (sightInput != null) sightInput.HideCursor();
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
