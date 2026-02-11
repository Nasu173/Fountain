using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 玩家视野,实现相机相关的行为,挂到Player下的子物体CameraMountPoint,它是相机的父物体,不要直接挂到相机上
    /// </summary>
    public class PlayerSight : MonoBehaviour
    {
        private CinemachineVirtualCamera sightCamera;
        [Header("相机旋转相关设置")]
        [Tooltip("相机最小旋转角度")]
        [SerializeField]
        private float sightAngleMax;
        [Tooltip("相机最小旋转角度")]
        [SerializeField]
        private float sightAngleMin;
        /// <summary>
        /// 累计旋转的角度
        /// </summary>
        private float cameraRotationAngle=0;
        [Header("抖动的相关参数")]
        [Tooltip("噪声设置")]
        [SerializeField]
        private NoiseSettings shakeNoise;
        [Tooltip("跑步时的震动频率")]
        [SerializeField]
        private float runFrequency;
        [Tooltip("跑步时的振幅")]
        [SerializeField]
        private float runAmplitude;
        [Tooltip("是否启用抖动")]
        public bool enableShake;
        private CinemachineBasicMultiChannelPerlin noise;

        private void Start()
        {
            sightCamera = this.GetComponentInChildren<CinemachineVirtualCamera>();
             noise =sightCamera.
                GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise == null)
            {
                noise = sightCamera.
                        AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }

        }
        //TODO:应用震动效果的时候没有平滑过渡,有点不太好做?如果不需要就先放着先

        /// <summary>
        /// 相机随着视线旋转
        /// </summary>
        /// <param name="moveDelta">视线移动的方向</param>
        /// <param name="sensitivity">灵敏度</param>
        public void Rotate(Vector2 moveDelta,float sensitivity)
        {
            if (moveDelta==Vector2.zero)
            {
                return;
            }
            float playerRotationAngle = moveDelta.x * sensitivity * Time.deltaTime;
            //向上看是绕x轴负的方向旋转,所以是减号,
            cameraRotationAngle -= moveDelta.y * sensitivity * Time.deltaTime;
            cameraRotationAngle = Mathf.Clamp
                (cameraRotationAngle,sightAngleMin,sightAngleMax);
            //这里已经叠加好了,不用*过去了
            this.transform.localRotation = Quaternion.Euler
                (new Vector3(cameraRotationAngle, 0, 0));
        }
        /// <summary>
        /// 应用走路时的震动
        /// </summary>
        public void ApplyWalkShake()
        {
            if (enableShake)
            {
                noise.m_NoiseProfile = shakeNoise;
                noise.m_AmplitudeGain = 1;
                noise.m_FrequencyGain = 1;
            }
        }
        /// <summary>
        /// 应用跑步时的震动
        /// </summary>
        public void ApplyRunShake()
        {
            if (enableShake)
            {
                noise.m_NoiseProfile = shakeNoise;
                noise.m_AmplitudeGain = runAmplitude;
                noise.m_FrequencyGain = runFrequency;
            }
        }
        /// <summary>
        /// 取消震动
        /// </summary>
        public void CancelShake()
        {
            noise.m_NoiseProfile = null;
        }
    }
}
