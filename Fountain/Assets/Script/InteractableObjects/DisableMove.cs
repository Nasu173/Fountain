using Fountain.InputManagement;
using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameInputManager.Instance.GetProvider<CharacterInputProvider>().DisableMove(); 
        } 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
