using Fountain.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 胜利结束的UI
    /// </summary>
    public class VictoryUI : MonoBehaviour
    {
        private FadeEffect fadeEffect;
        private void Start()
        {
            fadeEffect = this.GetComponent<FadeEffect>();
            GameEventBus.Subscribe<ControlFountainEndEvent>((e) =>
            {
                fadeEffect.FadeIn();
            });
        }

    }
}
