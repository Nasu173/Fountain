using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPlayerPosAndRotOnASpawn : MonoBehaviour
{
    public Vector3 spawnPos;
    public Vector3 spawnEuler;
    // Start is called before the first frame update
    void Start()
    {
        Transform player = PlayerInstance.Instance.transform;
        player.position = spawnPos;
        player.eulerAngles = spawnEuler; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
