using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        LoadingSceneManager.LoadScene(sceneName);
    }

    public void NextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
