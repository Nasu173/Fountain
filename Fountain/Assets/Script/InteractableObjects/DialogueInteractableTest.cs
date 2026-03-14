using Foutain.Common;
using Foutain.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Foutain.Dialogue
{
    /// <summary>
    /// 测试对话系统的测试脚本,用完记得删除
    /// </summary>
    public class DialogueInteractableTest : MonoBehaviour, IInteractable
    {
        public DialogueSequence dialogues;
        private OutlineVisual[] outlineVisuals;
        private List<IPerformDataProvider> dataProviders;
        private void Start()
        {
            outlineVisuals = this.GetComponents<OutlineVisual>();
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
