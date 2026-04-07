using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 由于玩家在常驻场景,只能手动设置一下玩家的位置
    /// </summary>
    public class ResetPlayerPositionWhenChangeScene : MonoBehaviour
    {
        [Tooltip("玩家一开始的位置")]
        [SerializeField]
        private Vector3 startPosition;
        [Tooltip("玩家一开始的旋转")]
        [SerializeField]
        private Vector3 startRotation;
        [Tooltip("加载到这个场景才重置玩家位置")]
        [SerializeField]
        private string targetSceneAddress;
        private void OnEnable()
        {
            GameEventBus.Subscribe<LoadSceneEvent>(SetPlayerPosition);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<LoadSceneEvent>(SetPlayerPosition);
        }
        private void SetPlayerPosition(LoadSceneEvent e)
        {
            if (e.SceneAddress!=targetSceneAddress)
            {
                return;
            }
            PlayerInstance.Instance.transform.position = startPosition;
            PlayerInstance.Instance.transform.eulerAngles = startRotation;
        }
    }
}
