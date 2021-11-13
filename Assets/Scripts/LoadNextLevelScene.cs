using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevelScene : MonoBehaviour
{
    public void LoadNext()
    {
        string nextLevel = GameManager.StaticNextLevel;
        SceneManager.LoadScene(nextLevel);
    }
}
