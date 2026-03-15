using Fountain.Common;
using Fountain.Player;
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


        /// <summary>
        /// 显示笔记内容
        /// </summary>
        /// <param name="content"></param>
        public void ShowNote(NoteContent content)
        {
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
            GameInputManager.Instance.EnableMoveInput();
            GameInputManager.Instance.EnableSightInput();
            GameInputManager.Instance.EnableInteractInput();
            GameInputManager.Instance.EnablePausePanel();
            //显示鼠标
            GameInputManager.Instance.HideCursor();

        }
    }
}
