using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetPlayerPosAndRotOnASpawn : MonoBehaviour
{
    public Vector3 spawnPos;
    public Vector3 spawnEuler;
    void Start()
    {
        Transform player = PlayerInstance.Instance.transform;
        player.position = spawnPos;
        player.eulerAngles = spawnEuler; 
    }
}
