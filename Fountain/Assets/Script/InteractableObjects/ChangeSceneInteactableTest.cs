using Fountain.Common;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    public class ChangeSceneInteactableTest : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _gameSceneAddress;

        private OutlineVisual outline;
        private void Start()
        {
            outline = this.transform.GetComponent<OutlineVisual>();
        }
        public void Deselect()
        {
            outline.SetOutline(false);
        }
        public void InteractWith(PlayerInteractor player)
        {
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = _gameSceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });

        }
        public void Select()
        {
            outline.SetOutline(true);
        }
    }
}
