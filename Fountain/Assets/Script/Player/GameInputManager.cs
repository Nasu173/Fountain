using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 输入管理器(单例),管理所有的输入,并调用所有需要玩家输入的方法,比如移动
    /// </summary>
    public class GameInputManager : MonoBehaviour
    {
        public static GameInputManager Instance { get; private set; }
        private PlayerInputActions inputActions;
        
        [SerializeField]
        private float sensitivity = 1;

        private PlayerMove playerMove;
        private PlayerSight playerSight;
        private void Start()
        {
            //进行相关初始化,注册输入事件来调用移动相关方法
            GameInputManager.Instance = this;
            inputActions = new PlayerInputActions();
            playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            playerSight = playerMove.GetComponentInChildren<PlayerSight>();
            inputActions.Player.Enable();
            EnableMoveInput();
            HideCursor();

            inputActions.Player.Run.started += (callback) =>
            { playerMove.SwitchToRun(); };
            inputActions.Player.Run.canceled += (callback) =>
            { playerMove.SwitchToWalk(); };
            inputActions.Player.Crouch.started += (callback) =>
            { playerMove.SwitchCrouch(); };
        }
        private void Update()
        {
            Vector2 moveVal = inputActions.Player.Move.ReadValue<Vector2>();
            playerMove.Move(new Vector3(moveVal.x, 0, moveVal.y));
            Vector2 sightMove = inputActions.Player.Look.ReadValue<Vector2>();
            playerMove.Rotate(sightMove, sensitivity);
            playerSight.Rotate(sightMove, sensitivity);

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
