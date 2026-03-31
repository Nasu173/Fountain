using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 由于玩家在常驻场景,只能手动设置一下玩家的位置
    /// </summary>
    public class ResetPlayerPosition : MonoBehaviour
    {
        [Tooltip("玩家一开始的位置")]
        [SerializeField]
        private Vector3 startPosition;
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
            PlayerInstance.Instance.transform.position = startPosition;
        }
    }
}
