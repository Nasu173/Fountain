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
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<ControlFountainEndEvent>(Show);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<ControlFountainEndEvent>(Show);
        }
        private void Show(ControlFountainEndEvent e)
        {
            fadeEffect.FadeIn();
        }
    }
}
