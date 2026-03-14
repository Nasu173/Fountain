using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 提供对话时看向某个物体的演出所需要的数据
    /// </summary>
    public class LookData : MonoBehaviour, IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要看向的东西")] 
        public Transform target;
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        [Tooltip("看向目标的速度")]
        public float transitionSpeed;
        [HideInInspector]
        public PlayerMove playerMove;
        private void Start()
        {
            playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
        }
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }
    }
}
