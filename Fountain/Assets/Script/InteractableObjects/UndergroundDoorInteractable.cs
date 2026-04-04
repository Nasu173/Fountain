using Fountain.Common;
using Fountain.Dialogue;
using Fountain.InputManagement;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    public class UndergroundDoorInteractable : MonoBehaviour, IInteractable
    {
        [Tooltip("描边效果,手动拖得了")]
        [SerializeField]
        private OutlineVisual outlineVisual;
        public float doorDuration;//开门总时间
        private Transform door;
        public float doorXRotation;
        public float playerXRotation;
        public float playerDuration;

        public string sceneAddress;

        public float monsterDistance=1f;//突脸的时候在玩家身后的距离
        public Transform monster;
        public LookData data;
       // public Vector3 targetPosition;
        //public float pushDuration;

        private bool canInteract = false;

        public bool CanInteract
        { get { return canInteract; } set { canInteract = value; } }
        private void Start()
        {
            door = this.transform.GetChild(0);
        }
        public void Deselect()
        {
            outlineVisual.SetOutline(false);
        }
        public void Select()
        {
            outlineVisual.SetOutline(true);
        }

        public void InteractWith(PlayerInteractor player)
        {
            GameInputManager.Instance.GetProvider<CharacterInputProvider>()
                .enabled = false;
            GameInputManager.Instance.GetProvider<PlayerSightInputProvider>()
                .enabled = false;

            StartCoroutine(Open());
            this.canInteract = false;
        }
        private IEnumerator Open()
        {
            float elapsed = 0;
            Quaternion startRotation = door.localRotation;
            Quaternion endRotation = Quaternion.Euler(new Vector3(doorXRotation, 0, 0));
            while (elapsed<doorDuration)
            {
                door.localRotation = Quaternion.Lerp
                    (startRotation, endRotation, elapsed / doorDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            door.localRotation = endRotation;
            ShowMonster();
            LookAtPerform perform = new LookAtPerform();
            perform.ReceiveData(data);
            perform.Perform();
            yield return new WaitForSeconds(data.lookatDuration + 1);

            StartCoroutine(HideMonster(0.3f));
            Transform player = PlayerInstance.Instance.transform;
            PlayerSight playerSight = player.GetComponentInChildren<PlayerSight>();
            elapsed = 0;
            startRotation = player.rotation;
            endRotation = Quaternion.Euler
                (player.eulerAngles+new Vector3(playerXRotation, 0, 0));
            while (elapsed<playerDuration)
            {
                player.rotation = Quaternion.Lerp
                    (startRotation, endRotation, elapsed / playerDuration);
                elapsed += Time.deltaTime;
                playerSight.ApplyRunShake();
                yield return null;
            }
            player.rotation = endRotation;
            GameInputManager.Instance.GetProvider<CharacterInputProvider>()
                .enabled = true;
            GameInputManager.Instance.GetProvider<PlayerSightInputProvider>()
                .enabled = true;
            //切换场景
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = sceneAddress,
                Additive = true,
                SceneToUnload = gameObject.scene.name
            });
            //  PlayerInstance.Instance.GetComponent<PlayerMove>().ForceMoveTo
            //      (targetPosition,pushDuration ,null);    
        }
        private void ShowMonster()
        {
            monster.gameObject.SetActive(true);
            Transform player = PlayerInstance.Instance.transform;
            monster.forward = player.forward;
            monster.position = player.position + (-1 * player.forward * monsterDistance);
        }

        private IEnumerator HideMonster(float delay)
        {
            yield return new WaitForSeconds(delay);
            monster.gameObject.SetActive(false);
        }
    }
}
