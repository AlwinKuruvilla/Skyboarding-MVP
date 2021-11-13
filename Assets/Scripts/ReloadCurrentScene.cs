using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadCurrentScene : MonoBehaviour
{
    public void ReloadCurrent()
    {
        if (Time.timeScale <= 0)
        {
            Time.timeScale = 1;
        }
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }
}
