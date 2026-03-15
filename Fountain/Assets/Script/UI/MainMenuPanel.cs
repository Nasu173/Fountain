using Fountain.Player;
using Foutain.Scene;
using UnityEngine;

namespace Foutain.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] private string _gameSceneAddress;

        public void OnStartClicked()
        {
            GameInputManager.Instance.HideCursor();

            GameInputManager.Instance.EnablePausePanel();
            
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
