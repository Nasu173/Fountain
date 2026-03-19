using Fountain.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 计时UI,显示时间
    /// </summary>
    public class TimerUI : MonoBehaviour
    {
        [Tooltip("倒计时物体")]
        [SerializeField]
        private CountdownTimer timer;
        private TextMeshProUGUI timeText;
        [Tooltip("倒计时结束时是否隐藏")]
        [SerializeField]
        private bool autoHide;
        private CanvasGroup canvasGroup;
        private void Start()
        {
            timeText = this.transform.FindChildByName(nameof(timeText)).
                GetComponent<TextMeshProUGUI>();
            canvasGroup = this.GetComponent<CanvasGroup>();
            timeText.text = string.Empty; 
            timer.CountdownStart += this.Show;
            timer.CountdownEnd += this.StopShow;
            this.enabled = false;
        }
        private void Update()
        {
            //Math.Ceiling(1.1);//2  
            timeText.text = Math.Ceiling
                (timer.GetRemainingTime()).ToString();                
        }
        /// <summary>
        /// 开始显示时间显示时间
        /// </summary>
        public void Show()
        {
            this.enabled = true;
        }
        /// <summary>
        /// 停止显示
        /// </summary>
        public void StopShow()
        {
            canvasGroup.alpha = autoHide ? 0 : 1;
            this.enabled = false;
            timeText.text = "0";
        }
    }
}
