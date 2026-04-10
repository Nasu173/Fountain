using System;
using System.Collections;
using System.Threading.Tasks;
using Fountain.Common;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 交互时播放音效的可交互物体
    /// </summary>
    public class DoorPlaySound : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioClip openClip;
        [SerializeField] private AudioClip closeClip;
        [SerializeField] private AudioTrack track = AudioTrack.Other;

        [SerializeField] private string[] listenEventID; // 监听的事件ID，如 "TaskProgressEvent"

        [Tooltip("描边效果，可不配置")]
        [SerializeField] private OutlineVisual outlineVisual;

        private bool canInteract = true;
        public bool CanInteract { get => canInteract; set => canInteract = value; }

        void OnEnable()
        {
            GameEventBus.Subscribe<PlayDoorSoundEvent>(OnPlayDoorSoundEvent);
        }

        void OnDisable()
        {
            GameEventBus.Unsubscribe<PlayDoorSoundEvent>(OnPlayDoorSoundEvent);
        }

        private void OnPlayDoorSoundEvent(PlayDoorSoundEvent e)
        {
            StartCoroutine(PlayDoorSound());
        }

        public void InteractWith(PlayerInteractor player)
        {
            StartCoroutine(PlayDoorSound());
        }

        private IEnumerator PlayDoorSound()
        {
            // 播放开门音效
            GameEventBus.Publish(new PlaySoundEvent { Clip = openClip, Track = track });
            yield return new WaitForSeconds(openClip.length);

            // 播放关门音效
            GameEventBus.Publish(new PlaySoundEvent { Clip = closeClip, Track = track });
        }

        public void Select()
        {
            outlineVisual?.SetOutline(true);
        }

        public void Deselect()
        {
            outlineVisual?.SetOutline(false);
        }
    }
}
