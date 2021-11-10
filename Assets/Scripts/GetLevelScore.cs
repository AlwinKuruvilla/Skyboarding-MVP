using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class GetLevelScore : MonoBehaviour
{
    [SerializeField] private TMP_Text finalScoreValue;
    private void Awake()
    {
        if (finalScoreValue == false)
        {
            Debug.LogError("ERROR: finalScoreValue reference not set on GetLevelScore.cs on " + gameObject.name);
        }
        finalScoreValue.text = ScoreKeeper.LevelScore.ToString("0000");
    }
}
