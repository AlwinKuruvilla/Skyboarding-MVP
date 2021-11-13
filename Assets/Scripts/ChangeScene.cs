using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] public String targetSceneName;
    
    public void Change()
    {
        SceneManager.LoadScene(targetSceneName); 
    }
}
