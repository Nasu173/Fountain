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
        //private FadeEffect fadeEffect;
        private CanvasGroup canvasGroup;
        [Tooltip("游戏过程倒计时器")]
        [SerializeField]
        private CountdownTimer gameTimer;
        [Tooltip("游戏黑屏的时间(在倒计时内)")]
        public float[] blackTimes;
        [Tooltip("黑屏持续时间")]
        public float blackDuration;
        public AudioClip blackSFX;
        public AudioTrack track;
        private void Start()
        {
            //fadeEffect = this.GetComponent<FadeEffect>();
            canvasGroup = this.GetComponent<CanvasGroup>();
            for (int i = 0; i < blackTimes.Length; i++)
            {
                gameTimer.AddEvent(new CountdownTimer.CountDownEvent()
                {
                    invokeTime = blackTimes[i],
                    action=Show
                    //action = () => { fadeEffect.FadeIn(); }
                });
                gameTimer.AddEvent(new CountdownTimer.CountDownEvent()
                {
                    //invokeTime = blackTimes[i]-fadeEffect.duration-blackDuration,
                    invokeTime = blackTimes[i]-blackDuration,
                    action=Hide
                   // action = () => { fadeEffect.FadeOut(); }
                });
            }
        }
        private void Hide()
        {
            this.canvasGroup.alpha = 0;
        }
        private void Show()
        {
            this.canvasGroup.alpha = 1;
            GameEventBus.Publish<PlaySoundEvent>(new PlaySoundEvent()
            { Clip = blackSFX, Track = this.track, IsLoop = false });
        }
    }
}
