using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.Player
{
    public class BlackScreenInteractable : MonoBehaviour,IInteractable
    {
        [Header("黑屏效果设置")]
        public float fadeInTime;
        public float fadeOutTime;
        public float duration;
        public Image fadeImage;

        private bool canInteract = false;
        public bool CanInteract
        { get { return canInteract; } set { canInteract = value; } }
        public void Deselect()
        {
        }
        public void InteractWith(PlayerInteractor player)
        {
            GameEventBus.Publish<FadeEvent>(new FadeEvent()
            {
                fadeInTime = this.fadeInTime,
                fadeImage = this.fadeImage,
                duration = this.duration,
                fadeOutTime = this.fadeOutTime
            });
            canInteract = false;
        }
        public void Select()
        {
        }
    }
}
