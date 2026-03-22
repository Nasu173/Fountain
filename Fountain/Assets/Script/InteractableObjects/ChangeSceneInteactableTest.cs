using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    public class ChangeSceneInteactableTest : MonoBehaviour, IInteractable
    {
        [SerializeField] private string _gameSceneAddress;
        public void Deselect()
        {
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

        }
    }
}
