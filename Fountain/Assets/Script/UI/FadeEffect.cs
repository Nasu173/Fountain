using System;
using System.Collections;
using System.Collections.Generic;
using Fountain.InputManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.UI
{
    /// <summary>
    /// 渐变效果
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeEffect : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        public Image fadeImage;
        [Tooltip("过渡持续过程")]
        public float duration;
        [Tooltip("是否默认隐藏")]
        [SerializeField]
        private bool hiddenDefault=true;

        private Coroutine fadeCoroutine;
        private void Awake()
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }
        private void Start()
        {
            if (hiddenDefault)
            {
                canvasGroup.alpha = 0;
            }
            else
            {
                canvasGroup.alpha = 1;
            }
        }
        /// <summary>
        /// 淡入
        /// </summary>
        public void FadeIn()
        {
            if (fadeCoroutine!=null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(Fade(0, 1));
        }
        /// <summary>
        /// 淡出
        /// </summary>
        public void FadeOut()
        {
            if (fadeCoroutine!=null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(Fade(1, 0));
        }
        public void SetFadeImage(Image img)
        {
            fadeImage = img;
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
            canvasGroup.alpha = end;
            fadeCoroutine = null;
        }
    }
}
