using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.InputManagement
{
    public class GunInputProvider : MonoBehaviour,IInputProvider
    {
        private PlayerInputActions inputActions;
        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction(); 
        }
        private void OnEnable()
        {
            this.inputActions.FountainGun.Enable();
        }
        private void OnDisable()
        {
            this.inputActions.FountainGun.Disable();
        }
        /// <summary>
        /// 枪的移动输入
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMove()
        {
            float inputVal= this.inputActions.FountainGun.Move.ReadValue<float>();
            return new Vector3(inputVal, 0, 0);
        }
        /// <summary>
        /// 是否按下开火键
        /// </summary>
        /// <returns></returns>
        public bool GetFire()
        {
            return this.inputActions.FountainGun.Fire.WasPressedThisFrame();
        }

    }
}
