using Fountain.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    public class Door : MonoBehaviour,IInteractable
    {
        [Tooltip("描边效果,手动拖得了")]
        [SerializeField]
        private OutlineVisual outlineVisual;

        public Vector3 targetRotation;
        public float rotationDuration=1f;
        private bool canInteract = false;
        public bool CanInteract
        { get { return canInteract; } set { canInteract = value; } }
        public void Deselect()
        {
            outlineVisual.SetOutline(false);
        }

        public void InteractWith(PlayerInteractor player)
        {
            StartCoroutine(RotateDoor());
            this.canInteract = false;
        }

        public void Select()
        {
            outlineVisual.SetOutline(true);
        }
        private IEnumerator RotateDoor()
        {
            float elapsed = 0;
            Quaternion startRotation = this.transform.rotation;
            Quaternion endRotation = Quaternion.Euler(targetRotation);
            while (elapsed < rotationDuration)
            {
                this.transform.rotation = Quaternion.Lerp
                    (startRotation, endRotation, elapsed / rotationDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            this.transform.rotation = endRotation;
        }
    }
}
