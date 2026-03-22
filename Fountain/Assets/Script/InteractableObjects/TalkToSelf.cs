using Fountain.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkToSelf : MonoBehaviour
{
    public DialogueSequence dialogue;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.Instance.StartDialogue(dialogue, null);
            Destroy(this.gameObject);
        }
         
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
