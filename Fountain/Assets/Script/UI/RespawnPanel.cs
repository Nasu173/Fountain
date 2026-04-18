using Fountain.Common;
using Fountain.InputManagement;
using Fountain.Player;
using Foutain.Scene;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.UI
{
    public class RespawnPanel : MonoBehaviour
    {
        private FadeEffect fadeEffect;
        private Button respawnButton;
        private Image background;
        private TextMeshProUGUI respawnText;
        private string sceneAddress;
        private string sceneToUnload;
        private bool _isActive = false;
        private void Awake()
        {
            fadeEffect = this.GetComponent<FadeEffect>();
            respawnButton = this.GetComponentInChildren<Button>();
            background = this.transform.FindChildByName(nameof(background)).
                GetComponent<Image>();
            respawnText = this.transform.FindChildByName(nameof(respawnText)).
                GetComponent<TextMeshProUGUI>();
        }
        private void OnEnable()
        {
            GameEventBus.Subscribe<RespawnEvent>(FadeIn);
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<RespawnEvent>(FadeIn);
        }
        private void FadeIn(RespawnEvent e)
        {
            if (_isActive) return;
            _isActive = true;

            //禁用玩家输入
            var playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            var playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>();
            if (playerMove != null) playerMove.enabled = false;
            if (playerSight != null) playerSight.enabled = false;
            GameInputManager.Instance.GetProvider<PauseInputProvider>().enabled = false;

            //清空任务列表
            GameEventBus.Publish<MenuEvent>(null);

            fadeEffect.duration = e.fadeInTime;
            CursorManager.Instance.SetRespawnPanelEnabled(true);
            fadeEffect.FadeIn();
            this.sceneAddress = e.sceneToLoad;
            this.sceneToUnload = e.sceneToUnload;
            respawnText.color = e.textColor;
            background.color = e.backgroundColor;

            StartCoroutine(Move());
        }
        public void ChangeScene()
        {
            _isActive = false;
            CursorManager.Instance.SetRespawnPanelEnabled(false);
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = this.sceneAddress,
                Additive = true,
                SceneToUnload = this.sceneToUnload
            });
            fadeEffect.FadeOut();

            //启用玩家输入
            CharacterInputProvider moveInput = GameInputManager.Instance.
            GetProvider<CharacterInputProvider>();
            moveInput.enabled = true;
            PlayerSightInputProvider sightInput = GameInputManager.Instance.GetProvider
                <PlayerSightInputProvider>();
            PlayerMove player = PlayerInstance.Instance.GetComponent<PlayerMove>();
            sightInput.enabled = true;
            player.enabled = true;
            PlayerSight playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>();
            if (playerSight != null) playerSight.enabled = true;
            respawnButton.onClick.RemoveListener(ChangeScene);//移了一下才能复活

        }

        private IEnumerator Move()
        {
            yield return new WaitForSeconds(1f);
            PlayerInstance.Instance.transform.position = Vector3.zero;
            respawnButton.onClick.AddListener(ChangeScene);//移了一下才能复活
        }
    }
}
