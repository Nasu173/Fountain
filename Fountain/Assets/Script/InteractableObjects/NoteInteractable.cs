using Fountain.Common;
using Fountain.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 笔记可交互物体,挂在笔记上
    /// </summary>
    public class NoteInteractable : MonoBehaviour, IInteractable
    {
        private OutlineVisual[] outlineVisuals;
        [Tooltip("笔记SO")]
        [SerializeField]
        private NoteContent note;
        /*
        [Tooltip("暂停用的东西")]
        [SerializeField]
        private PanelManager pause;
        [Tooltip("暂停用的东西")]
        [SerializeField]
        private Transform pauseUI;
        */
         
        private void Start()
        {
            outlineVisuals = this.GetComponents<OutlineVisual>();
        }
        public void Deselect()
        {
            SetOutline(false);
        }

        public void InteractWith(PlayerInteractor player)
        {
            NotePanel.Instance.ShowNote(note);
            //禁用输入
            GameInputManager.Instance.DisableMoveInput();
            GameInputManager.Instance.DisableSightInput();
            GameInputManager.Instance.DisableInteractInput();
            GameInputManager.Instance.DisablePausePanel();
            //显示鼠标
            GameInputManager.Instance.ShowCursor();
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
