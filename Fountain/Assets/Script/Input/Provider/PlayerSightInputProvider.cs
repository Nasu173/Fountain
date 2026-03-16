using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.InputManagement
{
    /// <summary>
    /// 玩家视线输入提供器,提供与相机相关的输入
    /// </summary>
    public class PlayerSightInputProvider : MonoBehaviour,IInputProvider
    {
        private PlayerInputActions inputActions;
        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
            this.HideCursor();
        }
        private void OnEnable()
        {
            this.inputActions.Player.Enable();
        }
        private void OnDisable()
        {
            this.inputActions.Player.Disable();
        }

        /// <summary>
        /// 获得视线移动的输入
        /// </summary>
        /// <returns></returns>
        public Vector3 GetSightMove()
        {
            return inputActions.Player.Look.ReadValue<Vector2>();
        }
        /// <summary>
        /// 隐藏鼠标
        /// </summary>

        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        /// <summary>
        /// 显示鼠标
        /// </summary>
        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
