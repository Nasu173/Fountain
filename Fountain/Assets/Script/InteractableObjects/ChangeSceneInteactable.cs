using Fountain.Common;
using Fountain.InputManagement;
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
        private OutlineVisual outlineVisual;
        public string completeTaskId;
        private bool canInteract=false;
        public bool CanInteract 
        { get { return canInteract; } set { canInteract = value; } } 
        public void Deselect()
        {
            outlineVisual.SetOutline(false);
        }
        public void Select()
        {
            outlineVisual.SetOutline(true);
        }


      
        public void InteractWith(PlayerInteractor player)
        {
            this.canInteract = false;
            //切换场景
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = gameSceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });
            GameEventBus.Publish<TaskProgressEvent>(new TaskProgressEvent
                (){ TaskId = this.completeTaskId, Amount = 1 });
        }
    }
}
