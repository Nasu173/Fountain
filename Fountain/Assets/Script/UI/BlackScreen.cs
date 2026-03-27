using Fountain.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.UI
{
    /// <summary>
    /// 全局的黑屏效果
    /// </summary>
    public class BlackScreen :MonoBehaviour
    {
        private FadeEffect fade;

        [Header("黑屏需要的数据")]
        public float fadeInTime;
        public float fadeOutTime;
        public float duration;
        private float elapsed;
        private void Start()
        {
            fade = this.GetComponent<FadeEffect>();
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<FadeEvent>(Black);  
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<FadeEvent>(Black);  
        }
        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed>=duration)
            {
                fade.duration = fadeOutTime;
                fade.FadeOut(); 
            }
        }
        public void Black(FadeEvent e)
        {
            fadeInTime = e.fadeInTime;
            fadeOutTime = e.fadeOutTime;
            duration = e.duration;
            fade.SetFadeImage(e.fadeImage);

            fade.duration = fadeInTime;
            fade.FadeIn();
            elapsed = 0;
        }
    }
}
