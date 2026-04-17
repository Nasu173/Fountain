using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Common
{
    /// <summary>
    /// 让一个物体看向摄像机
    /// </summary>
    public class LookAtCamera : MonoBehaviour
    {
        [Tooltip("要看向的相机")]
        [SerializeField]
        private Transform target;
        [Tooltip("是否默认看向主相机?")]
        [SerializeField]
        private bool isMain=false;
        public Mode mode;
        private void Start()
        {
            if (isMain)
            {
                target = Camera.main.transform; 
            }
        }
        public enum Mode
        {
            /// <summary>
            /// 看向相机
            /// </summary>
            LookAt,
            /// <summary>
            /// 背面看向相机
            /// </summary>
            LookAtInverted,
            /// <summary>
            /// 朝向与相机的朝向相同
            /// </summary>
            CameraForward,
            CameraForwardInverted
        }
        private void LateUpdate()
        {
            if (target==null)
            {
                return;
            }
            switch (mode)
            {
                case Mode.LookAt:
                    this.transform.LookAt(target.position);
                    break;
                case Mode.LookAtInverted:
                    this.transform.forward = this.transform.position - target.position;
                    break;
                case Mode.CameraForward:
                    this.transform.forward = target.forward;
                    break;
                case Mode.CameraForwardInverted:
                    this.transform.forward = -target.forward;
                    break;
                default:
                    break;
            }
        }
    }
}
