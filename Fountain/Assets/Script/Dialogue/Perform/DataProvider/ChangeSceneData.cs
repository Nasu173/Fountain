using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class ChangeSceneData:MonoBehaviour ,IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        public string loadSceneAddress;
        [HideInInspector]
        public string unloadSceneAddress;
        public float delay;
        private void Start()
        {
            unloadSceneAddress = this.gameObject.scene.name; 
        }
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }
    }
}
