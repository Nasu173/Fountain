using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 倒计时器,暂时引用就拖上去吧
    /// </summary>
    public class CountdownTimer : MonoBehaviour
    {
        /// <summary>
        /// 倒计时的事件
        /// </summary>
        public class CountDownEvent
        {
            /// <summary>
            /// 在第几秒触发
            /// </summary>
            public float invokeTime;
            /// <summary>
            /// 要触发的事件
            /// </summary>
            public Action action;
        }

        [Tooltip("倒计时总时间")]
        [SerializeField]
        private float timeTotal;
        /// <summary>
        /// 剩余时间
        /// </summary>
        private float timeRemain;
        /// <summary>
        /// 倒计时开始事件
        /// </summary>
        public event Action CountdownStart;
        /// <summary>
        /// 倒计时结束事件
        /// </summary>
        public event Action CountdownEnd;

        /// <summary>
        /// 计时过程中要触发的事件
        /// </summary>
        private List<CountDownEvent> events;
        private void Awake()
        {
            events = new List<CountDownEvent>();
        }
        private void Start()
        {
            this.enabled = false;
        }
        private void Update()
        {
            timeRemain -= Time.deltaTime;
            if (timeRemain<=0)
            {
                CountdownEnd?.Invoke();
                this.enabled = false;
                events.Clear();
            }
            TriggerEvent();
        }


        /// <summary>
        /// 开始倒计时,用默认设置的总时间
        /// </summary>
        public void StartCountDown()
        {
            this.enabled = true;
            timeRemain = timeTotal;
            CountdownStart?.Invoke(); 
        }
        /// <summary>
        /// 开始倒计时
        /// </summary>
        /// <param name="time">总时间</param>
        public void StartCountDown(float time)
        {
            this.timeTotal = time;
            StartCountDown();
        }
        /// <summary>
        /// 获得倒计时剩余时间
        /// </summary>
        /// <returns></returns>
        public float GetRemainingTime()
        {
            return timeRemain;
        }
        public float GetTotalTime()
        {
            return timeTotal;
        }

        /// <summary>
        /// 添加倒计时事件
        /// </summary>
        /// <param name="e"></param>
        public void AddEvent(CountDownEvent e)
        {
            if (e==null)
            {
                return;
            }
            this.events.Add(e);
        }    

        /// <summary>
        /// 触发倒计时事件
        /// </summary>
        private void TriggerEvent()
        {
            //倒序遍历防止删除错误
            for (int i = this.events.Count-1; i>=0;i--)
            {
                if (this.timeRemain <= events[i].invokeTime)
                {
                    events[i].action?.Invoke();
                    events.RemoveAt(i);
                } 
            } 
        }
    }
}
