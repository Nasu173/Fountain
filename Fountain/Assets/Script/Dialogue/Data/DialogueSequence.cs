using Fountain.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Fountain.Dialogue
{
    /// <summary>
    /// 对话序列SO,存储完整的对话
    /// </summary>
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Sequence")]
    public class DialogueSequence :ScriptableObject 
    {
        /// <summary>
        /// 对话节点,存储“一行”对话,对话系统里的基本数据
        /// </summary>
        [Serializable]
        public class DialogueNode
        {
            [Header("对话文本")]
            [Header("英文文本")]
            [Tooltip("说话人名字(英文)")]
            public string speakerEN;
            [Tooltip("对话内容(英文)")]
            [TextArea(3,6)]
            public string textEN;

            [Header("中文文本")]
            [Tooltip("说话人名字(中文)")]
            public string speakerZH;
            [Tooltip("对话内容(中文)")]
            [TextArea(3,5)]
            public string textZH;

            [Header("演出相关")]
            [Tooltip("演出效果实现类的名称")]
            public string performName;
            public string GetSpeaker()
            {
                switch (LocalizationManager.Instance.GetLocale())
                {
                    case LocalizationManager.LocaleID.zh:
                        return speakerZH;
                        //break;
                    case LocalizationManager.LocaleID.en:
                        return speakerEN;
                        //break;
                    default:
                        return string.Empty;
                }
            }
            public string GetText()
            {
                switch (LocalizationManager.Instance.GetLocale())
                {
                    case LocalizationManager.LocaleID.zh:
                        return textZH;
                        //break;
                    case LocalizationManager.LocaleID.en:
                        return textEN;
                        //break;
                    default:
                        return string.Empty;
                }
            }


        }

        [Tooltip("对话唯一的数字id,仅用于标识,数字之间没有任何关联")]
        public int id;
        [Tooltip("所有的对话节点,按顺序放好")]
        public DialogueNode[] sequence;
        [Tooltip("对话结束后的演出设置")]
        public string dialogueEndPerformName;
        public bool unlockInput=true;
        //可选:添加对话开始时的事件和对话结束时的事件,
        //对话不一定于策划案的划分一致,有些对话可能要拆成多个对话
    }
}
