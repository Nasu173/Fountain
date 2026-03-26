using Fountain.InputManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 玩家交互类,负责检测可交互物体并提供交互方法
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        private CharacterInputProvider interactInput;
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
        private IInteractable[] currentTargets;
        private PlayerSight sight;
        /// <summary>
        /// 是否允许交互
        /// </summary>
        private bool enableInteract = true;

        private void Start()
        {
            sight = this.GetComponentInChildren<PlayerSight>();
            interactInput = GameInputManager.Instance.
                GetProvider<CharacterInputProvider>();
        }

        private void Update()
        {
            if (!enableInteract)
            {
                return;
            }

            DetectInteractable();

            if (interactInput != null && interactInput.GetInteract())
            {
                //Debug.LogWarning("正在交互吗?"+interactInput.GetInteract());
                Interact();
            }
        }

        /// <summary>
        /// 与可交互物体交互
        /// </summary>
        public void Interact()
        {
            if (currentTargets != null)
            {
                foreach (var item in currentTargets)
                {
                    if (item.CanInteract)
                    {
                        item.InteractWith(this);
                    }
                }
            }
        }
        /// <summary>
        /// 禁止交互
        /// </summary>
        public void Disable()
        {
            this.enableInteract = false;
            if (currentTargets != null) 
            {
                foreach (var item in currentTargets)
                {
                    if (item.CanInteract)
                    {
                        item.Deselect();
                    }
                }
                //currentTarget.Deselect();
                DeselectInteractable();
                currentTargets = null;
            }
        }
        /// <summary>
        /// 允许交互
        /// </summary>
        public void Enable()
        {
            this.enableInteract = true;        
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
                if (currentTargets != null)
                {
                      foreach (var item in currentTargets)
                      {
                          item.Deselect();
                      }
                     //    currentTarget.Deselect();
                    DeselectInteractable();
                }
                currentTargets = null;
                return;
            }
            // 从命中物体向上查找 IInteractable（支持挂载在任意层级）
            // IInteractable detectedInteractable =
            //     hit.collider.GetComponentInParent<IInteractable>();
            IInteractable[] detectedInteractables =
                hit.collider.GetComponentsInParent<IInteractable>();
            bool hasInteractable = detectedInteractables != null;

            if (!hasInteractable)
            {
                if (currentTargets!=null)
                {
                      foreach (var item in currentTargets)
                      {
                          item.Deselect();
                      }
                    DeselectInteractable();
                }

                currentTargets= null;
                return;
            }
            // 检测到不同的可交互物体
            if (currentTargets != detectedInteractables)
            {
                if (currentTargets!=null)
                {
                      foreach (var item in currentTargets)
                      {
                          item.Deselect();
                      }
                    DeselectInteractable();
                }

                currentTargets = detectedInteractables;
                foreach (var item in currentTargets)
                {
                    if (item.CanInteract)
                    {
                        item.Select();
                        SelectInteractable();
                    }
                }
            }

        }
    }
}
