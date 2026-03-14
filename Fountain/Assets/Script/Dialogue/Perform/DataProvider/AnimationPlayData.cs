using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Dialogue
{
    /// <summary>
    /// 对话时播放动画的数据
    /// </summary>
    public class AnimationPlayData : MonoBehaviour, IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        //动画名称
        public string animName;
        //public Animator anim;
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }
    }
}
