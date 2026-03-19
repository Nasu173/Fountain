using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fountain.MiniGame.ControlFountain
{
    /// <summary>
    /// 游戏流程控制器,负责提供开始和结束的方法,小游戏就不用做成单例了
    /// </summary>
    public class GameFlowController : MonoBehaviour
    {
        private void Update()
        {
            /*
             
            Keyboard keyboard = Keyboard.current;
            if (keyboard.pKey.wasPressedThisFrame)
            {
                GameEventBus.Publish<ControlFountainReadyEvent>(null);
            }
            if (keyboard.oKey.wasPressedThisFrame)
            {
                GameEventBus.Publish<ControlFountainStartEvent>(null);
            }
             */
        }
        /// <summary>
        /// 准备游戏
        /// </summary>
        public void PrepareGame()
        {
            GameEventBus.Publish<ControlFountainReadyEvent>(null);
        }
        public void StartGame()
        {
            GameEventBus.Publish<ControlFountainStartEvent>(null);
        }
        /// <summary>
        /// 游戏结束
        /// </summary>
        public void EndGame()
        {
            GameEventBus.Publish<ControlFountainEndEvent>(null);
        }
    }
}
