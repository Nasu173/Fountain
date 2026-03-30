using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fountain.Player
{
    /// <summary>
    /// 根据任务ID决定当前的可交互物体能否交互
    /// </summary>
    public class ControlInteractable : MonoBehaviour
    {
        public IInteractable[] interactables;
        [Tooltip("启用交互的任务id")]
        [SerializeField]
        private string[] taskID;
        private void Start()
        {
            interactables = this.GetComponentsInChildren<IInteractable>();
        }
        private void EnableInteraction(TaskStartEvent e)
        {
            for (int i = 0; i < taskID.Length; i++)
            {
                if (e.TaskId == taskID[i])
                {
                    foreach (var item in interactables)
                    {
                        //这个好像依赖于可交互物体交互一次后就禁止交互
                        item.CanInteract = true;
                    }
                    return;
                }
            }
            //如果当前的任务id不启用交互,就禁止交互
            foreach (var item in interactables) 
            {
                //这个好像依赖于可交互物体交互一次后就禁止交互
                item.CanInteract = false;
            }

        }

        void OnEnable()
        {
            GameEventBus.Subscribe<TaskStartEvent>(EnableInteraction);
        }

        void OnDisable()
        {
            GameEventBus.Unsubscribe<TaskStartEvent>(EnableInteraction);
        }

    }
}
