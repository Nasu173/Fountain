using Fountain.Common;
using Fountain.Dialogue;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 能触发对话的可交互物体
    /// </summary>
    public class DialogueInteractable : MonoBehaviour,IInteractable
    {
        [Tooltip("描边效果,手动拖得了")]
        [SerializeField]
        private OutlineVisual[] outlineVisuals;

        [Tooltip("对话SO")]
        [SerializeField]
        private DialogueSequence dialogues;
        /// <summary>
        /// 对话数据
        /// </summary>
        private List<IPerformDataProvider> dataProviders;

        private void Start()
        {
            dataProviders = this.GetComponents<IPerformDataProvider>().ToList();
        }
        public void Deselect()
        {
            SetOutline(false);
        }

        public void InteractWith(PlayerInteractor player)
        {
            DialogueManager.Instance.StartDialogue(this.dialogues,dataProviders);                
        }

        public void Select()
        {
            SetOutline(true);
        }

        private void SetOutline(bool visible)
        {
            foreach (var item in outlineVisuals)
            {
                item.SetOutline(visible);
            }
        }

    }
}
