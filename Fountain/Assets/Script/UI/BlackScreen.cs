using Fountain.Common;
using Fountain.InputManagement;
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
        //是否正在做黑屏
        private bool isBlacking;

        private void Start()
        {
            fade = this.GetComponent<FadeEffect>();
            isBlacking = false;
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
            if (!isBlacking)
            {
                return;
            }
            elapsed += Time.deltaTime;
            if (elapsed>=duration)
            {
                fade.duration = fadeOutTime;

                GameInputManager.Instance.
                    GetProvider<CharacterInputProvider>().enabled = true;
                fade.FadeOut();
                isBlacking = false;
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
            isBlacking = true;

            //禁用输入
            GameInputManager.Instance.
                GetProvider<CharacterInputProvider>().enabled = false;
        }
    }
}
