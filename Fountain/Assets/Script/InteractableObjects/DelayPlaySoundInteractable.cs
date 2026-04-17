using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    public class DelayPlaySoundInteractable : MonoBehaviour, IInteractable
    {
        public float delay;
        public AudioClip sound;
        public AudioTrack track;
        public bool loop;
        private bool canInteract = false;
        public bool CanInteract
        { get { return canInteract; } set { canInteract = value; } }
        public void Deselect()
        {
        }

        public void InteractWith(PlayerInteractor player)
        {
            StartCoroutine(DelaySound());
        }

        public void Select()
        {

        }
        private IEnumerator DelaySound()
        {
            yield return new WaitForSeconds(delay);     
            GameEventBus.Publish(new PlaySoundEvent 
            { Clip = sound, Track = track,IsLoop=loop });
        }
    }
}
