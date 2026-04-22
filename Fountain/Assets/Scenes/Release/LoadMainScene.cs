using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//为了打包出来可以运行的临时脚本,不涉及任何的addressable的东西
public class LoadMainScene : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        //StartCoroutine(LoadScene());
    }
    private IEnumerator LoadScene()
    {
        yield return null;
    }
}
