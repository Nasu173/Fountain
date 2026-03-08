using Foutain.Player;
using Foutain.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Foutain.Dialogue
{
    /// <summary>
    /// 对话管理器(单例),提供开启对话、继续对话的接口,并调用对话中的演出事件
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private DialoguePanel dialoguePanel;
        /// <summary>
        /// 当前进行的对话的SO
        /// </summary>
        private DialogueSequence currentDialogues;
        /// <summary>
        /// 对话是否结束
        /// </summary>
        private bool dialogueCompleted=true;
        /// <summary>
        /// 当前对话的索引
        /// </summary>
        private int dialogueIndex;

        /// <summary>
        /// 演出数据
        /// </summary>
        private List<IPerformDataProvider> performDatas;
        private DialoguePerformFactory performFactory; 
        
        private void Awake()
        {
            DialogueManager.Instance = this;
        }
        private void Start()
        {
            dialoguePanel = DialoguePanel.Instance;
            performFactory = new DialoguePerformFactory();
        }

        /// <summary>
        /// 开始对话 
        /// </summary>
        /// <param name="dialogues">对话数据的SO</param>
        /// <param name="dataProviders">提供演出数据的对象</param>
        public void StartDialogue(DialogueSequence dialogues, List<IPerformDataProvider> dataProviders)
        {
            if (!dialogueCompleted)
            {
                return;
            }
            //记录对话数据
            dialogueIndex = -1;
            dialogueCompleted = false;
            this.currentDialogues = dialogues;
            this.performDatas = dataProviders;

            //禁用输入,注意对话时不允许打开设置面板,会有bug,要修有点麻烦
            GameInputManager.Instance.DisableInteractInput();
            GameInputManager.Instance.DisableMoveInput();
            GameInputManager.Instance.DisableSightInput();
            GameInputManager.Instance.DisablePausePanel();

            //UI显示
            dialoguePanel.SetVisible(true);
            GameEventBus.Publish<DialogueBeginEvent>(null);
            ContinueDialogue();
        }
        /// <summary>
        /// 继续当前已经开始的对话
        /// </summary>
        public void ContinueDialogue()
        {
            //对话在显示中则跳过显示
            if (dialoguePanel.IsShowing())
            {
                dialoguePanel.Skip();
                return;
            }

            //否则到下一句对话
            dialogueIndex++;
            //没有对话了就结束
            if (dialogueIndex>=currentDialogues.sequence.Length)
            {
                EndDialogue();
                return;
            }
            
            DialogueSequence.DialogueNode dialogueNode =
                currentDialogues.sequence[dialogueIndex];
            dialoguePanel.ShowDialogue(dialogueNode);
            TriggerPerform(dialogueIndex,dialogueNode.performName);
        }
        private void EndDialogue()
        {
            dialogueCompleted = true;
            this.currentDialogues = null;
            dialoguePanel.SetVisible(false);
            this.performDatas = null;
            GameEventBus.Publish<DialogueEndEvent>(null);
            
            //恢复输入
            GameInputManager.Instance.EnableInteractInput();
            GameInputManager.Instance.EnableMoveInput();
            GameInputManager.Instance.EnableSightInput();
            GameInputManager.Instance.EnablePausePanel();
        }
        /// <summary>
        /// 实现演出效果
        /// </summary>
        /// <param name="dialogueIndex">这个对话在序列里的索引</param>
        /// <param name="performName">演出效果实现类名称</param>
        private void TriggerPerform(int dialogueIndex,string performName)
        {
            if (this.performDatas==null||string.IsNullOrEmpty(performName))
            {
                return;
            }

            //找到对话演出要用到的数据
            IPerformDataProvider data = performDatas.Find
                ((data) => { return data.GetTargetIndex() == dialogueIndex; });
            if (data==null)
            {
                return;
            }
            DialoguePerform perform = performFactory.GetPerform(performName);
            if (perform!=null)
            {
                perform.ReceiveData(data);
                perform.Perform();
            }
        }
    }
}
