using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.Dialogue
{
    public class BlackScreenDataProvider :MonoBehaviour, IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        public float fadeInTime;
        public float fadeOutTime;
        public float duration;
        public Image fadeImage;
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }
        
    }
}
