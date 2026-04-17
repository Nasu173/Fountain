using Fountain.Common;
using Fountain.InputManagement;
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
        private void Awake()
        {
            fadeEffect = this.GetComponent<FadeEffect>();
            respawnButton = this.GetComponentInChildren<Button>();
           // respawnButton.onClick.AddListener(ChangeScene);
            background = this.transform.FindChildByName(nameof(background)).
                GetComponent<Image>();
            respawnText= this.transform.FindChildByName(nameof(respawnText)).
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
            //清空任务列表
            GameEventBus.Publish<MenuEvent>(null);

            fadeEffect.duration = e.fadeInTime;
            CursorManager.Instance.SetRespawnPanelEnabled(true);
            fadeEffect.FadeIn();
            this.sceneAddress = e.sceneToLoad;
            respawnText.color = e.textColor;
            background.color = e.backgroundColor;
        }
        public void ChangeScene()
        {
            CursorManager.Instance.SetRespawnPanelEnabled(false);
            GameEventBus.Publish(new LoadSceneEvent
            {
                SceneAddress = this.sceneAddress,
                Additive = true,
                SceneToUnload =this.sceneAddress
            });
        }
    }
}
