using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Foutain.Scene;
using Fountain.Player;

public class SwitchScene : MonoBehaviour, IInteractable
{
    [SerializeField] private string _gameSceneAddress;
    public float duration;
    public float fadeInTime;
    public float fadeOutTime;

    public bool CanInteract { get ; set ;}
    
    public void Deselect(){}

    public void InteractWith(PlayerInteractor player)
    {
        SwitchToScene();
    }

    public void Select(){}

    public void SwitchToScene()
    {
        GameEventBus.Publish<FadeEvent>(new FadeEvent()
        {
            fadeInTime = fadeInTime,
            fadeOutTime = fadeOutTime,
            duration = duration
        });
        StartCoroutine(DelayChangeScene());
    }
    private IEnumerator DelayChangeScene()
    {
        yield return new WaitForSeconds(fadeInTime);
        GameEventBus.Publish(new LoadSceneEvent
        {
            SceneAddress = _gameSceneAddress,
            Additive = true,
            SceneToUnload = gameObject.scene.name
        });
        
    }
}
