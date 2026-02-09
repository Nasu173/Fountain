using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 玩家移动类,只实现移动的功能,提供控制移动的方法供外部调用
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        [Header("移动设置")]
        [Tooltip("行走速度")]
        [SerializeField]
        private float walkSpeed;
        [Tooltip("奔跑速度倍率 (相对于行走速度)")]
        [SerializeField]
        private float runMultiplier;
        [Tooltip("蹲伏速度倍率 (相对于行走速度)")]
        [SerializeField]
        private float crouchMultiplier;

        //这三个表示移动状态
        [SerializeField]
        private bool crouching;
        [SerializeField]
        private bool moving;
        [SerializeField]
        private bool running;

        private CharacterController characterController;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        /// <summary>
        /// 移动 
        /// </summary>
        /// <param name="inputDirection">玩家输入的方向</param>
        public void Move(Vector3 inputDirection)
        {
            if (inputDirection==Vector3.zero)
            {
                moving = false;
                return;
            }
            else
            {
                moving = true;
            }

            Vector3 direction =
                this.transform.forward * inputDirection.z +
                this.transform.right * inputDirection.x;
            direction.Normalize();
            float currentSpeed = CalculateSpeed();
            //给一点向下的速度(Vector3.down)保持贴在地面的状态
            Vector3 finalMove =
                (direction + Vector3.down) * currentSpeed * Time.deltaTime;
            Debug.Log(finalMove); 
            characterController.Move(finalMove);
        }
        /// <summary>
        /// 切换到跑步
        /// </summary>
        public void SwitchToRun()
        {
            //下蹲的时候不能跑
            if (!crouching)
            {
                this.running = true;
            }
        }
        /// <summary>
        /// 切换到行走
        /// </summary>
        public void SwitchToWalk()
        {
            running = false;
        }
        /// <summary>
        /// 切换下蹲状态,(下蹲《==》站立)
        /// </summary>
        public void SwitchCrouch()
        {
            if (crouching)
            {
                //TODO:检测头上是否有东西
                crouching = false;
            }
            else
            {
                crouching = true;
            }
        }
        /// <summary>
        /// 玩家旋转
        /// </summary>
        /// <param name="moveDelta">视线的移动方向</param>
        /// <param name="sensitivity">灵敏度</param>
        public void Rotate(Vector2 moveDelta,float sensitivity)
        {
            if (moveDelta==Vector2.zero)
            {
                return;
            }
            float playerRotationAngle = moveDelta.x * sensitivity * Time.deltaTime;
            //由于水平方向没有限制,随便旋转都行,在当前的rotation上转即可
            this.transform.Rotate(Vector3.up * playerRotationAngle); 

        }
        /// <summary>
        /// 计算移动速度
        /// </summary>
        private float CalculateSpeed()
        {
            if (crouching) 
            {
                return walkSpeed * crouchMultiplier;
            }
            else if (running)
            {
                return walkSpeed * runMultiplier;
            }
            else
            {
                return walkSpeed;
            }
    
        }
    }
}
