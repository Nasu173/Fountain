using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace Fountain.InputManagement
{
    /// <summary>
    /// 玩家角色输入提供器,提供移动和交互的输入
    /// </summary>
    public class CharacterInputProvider : MonoBehaviour,IInputProvider
    {
        private PlayerInputActions inputActions;
        /// <summary>
        /// 由于要支持长按的交互,因此用一个变量记录是否“按下交互键”
        /// </summary>
        private bool isInteracting;
        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
            this.isInteracting = false;
            //只要按下的时候认为是在交互
            inputActions.Player.Interact.performed += (context) =>
            {
                this.isInteracting = true;
            };
        }
        private void OnEnable()
        {
            this.inputActions.Player.Enable();
        }
        private void Update()
        {
            if (inputActions.Player.Interact.WasReleasedThisFrame())
            {
                this.isInteracting = false;
            }
        }
        private void OnDisable()
        {
            this.isInteracting = false;
            this.inputActions.Player.Disable();
        }

        /// <summary>
        /// 获得移动输入
        /// </summary>
        /// <returns></returns>
        public Vector3 GetMove()
        {
            Vector2 moveVal = inputActions.Player.Move.ReadValue<Vector2>();
            return new Vector3(moveVal.x, 0, moveVal.y);
        }

        /// <summary>
        /// 获得奔跑输入
        /// </summary>
        /// <returns>是否按下奔跑键</returns>
        public bool GetRun()
        {
            //不知为何,不能直接读取为bool,只能转换下
            return Convert.ToBoolean(inputActions.Player.Run.ReadValue<float>());
        }

        /// <summary>
        /// 获得下蹲输入
        /// </summary>
        /// <returns>是否按下下蹲键</returns>
        public bool GetCrouch()
        {
            return inputActions.Player.Crouch.WasPressedThisFrame();
        }

        /// <summary>
        /// 获得交互输入
        /// </summary>
        /// <returns>是否按下交互键</returns>
        public bool GetInteract()
        {
            return this.isInteracting;
        }
        
        /// <summary>
        /// 获得对话输入
        /// </summary>
        /// <returns>是否按下对话键</returns>
        public bool GetDialogueContinue()
        {
            return inputActions.Player.ContinueDialogue.WasPressedThisFrame();
        }

    }
}
