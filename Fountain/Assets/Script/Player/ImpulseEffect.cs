using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 让玩家视角产生震动
    /// </summary>
    public class ImpulseEffect : MonoBehaviour
    {
        private CinemachineImpulseSource impulseSource;
        private Transform player;
        private void Start()
        {
            impulseSource = this.GetComponent<CinemachineImpulseSource>();
            player = PlayerInstance.Instance.transform;
        }

        /*
        测试代码,固定间隔产生震动 
        private float elapsed = 0;
        private float interval = 2;
        private void Update()
        {
            //测试
            elapsed += Time.deltaTime;
            if (elapsed>=2)
            {
                GenerateImpulseWithDirection();
                elapsed = 0;
            }
           
        }
         */


        /// <summary>
        /// 向玩家相机产生震动效果
        /// </summary>
        public void GenerateImpulse()
        {
            impulseSource.GenerateImpulse();
        }
        /// <summary>
        /// 如果震动的相关设置没改变过的话,这个方法根据相对位置产生震动
        /// 比如振动源在玩家右边,玩家相机朝右边震动
        /// </summary>
        public void GenerateImpulseWithDirection()
        {
            Vector3 velocity = impulseSource.m_DefaultVelocity;
            Vector3 direction = (this.transform.position - player.transform.position);

            //根据震动源在玩家的左/右边决定x轴震动的方向
            velocity.x = Math.Abs(velocity.x);
            if (direction.x<0)
            {
                velocity.x *= -1;
            }

            impulseSource.GenerateImpulse(velocity);
        }
    }
}
