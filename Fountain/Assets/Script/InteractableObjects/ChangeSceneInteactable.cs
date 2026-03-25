using Fountain.Common;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 更换场景的可交互物体
    /// </summary>
    public class ChangeSceneInteactable : MonoBehaviour, IInteractable
    {
        [Tooltip("要切换到的场景的Addressable名字")]
        [SerializeField] 
        private string gameSceneAddress;

        [Tooltip("描边效果,手动拖得了")]
        [SerializeField]
        private OutlineVisual[] outlineVisuals;
        public void Deselect()
        {
            SetOutline(false);
        }
        public void Select()
        {
            SetOutline(true);
        }
      
        public void InteractWith(PlayerInteractor player)
        {
            //切换场景
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = gameSceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });

        }

        private void SetOutline(bool visible)
        {
            foreach (var item in outlineVisuals)
            {
                item.SetOutline(visible);
            }
        }
    }
}
