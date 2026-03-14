using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Dialogue
{
    /// <summary>
    /// 对话时播放声音的数据
    /// </summary>
    public class SoundData : MonoBehaviour, IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        //声音资源
        public string soundStr;
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }
    }
}
