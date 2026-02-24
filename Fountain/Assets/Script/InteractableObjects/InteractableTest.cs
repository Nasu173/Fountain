using Foutain.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Player
{
    /// <summary>
    /// 交互测试脚本,测试完记得删除
    /// </summary>
    public class InteractableTest : MonoBehaviour, IInteractable
    {
        private OutlineVisual outlineVisual;
        private void Start()
        {
            outlineVisual = this.GetComponent<OutlineVisual>();
        }
        public void Deselect()
        {
            outlineVisual.SetOutline(false);
        }

        public void InteractWith(PlayerInteractor player)
        {
            Debug.LogFormat("与{0}交互",player.name);
        }

        public void Select()
        {
            outlineVisual.SetOutline(true);
        }
    }
}
