using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 玩家视野,实现相机相关的行为
    /// </summary>
    public class PlayerSight : MonoBehaviour
    {
        private Camera sightCamera;
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
        private void Start()
        {
            sightCamera = this.GetComponent<Camera>();
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
            sightCamera.transform.localRotation = Quaternion.Euler
                (new Vector3(cameraRotationAngle, 0, 0));
        }

    }
}
