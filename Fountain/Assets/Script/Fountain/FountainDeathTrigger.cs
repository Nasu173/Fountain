using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fountain.Player;
using Fountain.InputManagement;

public class FountainDeathTrigger : MonoBehaviour
{
    [SerializeField] private string sceneAddress = "Underground";
    [SerializeField] private string sceneToUnload;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            var playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>();
            if (playerMove != null) playerMove.enabled = false;
            if (playerSight != null) playerSight.enabled = false;
            GameInputManager.Instance.GetProvider<PauseInputProvider>().enabled = false;

            GameEventBus.Publish<RespawnEvent>(new RespawnEvent()
            {
                backgroundColor = Color.red,
                textColor = Color.black,
                fadeInTime = 1f,
                sceneToLoad = this.sceneAddress,
                sceneToUnload = this.sceneToUnload,
            });
        }
    }
}
