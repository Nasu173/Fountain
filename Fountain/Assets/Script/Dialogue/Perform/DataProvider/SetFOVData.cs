using Cinemachine;
using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class SetFOVData : MonoBehaviour, IPerformDataProvider
    {
        [Header("演出数据")]
        [Tooltip("要演出的对话的节点在序列里的索引")]
        public int dialogueNodeIndex;
        [Tooltip("要恢复到的FOV")]
        public float targetFOV;
        [Tooltip("复原的持续时间")]
        public float duration;
        [HideInInspector]
        public CinemachineVirtualCamera cam;
        private void Start()
        {
            cam = Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
        }
        public int GetTargetIndex()
        {
            return dialogueNodeIndex;
        }
    }
}
