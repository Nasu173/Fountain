using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 玩家单例,仅是提供一个方便找玩家的单例,可以通过这个GetComponent来找玩家的相关脚本
    /// </summary>
    [RequireComponent(typeof(PlayerMove))]
    public class PlayerInstance : MonoBehaviour
    {
        public static PlayerInstance Instance { get; private set; }
        private void Awake()
        {
            PlayerInstance.Instance = this;
        }
    }
}
