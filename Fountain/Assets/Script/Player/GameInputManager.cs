using Foutain.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Foutain.Player
{
    /// <summary>
    /// 输入管理器(单例),管理所有的输入,并调用所有需要玩家输入的方法,比如移动
    /// </summary>
    public class GameInputManager : MonoBehaviour
    {
        public static GameInputManager Instance { get; private set; }
        private PlayerInputActions inputActions;

        public float sensitivity = 1;

        //一些需要玩家输入的脚本
        private PlayerMove playerMove;
        private PlayerSight playerSight;
        private PlayerInteractor playerInteractor;
        private void Awake()
        {
            GameInputManager.Instance = this;
        }
        private void Start()
        {
            //初始化组件
            inputActions = new PlayerInputActions();
            playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            playerSight = playerMove.GetComponentInChildren<PlayerSight>();
            playerInteractor = playerMove.GetComponent<PlayerInteractor>();

            //启用对应的ActionMap
            inputActions.Player.Enable();
            EnablePausePanel();
            EnableInteractInput();
            HideCursor();

            //注册移动、交互相关事件
            inputActions.Player.Run.canceled += (callback) =>
            { playerMove.SwitchToWalk(); };
            inputActions.Player.Crouch.started += (callback) =>
            { playerMove.SwitchCrouch(); };
            inputActions.PausePanel.Pause.started += (callback) =>
            {
                GameEventBus.Publish<GamePauseEvent>(new GamePauseEvent());
            };
            inputActions.Player.Interact.performed += (callback) =>
            {
                playerInteractor.Interact();
            };

            //注册对话事件
            GameEventBus.Subscribe<DialogueBeginEvent>((e) =>
            {
                inputActions.Player.ContinueDialogue.performed += ContinueDialogue;
            });
            GameEventBus.Subscribe<DialogueEndEvent>((e) =>
            {
                inputActions.Player.ContinueDialogue.performed -= ContinueDialogue;
            });
        }
        private void Update()
        {
            //不知为何,不能直接读取为bool,只能转换下
            if (Convert.ToBoolean(inputActions.Player.Run.ReadValue<float>()))
            {
                playerMove.SwitchToRun();
            }
            Vector2 moveVal = inputActions.Player.Move.ReadValue<Vector2>();
            playerMove.Move(new Vector3(moveVal.x, 0, moveVal.y));

            Vector2 sightMove = inputActions.Player.Look.ReadValue<Vector2>();
            playerMove.Rotate(sightMove, sensitivity);
            if (playerSight != null)
            {
                playerSight.Rotate(sightMove, sensitivity); // 确保传入sensitivity
            }
        }
        private void OnDestroy()
        {
            inputActions.Dispose();
            inputActions = null;
        }

        /// <summary>
        /// 禁止移动输入
        /// </summary>
        public void DisableMoveInput()
        {
            inputActions.Player.Move.Disable();
            inputActions.Player.Run.Disable();
            inputActions.Player.Crouch.Disable();
        }
        /// <summary>
        /// 启用移动输入
        /// </summary>
        public void EnableMoveInput()
        {
            inputActions.Player.Move.Enable();
            inputActions.Player.Run.Enable();
            inputActions.Player.Crouch.Enable();
        }
        /// <summary>
        /// 禁止旋转相机视野的输入
        /// </summary>
        public void DisableSightInput()
        {
            inputActions.Player.Look.Disable();    
        }
        /// <summary>
        /// 启动旋转相机视野的输入
        /// </summary>
        public void EnableSightInput()
        {
            inputActions.Player.Look.Enable();    
        }
        /// <summary>
        /// 禁止交互输入
        /// </summary>
        public void DisableInteractInput()
        {
            inputActions.Player.Interact.Disable();
            playerInteractor.Disable();
        }
        public void EnableInteractInput()
        {
            inputActions.Player.Interact.Enable();
            playerInteractor.Enable();
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

        public void DisablePausePanel()
        {
            inputActions.PausePanel.Disable();
        }
        public void EnablePausePanel()
        {
            inputActions.PausePanel.Enable();
        }

        private void ContinueDialogue(InputAction.CallbackContext callback)
        {
            DialogueManager.Instance.ContinueDialogue();
        }
    }
}
