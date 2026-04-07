using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.InputManagement
{
    /// <summary>
    /// UI相关的输入提供器
    /// </summary>
    public class PauseInputProvider : MonoBehaviour,IInputProvider
    {
        private PlayerInputActions inputActions;
        //private bool isPaused;
        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction(); 
        }
        private void OnEnable()
        {
            this.inputActions.UI.Pause.Enable();
        }
        private void OnDisable()
        {
            this.inputActions.UI.Pause.Disable();
        }
        /// <summary>
        /// 玩家是否按了暂停键?
        /// </summary>
        /// <returns></returns>
        public bool GetPause()
        {
            return this.inputActions.UI.Pause.WasPressedThisFrame(); 
        }
       // void Update()
       // {
       //     isPaused = GetPause();
       // }
    }
}
