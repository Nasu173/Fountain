using Fountain.Player;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fountain.MiniGame.ControlFountain
{
    public class ChangeSceneOnVictory : MonoBehaviour
    {
        public float changeDelay;
        public string _gameSceneAddress;
        public float xOffset; 
        private void OnEnable()
        {
            GameEventBus.Subscribe<ControlFountainEndEvent>(ChangeScene);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<ControlFountainEndEvent>(ChangeScene);
        }
        private void ChangeScene(ControlFountainEndEvent e)
        {
            StartCoroutine(Change());
            PlayerInstance.Instance.transform.position =
            PlayerInstance.Instance.transform.position + new Vector3(xOffset, 0, 0);
        }
        private IEnumerator Change()
        {
            yield return new WaitForSeconds(changeDelay);
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = _gameSceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });
        }
    }
}
