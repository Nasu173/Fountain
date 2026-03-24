using Fountain.InputManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 玩家视野,实现相机相关的行为,挂到Player下的子物体CameraMountPoint,
    /// 它是相机的父物体,不要直接挂到相机上
    /// </summary>
    public class PlayerSight : MonoBehaviour
    {
        /// <summary>
        /// 相机抖动效果实现类
        /// </summary>
        private CameraShakeEffect shakeEffect;
        /// <summary>
        ///是否允许旋转 
        /// </summary>
        private bool enableShake=true;
        [Header("相机旋转相关设置")]
        [Tooltip("相机最小旋转角度")]
        [SerializeField]
        public float sightAngleMax;
        [Tooltip("相机最小旋转角度")]
        [SerializeField]
        public float sightAngleMin;
        [Tooltip("旋转灵敏度")]
        public float sensitivity;

        private PlayerSightInputProvider sightInput;

        /// <summary>
        /// 累计旋转的角度
        /// </summary>
        private float cameraRotationAngle=0;

        [Header("震动效果相关设置")]
        [Header("移动时的震动")]
        [Tooltip("走路震动振幅")]
        [SerializeField]
        private float amplitudeWalk;
        [Tooltip("走路震动频率")]
        [SerializeField]
        private float frequencyWalk;
        [Header("跑步时的震动")]
        [Tooltip("跑步震动振幅")]
        [SerializeField]
        private float amplitudeRun;
        [Tooltip("跑步震动频率")]
        [SerializeField]
        private float frequencyRun;

        //由于移动脚本写的不好,反过来影响这个脚本,气笑了
        private bool isRunningShake;
        private bool isWalkingShake;
        private bool hasShake;

        private void Start()
        {
            isRunningShake = false;
            isWalkingShake = false;
            hasShake = false;
            shakeEffect = this.GetComponentInChildren<CameraShakeEffect>();
            shakeEffect.Mute(true);
            sightInput= GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();
        }
        private void Update()
        {
            if (sightInput == null) return;
            Rotate(sightInput.GetSightMove(), this.sensitivity);
        }
        /// <summary>
        /// 相机随着视线旋转
        /// </summary>
        /// <param name="moveDelta">视线移动的方向</param>
        /// <param name="sensitivity">灵敏度</param>
        public void Rotate(Vector2 moveDelta, float sensitivity)
        {
            if (moveDelta == Vector2.zero) return;
            cameraRotationAngle -= moveDelta.y * sensitivity * Time.deltaTime;
            cameraRotationAngle = Mathf.Clamp(cameraRotationAngle, sightAngleMin, sightAngleMax);

            this.transform.localRotation = Quaternion.Euler(new Vector3(cameraRotationAngle, 0, 0));
        }
        /// <summary>
        /// 让相机旋转至指定角度
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(float angle)
        {
            cameraRotationAngle = angle;
            this.transform.localRotation = Quaternion.Euler(new Vector3(cameraRotationAngle, 0, 0));
            
        }

        //下面的这些震动方法在对应运动状态开始时调用一次即可,否则会出现一些问题

        /// <summary>
        ///应用走路时的震动 
        /// </summary>
        public void ApplyWalkShake()
        {
            if (!enableShake||isWalkingShake)
            {
                return;
            }
            isWalkingShake = true;
            isRunningShake = false;
            hasShake = true;
            shakeEffect.SetNoise(amplitudeWalk, frequencyWalk);
        }
        /// <summary>
        /// 应用跑步时的震动
        /// </summary>
        public void ApplyRunShake()
        {
            if (!enableShake||isRunningShake)
            {
                return;
            }
            isWalkingShake = false;
            isRunningShake = true;
            hasShake = true;
            shakeEffect.SetNoise(amplitudeRun, frequencyRun);
        }
        /// <summary>
        /// 停止震动
        /// </summary>
        public void StopShake()
        {
            if (!hasShake)
            {
                return;
            }
            isWalkingShake = false;
            isRunningShake = false;
            hasShake = false;
            shakeEffect.Mute();
        }

        /// <summary>
        /// 禁用旋转
        /// </summary>
        public void DisableShake()
        {
            shakeEffect.Mute(true);
            this.enableShake = false;    
        }
        /// <summary>
        /// 开启旋转
        /// </summary>
        public void EnableShake()
        {
            this.enableShake = true;
        }
    }
}
