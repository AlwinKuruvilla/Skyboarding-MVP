using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Scene = UnityEngine.SceneManagement.Scene;

public class GameManager : MonoBehaviour
{
    [Header("Level Variables")]
    public float levelTimeLimit = 120f; // set default time time for the level to 2 minutes
    
    public int levelScoreBronze;
    public int levelScoreSliver;
    public int levelScoreGold;

    
    
    [Header("References")]
    public TMP_Text timeValue;

    public SceneAsset currentLevel;
    public SceneAsset nextLevel;
    public SceneAsset menu;

    public GameObject[] players;
    public float[] playerHps;

    private float m_Minutes;
    private float m_Seconds;



    // Start is called before the first frame update
    void Start()
    {
        m_Minutes = Mathf.RoundToInt(levelTimeLimit / 60);     // sets _minutes variable to rounded integer after dividing remaining time by 6o
        m_Seconds = Mathf.RoundToInt(levelTimeLimit % 60);     // sets _seconds variable to rounded integer after getting the remainder of the division of remaining time by 60
        if (players == null)
        {
            int x = 0; // index variable for arrays
            players = GameObject.FindGameObjectsWithTag("Player");
            
            foreach (var player in players)
            {
                playerHps[x] = players[x].GetComponent<HealthBar>().healthSlider.value;
                Debug.Log("Player " + (x+1) + " found. Player " + (x+1) + " HP logged as " + playerHps[x]);
                x++;
            }

            x = 0; //resets index variable
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        TimeKeeper();
        if (levelTimeLimit <= 0 && ScoreKeeper.LevelScore <= levelScoreBronze)
            Lose();

        foreach (var playerHp in playerHps)
        {
            if (playerHp == 0)
                Lose();
        }

        if (levelTimeLimit <= 0 && ScoreKeeper.LevelScore >= levelScoreBronze)
        {
            if (ScoreKeeper.LevelScore >= levelScoreBronze && ScoreKeeper.LevelScore < levelScoreSliver)
            {
                Win(1);  
            }
            else if (ScoreKeeper.LevelScore >= levelScoreSliver && ScoreKeeper.LevelScore < levelScoreGold)
            {
                Win(2);
            }
            else if (ScoreKeeper.LevelScore >= levelScoreGold)
            {
                Win(3);
            }
        }
    }

    public void TimeKeeper () // keeps time for "Time Remaining" in the UI and for the game
    {
        if (m_Seconds <= 0)
        {
            m_Minutes--;
            m_Seconds = 59f;
        }
        
        m_Seconds -= Time.deltaTime;   // decreased remaining time by 1 second
        timeValue.text = m_Minutes.ToString("00") + ":" + Mathf.RoundToInt(m_Seconds).ToString("00");  // displays the time in minutes and seconds to the timeValue TextMeshPro referenced in the Inspector
    }

    public void Win(int scoreTier)
    {
        switch (scoreTier)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    public void Lose()
    {
        
    }
}
