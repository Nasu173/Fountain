using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 玩家交互类,负责检测可交互物体并提供交互方法
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        //这两个事件目前用于显示交互的提示
        /// <summary>
        /// 选中可交互物体
        /// </summary>
        public event Action SelectInteractable;
        /// <summary>
        /// 取消选中可交互物体
        /// </summary>
        public event Action DeselectInteractable;

        [Header("交互设置")]
        [Tooltip("交互的最大距离")]
        [SerializeField]
        private float interactDistance;
        [Tooltip("可交互物体的层级")]
        [SerializeField]
        private LayerMask detectLayer;

        /// <summary>
        /// 当前检测到的物体
        /// </summary>
        private IInteractable currentTarget;
        private PlayerSight sight;

        private void Start()
        {
            sight = this.GetComponentInChildren<PlayerSight>();
        }

        private void Update()
        {
            DetectInteractable();
        }

        /// <summary>
        /// 检测可交互物体
        /// </summary>
        private void DetectInteractable()
        {
            //检测物体
            bool detected = Physics.Raycast(sight.transform.position, sight.transform.forward, out RaycastHit hit, interactDistance, detectLayer);
            if (!detected)
            {
                currentTarget?.Deselect();
                DeselectInteractable();
                currentTarget = null;
                return;
            }

            if (!hit.collider.TryGetComponent<IInteractable>(out var detectedInteractable))
            {
                currentTarget?.Deselect();
                DeselectInteractable();

                currentTarget = null;
                return;
            }
            // 检测到不同的可交互物体
            if (currentTarget != detectedInteractable)
            {
                currentTarget?.Deselect();
                DeselectInteractable();

                currentTarget = detectedInteractable;

                currentTarget.Select();
                SelectInteractable();
            }

        }

        /// <summary>
        /// 与可交互物体交互
        /// </summary>
        public void Interact()
        {
            Debug.Log("Interact!!");

            if (currentTarget != null)
            {
                // 原有的交互逻辑
                currentTarget.InteractWith(this);

                // 检查是否有 SimpleTaskInteractable
                if (currentTarget is MonoBehaviour mono)
                {
                    if (mono.TryGetComponent<SimpleTaskInteractable>(out var simpleInteractable))
                    {
                        simpleInteractable.OnPlayerInteract();
                    }
                }
            }
        }

    }
}
