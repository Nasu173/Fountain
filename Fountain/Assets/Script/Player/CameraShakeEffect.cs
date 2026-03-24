using Cinemachine;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 基于Cinemachine实现的相机持续的抖动效果控制,将这个脚本挂在要抖动的虚拟相机上
    /// </summary>
    public class CameraShakeEffect : MonoBehaviour
    {
        /// <summary>
        /// 要控制的虚拟相机
        /// </summary>
        private CinemachineVirtualCamera vcam;
        /// <summary>
        /// 虚拟相机上的柏林噪声
        /// </summary>
        private CinemachineBasicMultiChannelPerlin vcamPerlin;

        [Header("噪声设置")]
        [Tooltip("噪声配置文件")]
        [SerializeField]
        private NoiseSettings noiseProfile;
        [Tooltip("切换噪声的过渡时间")]
        [SerializeField]
        private float duration;
        /// <summary>
        /// 是否在过渡中
        /// </summary>
        private bool transitioning;

        //过渡用的中间变量
        private float elapsed;
        private float startAmplitude;
        private float startFrequency;
        private float targetAmplitude;
        private float targetFrequency;
        private void Awake()
        {
            vcam = this.GetComponent<CinemachineVirtualCamera>();
            vcamPerlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            vcamPerlin.m_NoiseProfile = noiseProfile;
        }
        private void Update()
        {
            if (transitioning)
            {
                Transition();
            }
        }

        /// <summary>
        /// 设置噪声,由于要过渡效果,不要在过渡期间频繁调用
        /// </summary>
        /// <param name="amplitude">振幅</param>
        /// <param name="frequency">频率</param>
        public void SetNoise(float amplitude, float frequency)
        {
            elapsed = 0;
            transitioning = true;
            startAmplitude = vcamPerlin.m_AmplitudeGain;
            startFrequency = vcamPerlin.m_FrequencyGain;
            targetAmplitude = amplitude;
            targetFrequency = frequency; 
        }
        /// <summary>
        /// 停止噪声,相当于把振幅调成0
        /// </summary>
        /// <param name="immediate">是否立即停止震动</param>
        public void Mute(bool immediate = false)
        {
            if (immediate)
            {
                vcamPerlin.m_AmplitudeGain = 0;
                return;
            }
            SetNoise(0, vcamPerlin.m_FrequencyGain);
        }

        /// <summary>
        /// 平滑过渡
        /// </summary>
        private void Transition()
        {
            if (elapsed >= duration)
            {
                transitioning = false;
                vcamPerlin.m_AmplitudeGain = targetAmplitude;
                vcamPerlin.m_FrequencyGain = targetFrequency;
                return;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // 使用 SmoothStep 可以让过渡更自然，也可以直接用 Lerp
            float smoothT = Mathf.SmoothStep(0, 1, t);

            vcamPerlin.m_AmplitudeGain = Mathf.Lerp(startAmplitude, targetAmplitude, smoothT);
            vcamPerlin.m_FrequencyGain = Mathf.Lerp(startFrequency, targetFrequency, smoothT);

        }
    }
}
