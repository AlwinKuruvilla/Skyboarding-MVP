using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetMinimumScore : MonoBehaviour
{
    [SerializeField] private TMP_Text minimumLevelScoreValue;
    private void Awake()
    {
        if (minimumLevelScoreValue == false)
        {
            Debug.LogError("ERROR: minimumLevelScoreValue reference not set on GetLevelScore.cs on " + gameObject.name);
        }
        minimumLevelScoreValue.text = GameManager.StaticBronzeScore.ToString("0000");
    }
}
