using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Fountain.InputManagement
{
    public class DialogueInputProvider:MonoBehaviour,IInputProvider
    {
        private PlayerInputActions inputActions;
        private void Awake()
        {
            this.inputActions = GameInputManager.Instance.GetInputAction();
        }
        private void OnEnable()
        {
            inputActions.UI.ContinueDialogue.Enable();    
        }
        private void OnDisable()
        {
            inputActions.UI.ContinueDialogue.Disable();    
        }
        /// <summary>
        /// 对话键是否按下
        /// </summary>
        /// <returns></returns>
        public bool GetDialogueContinue()
        {
            return inputActions.UI.ContinueDialogue.WasPressedThisFrame();
        }
    }
}
