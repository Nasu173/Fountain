using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Foutain.Player
{
    /// <summary>
    /// 玩家视野,实现相机相关的行为,挂到Player下的子物体CameraMountPoint,它是相机的父物体,不要直接挂到相机上
    /// </summary>
    public class PlayerSight : MonoBehaviour
    {
        private Camera sightCamera;
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

        [Header("走路抖动的相关参数")]
        [Tooltip("是否启用抖动")]
        public bool enableShake;
        [Tooltip("走路时的震动频率")]
        [SerializeField]
        private float frequencyWalk;
        [Tooltip("走路时相机位置各个方向上的振幅")]
        [SerializeField]
        private Vector3 amplitudePositionWalk;
        [Tooltip("走路时相机旋转各个方向上的振幅")]
        [SerializeField]
        private Vector3 amplitudeRotationWalk;

        [Header("跑步抖动的相关参数")]
        [Tooltip("跑步时的震动频率")]
        [SerializeField]
        private float frequencyRun;
        [Tooltip("跑步时的相机位置各个方向上振幅")]
        [SerializeField]
        private Vector3 amplitudePositionRun;
        [Tooltip("跑步时相机旋转各个方向上振幅")]
        [SerializeField]
        private Vector3 amplitudeRotationRun;

        [Header("震动平滑速度设置")]
        [Tooltip("位置变化速度")]
        public float smoothSpeedPosition;
        [Tooltip("旋转变化速度")]
        public float smoothSpeedRotation;

        private float noiseSeed;
        /// <summary>
        /// 初始相机位置
        /// </summary>
        private Vector3 originCameraPosition=Vector3.zero;
        /// <summary>
        /// 初始相机旋转
        /// </summary>
        private Quaternion originCameraRotation = Quaternion.identity;

        private void Start()
        {
            sightCamera = this.GetComponentInChildren<Camera>();
            noiseSeed = Random.Range(0f, 1000f);//1000f只是随便一个数,只要范围适中即可
            // noise =sightCamera.
            //    GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        //    if (noise == null)
            {
            //    noise = sightCamera.
            //            AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }

        }

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
        ///应用走路时的震动 
        /// </summary>
        public void ApplyWalkShake()
        {
            if (!enableShake)
            {
                return;
            }
            Vector3 noisePosition =
                CalculateNoise(frequencyWalk, amplitudePositionWalk, 0);
            Vector3 noiseRotation =
                CalculateNoise(frequencyWalk, amplitudeRotationWalk, 0);

            LerpPosAndRot(originCameraPosition + noisePosition,
                originCameraRotation * Quaternion.Euler(noiseRotation));
        }
        /// <summary>
        /// 应用跑步时的震动
        /// </summary>
        /// <param name="speed">奔跑速度</param>
        public void ApplyRunShake()
        {
            if (!enableShake)
            {
                return;
            }
            //100只是随便给的偏移量而已,与走路时的震动做区分
            Vector3 noisePosition =
                CalculateNoise(frequencyRun, amplitudePositionRun, 100);
            Vector3 noiseRotation =
                CalculateNoise(frequencyRun, amplitudeRotationRun, 100);

            LerpPosAndRot(originCameraPosition + noisePosition,
                originCameraRotation * Quaternion.Euler(noiseRotation));

        }
        /// <summary>
        /// 取消震动
        /// </summary>
        public void CancelShake()
        {
            LerpPosAndRot(originCameraPosition, originCameraRotation);
        }

        /// <summary>
        /// 计算噪声
        /// </summary>
        /// <param name="frequency">频率</param>
        /// <param name="amplitude">各个方向的振幅</param>
        /// <param name="offset">采样点偏移</param>
        /// <returns>噪声</returns>
        private Vector3 CalculateNoise(float frequency,Vector3 amplitude,float offset)
        {
            float time = Time.time * frequency + noiseSeed + offset;

            // 计算噪声,( (*2f -1f)为了让返回值从-1到1变化 )
            float x = (Mathf.PerlinNoise(time, 0f) * 2f - 1f) * amplitude.x;
            float y = (Mathf.PerlinNoise(0f, time) * 2f - 1f) * amplitude.y;
            float z = (Mathf.PerlinNoise(time, time) * 2f - 1f) * amplitude.z;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 平滑过渡 
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <param name="targetRotation"></param>
        private void LerpPosAndRot(Vector3 targetPosition,Quaternion targetRotation)
        {
            sightCamera.transform.localPosition =
                Vector3.Lerp(sightCamera.transform.localPosition,
                targetPosition, Time.deltaTime * smoothSpeedPosition);

            sightCamera.transform.localRotation =
                Quaternion.Slerp(sightCamera.transform.localRotation,
                targetRotation, Time.deltaTime * smoothSpeedRotation);
        }
    }
}
