using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Dialogue
{
    public class SwitchDialogue : MonoBehaviour
    {
        public DialogueInteractable interactable;
        /// <summary>
        /// 这两个的下标一一对应
        /// </summary>
        public string[] taskID;
        public DialogueSequence[] dialogues;
        private void OnEnable()
        {
            GameEventBus.Subscribe<TaskStartEvent>(Switch); 
        }
        private void OnDisable()
        {
            GameEventBus.Unsubscribe<TaskStartEvent>(Switch); 
        }
        private void Switch(TaskStartEvent e)
        {
            int i = 0;
            for ( i = 0; i < taskID.Length; i++)
            {
                if (taskID[i]==e.TaskId)
                {
                    interactable.dialogues = this.dialogues[i];
                    break;
                }
            }
            
        }
    }
}
