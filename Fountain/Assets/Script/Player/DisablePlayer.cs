using Fountain.InputManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fountain.Player
{
    /// <summary>
    /// 禁用玩家的脚本,十分不愿意写这个
    /// </summary>
    public class DisablePlayer : MonoBehaviour
    {
        //由于玩家是常驻在一个场景里的,有些场景不应该让玩家动

        private void Start()
        {
            GameInputManager.Instance.GetProvider<PlayerSightInputProvider>().enabled = false;
            GameInputManager.Instance.GetProvider<CharacterInputProvider>().enabled = false;
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                GameInputManager.Instance.GetProvider<PlayerSightInputProvider>().
                enabled = true;
                GameInputManager.Instance.GetProvider<CharacterInputProvider>().
                enabled = true;
            };
        }
    }
}
