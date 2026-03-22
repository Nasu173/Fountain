using Fountain.InputManagement;
using Fountain.UI;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 胜利结束的UI
    /// </summary>
    public class VictoryUI : MonoBehaviour
    {
        public string _gameSceneAddress;
        private FadeEffect fadeEffect;
        public Button returnButton;
        private void Start()
        {
            fadeEffect = this.GetComponent<FadeEffect>();
            GameEventBus.Subscribe<ControlFountainEndEvent>((e) =>
            {
                fadeEffect.FadeIn();
            });
            /*
            returnButton.onClick.AddListener(() =>
            {

                GameInputManager.Instance.GetProvider<PlayerSightInputProvider>().HideCursor();
            });
             
             */
        }

    }
}
