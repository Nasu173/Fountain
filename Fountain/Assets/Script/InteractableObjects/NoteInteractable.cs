using Fountain.Common;
using Fountain.InputManagement;
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
        [Tooltip("描边效果,手动拖得了")]
        [SerializeField]
        private OutlineVisual outlineVisual;
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
         
        //输入来源
        private PlayerSightInputProvider sightInput;
        private UIInputProvider uiInput;
        //玩家相关脚本
        private PlayerMove playerMove;
        private PlayerSight playerSight;
        private PlayerInteractor playerInteractor;

        private bool canInteract=false;
        public bool CanInteract 
        { get { return canInteract; } set { canInteract = value; } } 
        private void Start()
        {
            //outlineVisual = this.GetComponent<OutlineVisual>();
            uiInput = GameInputManager.Instance.GetProvider<UIInputProvider>();
            sightInput = GameInputManager.Instance.GetProvider<PlayerSightInputProvider>();

            playerMove = PlayerInstance.Instance.GetComponent<PlayerMove>();
            playerInteractor = PlayerInstance.Instance.GetComponent<PlayerInteractor>();
            playerSight = PlayerInstance.Instance.GetComponentInChildren<PlayerSight>(); 
        }
        public void Deselect()
        {
            outlineVisual.SetOutline(false);
        }

        public void InteractWith(PlayerInteractor player)
        {
            NotePanel.Instance.ShowNote(note);
            //禁用输入
            playerInteractor.Disable();
            playerMove.enabled = false;
            playerSight.enabled = false;
            uiInput.enabled = false;
           // GameInputManager.Instance.DisableMoveInput();
           // GameInputManager.Instance.DisableSightInput();
           // GameInputManager.Instance.DisableInteractInput();
           // GameInputManager.Instance.DisablePausePanel();
            //显示鼠标
            sightInput.ShowCursor();
            //GameInputManager.Instance.ShowCursor();
            //this.canInteract = false;
        }

        public void Select()
        {
            outlineVisual.SetOutline(true);
        }
    }
}
