using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 对话时移动的数据
    /// </summary>
    public class WalkData : MonoBehaviour, IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        [Tooltip("要移动到的位置")]
        public Vector3 targetPosition;
        [Tooltip("移动总时间")]
        public float duration;
        public NPCMove npcMove;
        //动画名称
        public string walkAnimName;
        public Animator anim;
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }

    }
}
