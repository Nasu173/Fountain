using Fountain.InputManagement;
using Fountain.Player;
using Foutain.Scene;
using UnityEngine;

namespace Foutain.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private string _gameSceneAddress;
        //输入来源
        private PlayerSightInputProvider sightInput;
        private UIInputProvider uiInput;

        private void Start()
        {
            uiInput = GameInputManager.Instance.GetProvider<UIInputProvider>();
            sightInput = GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();
        }

        public void OnStartClicked()
        {
            if (sightInput != null)
                sightInput.HideCursor();
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            //GameInputManager.Instance.HideCursor();
            if (uiInput != null) uiInput.enabled = true;
            //GameInputManager.Instance.EnablePausePanel();
            
            GameEventBus.Publish(new GameStartEvent());

            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = _gameSceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });
        }

        public void OnSettingClicked()
        {
            GameEventBus.Publish(new SettingEvent());
            gameObject.SetActive(false);
        }

        public void OnQuitClicked()
        {
            Application.Quit();
        }
    }
}
