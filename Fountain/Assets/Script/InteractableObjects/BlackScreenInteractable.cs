using Fountain.Common;
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
        public bool StartTask;
        public string[] taskIds;
        [SerializeField]
        private OutlineVisual visual;

        private bool canInteract = false;
        public bool CanInteract
        { get { return canInteract; } set { canInteract = value; } }
        public void Deselect()
        {
            visual.SetOutline(false);
        }
        public void InteractWith(PlayerInteractor player)
        {
            StartCoroutine(TaskStart());
            GameEventBus.Publish<FadeEvent>(new FadeEvent()
            {
                fadeInTime = this.fadeInTime,
                fadeImage = this.fadeImage,
                duration = this.duration,
                fadeOutTime = this.fadeOutTime
            });
            canInteract = false;
            player.Deselect(); //设计的不足才导致要写这行代码
           // visual.SetOutline(false);
        }
        public void Select()
        {
            visual.SetOutline(true);
        }

        private IEnumerator TaskStart()
        {
            yield return new WaitForSeconds(fadeInTime + duration + fadeOutTime);

            if (StartTask)
            {
                for (int i = 0; i < taskIds.Length; i++)
                {
                    GameEventBus.Publish<ScriptTriggerEvent>(new ScriptTriggerEvent()
                    {
                        TriggerId = taskIds[i]
                    });
                }
            }
        }
    }
}