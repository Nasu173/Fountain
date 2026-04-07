using Fountain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fountain.InputManagement;

public class ResetPlayerPosAndRotOnASpawn : MonoBehaviour
{
    public Vector3 spawnPos;
    public Vector3 spawnEuler;
    public float enableInputDelay=1f;
    void Start()
    {
        //在一个场景开始的时候指定玩家的位置,旋转
        StartCoroutine(ResetPlayer());
    }
    private IEnumerator ResetPlayer()
    {
        GameInputManager.Instance.GetProvider<CharacterInputProvider>()
            .enabled = false;
        GameInputManager.Instance.GetProvider<PlayerSightInputProvider>()
            .enabled = false;
        Transform player = PlayerInstance.Instance.transform;
        player.position = spawnPos;
        player.eulerAngles = spawnEuler;
        yield return new WaitForSeconds(enableInputDelay);
        GameInputManager.Instance.GetProvider<CharacterInputProvider>()
            .enabled = true;
        GameInputManager.Instance.GetProvider<PlayerSightInputProvider>()
            .enabled = true;

    }


}
