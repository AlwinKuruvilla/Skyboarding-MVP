using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] public SceneAsset targetScene;
    
    public void Change()
    {
        string sceneName = targetScene.name;
        SceneManager.LoadScene(sceneName); 
    }
}
