using Fountain.InputManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 玩家移动类,实现移动的功能,
    /// 虽然实现的并不怎样,臃肿,目前不太清楚移动用什么框架写比较好
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        //输入来源
        private CharacterInputProvider moveInput;
        private PlayerSightInputProvider sightInput;

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
        [Tooltip("是否在不移动的时候也应用重力")]
        [SerializeField]
        private bool autoGravity;

        //这两个表示蹲伏,跑步的状态
        private bool crouching;
        private bool moving;
        private bool running;

        private CharacterController characterController;
        [Header("下蹲设置")]
        [Tooltip("站立高度")]
        [SerializeField]
        private float standingHeight = 2f;
        [Tooltip("蹲伏高度")]
        [SerializeField]
        private float crouchingHeight = 1f;
        [Tooltip("下蹲过渡平滑速度")]
        [SerializeField]
        private float crouchTransitionSpeed = 10f;
        [Tooltip("头顶检测距离 (防止在障碍物下站起)")]
        [SerializeField]
        private float headCheckDistance = 0.5f;
        [Tooltip("头顶检测层级")]
        [SerializeField]
        private LayerMask obstacleLayer;
        //蹲起是否在过渡中
        private bool crouchTransitioning;
        //下蹲&起立要到的高度
        private float targetHeight;

        private PlayerSight sight;//视野和移动还是耦合的紧一些的,就直接用了,最好用事件重构
        [Tooltip("旋转灵敏度")]
        public float sensitivity;
        
        private void Start()
        {
            moveInput = GameInputManager.Instance.GetProvider<CharacterInputProvider>();
            sightInput= GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();
            characterController = this.GetComponent<CharacterController>();
            sight = this.GetComponentInChildren<PlayerSight>();
            targetHeight = standingHeight;
            InitHeight();
        }
        private void Update()
        {
            //移动和转向
            if (moveInput == null || sightInput == null) return;

            Move(moveInput.GetMove());

            if (moveInput.GetCrouch())
            {
                SwitchCrouch();
            }
            if (moveInput.GetRun())
            {
                SwitchToRun();
            }
            else
            {
                SwitchToWalk();
            }

            Rotate(sightInput.GetSightMove(), this.sensitivity);    
            
            if (crouchTransitioning)
            {
                CrouchTransition();    
            }
            
            if (autoGravity)
            {
                //不用真的重力的数值,有向下的量即可
                characterController.Move(Vector3.down);
            }
        }

        /// <summary>
        /// 移动 
        /// </summary>
        /// <param name="inputDirection">玩家输入的方向</param>
        public void Move(Vector3 inputDirection)
        {
            //应用相机震动
            if (inputDirection==Vector3.zero)
            {
                sight.StopShake();
                moving = false;
                return;
            }
            else
            {
                if (crouching||!running)
                {
                    sight.ApplyWalkShake();
                } 
                else
                {
                    sight.ApplyRunShake();
                }
            }

            moving = true;
            //计算方向和速度
            Vector3 direction =
                this.transform.forward * inputDirection.z +
                this.transform.right * inputDirection.x;
            direction.Normalize();
            float currentSpeed = CalculateSpeed();
            //给一点向下的速度(Vector3.down)保持贴在地面的状态
            Vector3 finalMove =
                (direction + Vector3.down) * currentSpeed * Time.deltaTime;
            characterController.Move(finalMove);
        }
        /// <summary>
        /// 切换到跑步
        /// </summary>
        public void SwitchToRun()
        {
            //下蹲的时候不能跑
            if (!crouching && moving) 
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
                //蹲着的时候头顶上没有东西才能站起来
                if (!HeadDetect())
                {
                    crouchTransitioning = true;
                    crouching = false;
                    targetHeight = standingHeight; 
                }
            }
            else//没蹲着的时候
            {
                crouchTransitioning = true;
                crouching = true;
                targetHeight = crouchingHeight;
                running = false;
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
        /// 强制朝向指定地点,只在禁止玩家输入的时候才调用这个方法!
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="speed">转向速度</param>
        public void LookAt(Vector3 target, float speed)
        {
            StartCoroutine(LookAtTransition(target, speed));
        }
        /// <summary>
        /// 强制转向的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator LookAtTransition(Vector3 target,float speed)
        {

            float t = 0;
            Quaternion startBodyRot = this.transform.rotation;
            Quaternion startCamRot = sight.transform.localRotation;

            while (t < 1f)//这个暂时硬编码一个持续时间1
            {
                t += Time.deltaTime * speed;
                //要朝向的方向
                Vector3 direction = (target - sight.transform.position).normalized;
                
                if (direction != Vector3.zero) 
                {
                    Quaternion lookRot = Quaternion.LookRotation(direction);
                    Vector3 euler = lookRot.eulerAngles;
                    
                    //身体只转y轴
                    Quaternion targetBodyRot = Quaternion.Euler(0, euler.y, 0);
                        this.transform.rotation = Quaternion.Slerp
                             (startBodyRot, targetBodyRot, t);

                    //相机只转x轴
                    float targetX = euler.x;
                    Math.Clamp(targetX, sight.sightAngleMin, sight.sightAngleMax);
                    Quaternion targetCamRot = Quaternion.Euler(targetX, 0, 0);
                    Quaternion localRotation =
                        Quaternion.Slerp(startCamRot, targetCamRot, t);
                    //同步旋转
                    sight.Rotate(localRotation.eulerAngles.x);
                }

                 yield return null;
             }

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
        /// <summary>
        /// 实现下蹲时的高度的平滑过渡
        /// </summary>
        private void CrouchTransition()
        {
            characterController.height =
                Mathf.Lerp(characterController.height, targetHeight,
                crouchTransitionSpeed * Time.deltaTime);
            //相等后就停止过渡
            float acceptableDelta = 0.01f;
            if (Mathf.Abs(characterController.height-targetHeight)<acceptableDelta)
            {
                crouchTransitioning = false;
            }
        
            // 修正中心点，防止穿地
            Vector3 center = characterController.center;
            center.y = characterController.height / 2f;
            characterController.center = center;
            //暂时这里直接操控相机,让相机的位置跟着下降
            //假定让相机在CharacterController的顶部
            Vector3 sightPos = sight.transform.localPosition;
            sightPos.y = characterController.height;
            sight.transform.localPosition = sightPos;
        }
        /// <summary>
        /// 检测头顶是否有东西
        /// </summary>
        /// <returns>头顶有东西则返回true</returns>
        private bool HeadDetect()
        {
            /*测试代码,记得删
             
            RaycastHit hit;
             bool has= Physics.BoxCast(this.transform.position, this.transform.localScale * 0.5f,
                    this.transform.up,out hit ,Quaternion.identity,
                    headCheckDistance + standingHeight,obstacleLayer);
            if (has)
            {
                Debug.LogWarning("头上是"+hit.collider.transform.name);
            }
            return has;
             */
            return Physics.BoxCast(this.transform.position, this.transform.localScale * 0.5f,
                    this.transform.up, Quaternion.identity,
                    headCheckDistance + standingHeight,obstacleLayer);
        }
        private void InitHeight()
        {
            characterController.height = standingHeight;
            //保证玩家物体在脚底
            characterController.center = new Vector3(0, standingHeight / 2, 0);
            //假设相机在头顶的位置
            sight.transform.localPosition = new Vector3(0, standingHeight, 0);
        }
    }
}
