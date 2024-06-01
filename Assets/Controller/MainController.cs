using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    public void runStart()
    {
        SceneManager.LoadScene("BlockGame", LoadSceneMode.Single);
    }
    public void exit()
    {
        UnityEditor.EditorApplication.isPlaying = false;

        Application.Quit();
    }
}
