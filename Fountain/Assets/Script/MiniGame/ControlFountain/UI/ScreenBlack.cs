using Fountain.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 屏幕黑屏效果
    /// </summary>
    public class ScreenBlack : MonoBehaviour
    {
        private FadeEffect fadeEffect;
        [Tooltip("游戏过程倒计时器")]
        [SerializeField]
        private CountdownTimer gameTimer;
        [Tooltip("游戏黑屏的时间(在倒计时内)")]
        public float[] blackTimes;
        [Tooltip("黑屏持续时间")]
        public float blackDuration;
        private void Start()
        {
            fadeEffect = this.GetComponent<FadeEffect>();
            for (int i = 0; i < blackTimes.Length; i++)
            {
                gameTimer.AddEvent(new CountdownTimer.CountDownEvent()
                {
                    invokeTime = blackTimes[i],
                    action = () => { fadeEffect.FadeIn(); }
                });
                gameTimer.AddEvent(new CountdownTimer.CountDownEvent()
                {
                    invokeTime = blackTimes[i]-fadeEffect.duration-blackDuration,
                    action = () => { fadeEffect.FadeOut(); }
                });
            }
        }
    }
}
