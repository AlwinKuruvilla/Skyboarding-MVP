using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeper : MonoBehaviour // attached to Score_Value child object of HUD
{
    public static int LevelScore = 0; // starting default score set to 0
    
    public TMP_Text scoreValue; // TMP component of Score_value reference set in the inspector

    // Start is called before the first frame update
    void Start()
    {
        if (scoreValue == null)
        {
            Debug.LogError("scoreValue TextMeshPro reference not defined on ScoreKeeper.cs");
        }
    }

    // Update is called once per frame
    void Update()
    {
        scoreValue.text = LevelScore.ToString("0000");
    }

    public static void IncreaseScore(int points)
    {
        LevelScore += points;
    }

    public static void DecreaseScore(int points)
    {
        LevelScore -= points;
    }
}
