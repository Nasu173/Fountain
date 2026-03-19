using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.UI
{
    /// <summary>
    /// 渐变效果
    /// </summary>
    public class FadeEffect : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        [Tooltip("过渡持续过程")]
        public float duration;
        private void Awake()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }
        /// <summary>
        /// 淡入
        /// </summary>
        public void FadeIn()
        {
            StartCoroutine(Fade(0, 1));
        }
        /// <summary>
        /// 淡出
        /// </summary>
        public void FadeOut()
        {
            StartCoroutine(Fade(1, 0));
        }
        /// <summary>
        /// 过渡的协程
        /// </summary>
        private IEnumerator Fade(float start,float end)
        {
            canvasGroup.alpha = start;
            float elapsed = 0;
            while (elapsed<duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
                yield return null;
            }
            
        }
    }
}
