using Fountain.Common;
using Fountain.InputManagement;
using Fountain.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fountain.UI
{
    /// <summary>
    /// 笔记UI,负责显示笔记内容
    /// </summary>
    public class NotePanel : MonoBehaviour
    {
        //非常不情愿地做成单例
        public static NotePanel Instance { get; private set; }
        private Button quitButton;
        private TextMeshProUGUI titleText;
        private TextMeshProUGUI noteContent;

        //输入来源
        private PlayerSightInputProvider sightInput;
        private UIInputProvider uiInput;
        //玩家相关脚本
        private PlayerMove playerMove;
        private PlayerSight playerSight;
        private PlayerInteractor playerInteractor;

        //private Action finishCallback;
        /*
        [Tooltip("暂停用")]
        [SerializeField]
        private PanelManager panelManager;
         */
        private void Awake()
        {
            NotePanel.Instance = this;
            titleText = this.transform.FindChildByName(nameof(titleText)).
                GetComponent<TextMeshProUGUI>();
            noteContent = this.transform.FindChildByName(nameof(noteContent)).
                GetComponent<TextMeshProUGUI>();
            quitButton = this.transform.FindChildByName(nameof(quitButton)).
                GetComponent<Button>();
            quitButton.onClick.AddListener(Hide);
            this.gameObject.SetActive(false);
        }
        private void Start()
        {
            uiInput = GameInputManager.Instance.GetProvider<UIInputProvider>();
            sightInput = GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();

            playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            playerInteractor = PlayerInstance.Instance.GetComponent<PlayerInteractor>();
            playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>(); 
        }


        public void ShowNote(NoteContent content)
        {
            //finishCallback = finishReading;
            if (content==null)
            {
                return;
            }
            this.gameObject.SetActive(true);

            this.titleText.text = content.GetTitle();
            this.noteContent.text = content.GetText();
            
        }
        /// <summary>
        /// 隐藏笔记内容
        /// </summary>
        private void Hide()
        {
            this.gameObject.SetActive(false);
            //其实最好不写在这里
            //启用输入
            playerInteractor.Enable();
            playerMove.enabled = true;
            playerSight.enabled = true;
            uiInput.enabled = true;
            // GameInputManager.Instance.EnableMoveInput();
            // GameInputManager.Instance.EnableSightInput();
            // GameInputManager.Instance.EnableInteractInput();
            // GameInputManager.Instance.EnablePausePanel();
            //隐藏鼠标
            sightInput.HideCursor();
            // GameInputManager.Instance.HideCursor();
           // finishCallback?.Invoke();
           // finishCallback = null;
            //GameEventBus.Publish<NoteFinishReadEvent>(null);
        }
    }
}
