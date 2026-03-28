using Fountain.Dialogue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 用于触发自言自语的效果的东西,不能算作可交互物体,只是顺手放在这个文件夹里罢了
    /// </summary>
    public class TalkToSelf : MonoBehaviour
    {
        public DialogueSequence dialogue;
        /// <summary>
        /// 对话数据
        /// </summary>
        private List<IPerformDataProvider> dataProviders;
        private void Start()
        {
            dataProviders = this.GetComponents<IPerformDataProvider>().ToList();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))//唉,懒得改了
            {
                DialogueManager.Instance.StartDialogue(dialogue, dataProviders);
                this.GetComponent<BoxCollider>().enabled = false;
            }
             
        }
    }
}
