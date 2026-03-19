using Fountain.Common;
using Fountain.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fountain.InputManagement
{
    /*由于旧的输入管理器与别的模块耦合程度到了必须要重构的地步,对此重构
     旧版:GameInputManager--调用-->其他模块
     
    重构版本:
    GameInputManager<----------获得Provider-----其它模块
            ↓                                       从Provider里获得输入,处理相关逻辑
    管理所有的InputProvider                   
     */

    /// <summary>
    /// 输入管理器(单例),管理所有的InputProvider,提供InputProvider需要的数据
    /// 所有的InputProvider和manager挂在同一个物体上
    /// </summary>
    public class GameInputManager :MonoSingleton<GameInputManager> 
    {
        /// <summary>
        /// 简单地放在这里先,虽然作为这样的管理器放这里并不合适
        /// </summary>
        private PlayerInputActions inputActions;
        /// <summary>
        /// 整个游戏的InputProviders
        /// </summary>
        private List<IInputProvider> inputProviders;

        protected override void Init()
        {
            base.Init();
            if (Instance!=this)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
            inputActions = new PlayerInputActions();

            // 自动添加缺失的 Provider 组件
            if (GetComponent<CharacterInputProvider>() == null)
                gameObject.AddComponent<CharacterInputProvider>();
            if (GetComponent<PlayerSightInputProvider>() == null)
                gameObject.AddComponent<PlayerSightInputProvider>();
            if (GetComponent<UIInputProvider>() == null)
                gameObject.AddComponent<UIInputProvider>();
            if (GetComponent<CursorManager>() == null)
                gameObject.AddComponent<CursorManager>();

            inputProviders = this.GetComponents<IInputProvider>().ToList();
            //inputActions.Enable();
        }
        private void OnDestroy()
        {
            inputActions.Dispose();
            inputActions = null;
        }

        /// <summary>
        /// 获得输入系统的引用,不要自行new一个新的实例
        /// </summary>
        /// <returns></returns>
        public PlayerInputActions GetInputAction()
        {
            return this.inputActions;
        }
        public T GetProvider<T>()where T : IInputProvider
        {
            return 
                (T)inputProviders.Find((arg) =>
            {
                return arg is T;
            });
        }
    }
}



/*
        //public float sensitivity = 1;

        //一些需要玩家输入的脚本
        private PlayerMove playerMove;
        private PlayerSight playerSight;
        private PlayerInteractor playerInteractor;
        private void Start()
        {
            //初始化组件
            playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            playerSight = playerMove.GetComponentInChildren<PlayerSight>();
            playerInteractor = playerMove.GetComponent<PlayerInteractor>();

            //启用对应的ActionMap
            inputActions.Player.Enable();
            //EnablePausePanel();
            EnableInteractInput();
            //HideCursor();

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
 
 
 
 */
