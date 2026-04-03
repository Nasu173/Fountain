using Fountain.Dialogue;
using Fountain.MiniGame.RepairWaterValve;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerDialogueAfterValve : MonoBehaviour
{
    //private WaterValveController valveController;
    [SerializeField]
    private DialogueSequence dialogue;
    private void Awake()
    {
       // valveController = this.GetComponent<WaterValveController>();
       // valveController.RepairComplete += TriggerDialogue;
    }
    private void OnEnable()
    {
        GameEventBus.Subscribe<ValveFixedEvent>(TriggerDialogue);
    }
    private void OnDisable()
    {
        GameEventBus.Unsubscribe<ValveFixedEvent>(TriggerDialogue);
    }
    private void TriggerDialogue(ValveFixedEvent e)
    {
        List<IPerformDataProvider> datas = this.GetComponents<IPerformDataProvider>().ToList();
        DialogueManager.Instance.StartDialogue(dialogue, datas);            
    }
}
