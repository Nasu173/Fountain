using Foutain.Common;
using Foutain.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Foutain.UI
{
    /// <summary>
    /// 对话窗口,仅负责显示对话文本,虽然不好,但出于某些原因,做成单例
    /// </summary>
    public class DialoguePanel : MonoBehaviour
    {
        //非常不想把这个做成单例
        public static DialoguePanel Instance { get; private set; }
        //下面这三个变量的名称和场景里的物体名称一致,要一致修改
        private TextMeshProUGUI speakerText;//说话对象名称的文本
        private TextMeshProUGUI dialogueText;//对话内容显示文本
        private Transform skipPrompt;//跳过的文本提示
        /// <summary>
        /// 打字机效果
        /// </summary>
        private TypeEffect typeEffect;

        [Tooltip("打字效果的速度,字符/秒")]
        [SerializeField]
        private float typeSpeed;

        private void Awake()
        {
            DialoguePanel.Instance = this;
            this.SetVisible(false);
            speakerText = this.transform.FindChildByName(nameof(speakerText)).
                GetComponent<TextMeshProUGUI>();
            dialogueText = this.transform.FindChildByName(nameof(dialogueText)).
                GetComponent<TextMeshProUGUI>();
            skipPrompt = this.transform.FindChildByName(nameof(skipPrompt));
            typeEffect = new TypeEffect(dialogueText, typeSpeed);
            typeEffect.TypeCompleted += this.ShowPrompt;
        }

        /// <summary>
        /// 设置对话面板的显隐
        /// </summary>
        /// <param name="visible"></param>
        public void SetVisible(bool visible)
        {
            this.gameObject.SetActive(visible);
        }
        /// <summary>
        /// 开始显示对话
        /// </summary>
        /// <param name="dialogueNode">对话节点</param>
        public void ShowDialogue(DialogueSequence.DialogueNode dialogueNode)
        {
            if (dialogueNode==null)
            {
                return;
            }
            HidePrompt();
            this.ShowSpeaker(dialogueNode);
            typeEffect.ShowText(dialogueNode.GetText());
        }
        /// <summary>
        /// 跳过文本的显示,直接显示所有文本
        /// </summary>
        public void Skip()
        {
            typeEffect.SkipEffect();
        }
        /// <summary>
        /// 文字是否在显示中
        /// </summary>
        /// <returns></returns>
        public bool IsShowing()
        {
            return typeEffect.IsTyping();
        }

        /// <summary>
        /// 显示继续对话的提示
        /// </summary>
        private void ShowPrompt()
        {
            skipPrompt.gameObject.SetActive(true);
        }
        /// <summary>
        /// 隐藏继续对话的提示
        /// </summary>
        private void HidePrompt()
        {
            skipPrompt.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示说话人
        /// </summary>
        /// <param name="dialogueNode">对话节点</param>
        private void ShowSpeaker(DialogueSequence.DialogueNode dialogueNode)
        {
            speakerText.text = dialogueNode.GetSpeaker();    
        }
    }
}
