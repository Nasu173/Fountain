using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foutain.Scene;
using Fountain.Player;

public class SwitchScene : MonoBehaviour, IInteractable
{
    [SerializeField] private string _gameSceneAddress;

    public bool CanInteract { get ; set ;}

    public void Deselect(){}

    public void InteractWith(PlayerInteractor player)
    {
        SwitchToScene();
    }

    public void Select(){}

    public void SwitchToScene()
    {
        GameEventBus.Publish(new LoadSceneEvent
        {
            SceneAddress = _gameSceneAddress,
            Additive = true,
            SceneToUnload = gameObject.scene.name
        });
    }
}
