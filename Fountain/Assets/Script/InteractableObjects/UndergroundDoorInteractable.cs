using Fountain.Common;
using Fountain.Dialogue;
using Fountain.InputManagement;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 专门为地下室的门写的可交互脚本,事实上不应该这样写,
    /// 交互系统设计的时候应该考虑多个可交互物体以及执行顺序的问题,
    /// 因为一个可交互物体的效果是可以组合出来的,而不是想这样全写一起,
    /// 此外,还要考虑延迟问题
    /// </summary>
    public class UndergroundDoorInteractable : MonoBehaviour, IInteractable
    {
        [Tooltip("描边效果,手动拖得了")]
        [SerializeField]
        private OutlineVisual outlineVisual;

        [Header("开门的演出的数据")]
        public float doorDuration;//开门总时间
        private Transform door;
        public float doorXRotation;

        [Header("玩家的旋转演出所需数据")]
        [Tooltip("旋转之前的延迟")]
        public float delayBeforeRotate = 1.5f;
        public float playerXRotation;
        public float playerDuration;

        [Header("强制转向所需数据")]
        [SerializeField]
        private Transform lookTarget;
        [SerializeField]
        private float lookDuration;

        public float monsterDistance = 1f;//突脸的时候在玩家身后的距离
        public Transform monster;

        //要切换到的场景
        public string sceneAddress;

        [SerializeField] private AudioClip audioClip;

        //public LookData data;
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

            StartCoroutine(Perform());
            this.canInteract = false;
        }
        private IEnumerator Perform()
        {
            yield return StartCoroutine(RotateDoor());
            ShowMonster();

            // LookAtPerform perform = new LookAtPerform();
            // perform.ReceiveData(data);
            // perform.Perform();
            PlayerMove player = PlayerInstance.Instance.GetComponent<PlayerMove>();
            if (player.IsCrouched())
            {
                player.SwitchCrouch();
                while (player.IsCrouching())
                {
                    yield return null;
                }
            }

            ForcePlayerLook();
            yield return new WaitForSeconds(delayBeforeRotate);

            StartCoroutine(HideMonster(0.3f));

            yield return StartCoroutine(RotatePlayer());

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
        private IEnumerator RotateDoor()
        {
            float elapsed = 0;
            Quaternion startRotation = door.localRotation;
            Quaternion endRotation = Quaternion.Euler(new Vector3(doorXRotation, 0, 0));
            while (elapsed < doorDuration)
            {
                door.localRotation = Quaternion.Lerp
                    (startRotation, endRotation, elapsed / doorDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            door.localRotation = endRotation;
        }
        private IEnumerator RotatePlayer()
        {
            Transform player = PlayerInstance.Instance.transform;
            PlayerSight playerSight = player.GetComponentInChildren<PlayerSight>();
            float elapsed = 0;
            Quaternion startRotation = player.rotation;
            Quaternion endRotation = Quaternion.Euler
                (player.eulerAngles + new Vector3(playerXRotation, 0, 0));
            while (elapsed < playerDuration)
            {
                player.rotation = Quaternion.Lerp
                    (startRotation, endRotation, elapsed / playerDuration);
                elapsed += Time.deltaTime;
                playerSight.ApplyRunShake();
                yield return null;
            }
            player.rotation = endRotation;

        }
        private void ForcePlayerLook()
        {
            PlayerMove player = PlayerInstance.Instance.GetComponent<PlayerMove>();
            player.LookAt(lookTarget.position, lookDuration);
            GameEventBus.Publish(new PlaySoundEvent
            {
                Clip = audioClip,
                Track = AudioTrack.Other
            });
        }


    }
}
