using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SearchService;
#endif
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;


public class GameManager : MonoBehaviour
{
    [Header("Level Variables")] public float levelTimeLimit = 120f; // set default time time for the level to 2 minutes

    public int levelScoreBronze;
    public int levelScoreSliver;
    public int levelScoreGold;

    private int _storedScoreTier;
  
    //public GameObject winWindowPrefab; // used for testing purposes
    private GameObject winWindowInstance;
    // flags used for determining which tier indicators are visible in the HUD
    private bool m_BrozneTierIndicatorOn = false;
    private bool m_SliverTierIndicatorOn = false;
    private bool m_GoldTierIndicatorOn = false;
    private bool m_AllTierIndicatorsOn = false;

    [Header("Time Reference")]
    [Tooltip("reference variable for time")]
    public TMP_Text timeValue;
    [NonSerialized] public TMP_Text finalLevelScore;
    [NonSerialized] public TMP_Text minimumLevelScore;
    
    [Tooltip("reference variables for scene assets")]
    public SceneAsset currentLevel;
    public SceneAsset nextLevel;
    public SceneAsset menu;
    
    [Header("HUD Tier Indicator References")]
    [Tooltip("reference variables for tier indicators in the HUD; set in the Inspector")]
    public GameObject bronzeTierHUDSpriteRef;
    public GameObject sliverTierHUDSpriteRef;
    public GameObject goldTierHUDSpriteRef;

    [Header("Win Message Prefab References")]
    [Tooltip("reference variables for tier win message prefabs; set in the Inspector")]
    public GameObject bronzeTierMessage;
    public GameObject sliverTierMessage;
    public GameObject goldTierMessage;
    public GameObject loseMessage;
    
    private GameObject m_WinMessageInstance;
    private GameObject m_LoseMessageInstance;// variable for win message instanced object
    public GameObject[] players;
    public float[] playerHps;

    private float m_Minutes;
    private float m_Seconds;
    
    public bool levelOver = false;



    // Start is called before the first frame update
    void Start()
    {
        m_Minutes = Mathf.RoundToInt(levelTimeLimit / 60);     // sets _minutes variable to rounded integer after dividing remaining time by 6o
        m_Seconds = Mathf.RoundToInt(levelTimeLimit % 60);     // sets _seconds variable to rounded integer after getting the remainder of the division of remaining time by 60

        if (players == null)
        {
            int x = 0; // index variable for arrays
            players = GameObject.FindGameObjectsWithTag("Player"); // find all gameobjects with "Player" tag and put them in the players array
            
            foreach (var player in players) // get the health bar for each player and put them in the playerHps array (health bar gameobject will need "Player" tag to be found by the line above)
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
        int x = players.Length; // index number
        
        TimeKeeper();
        
        if (levelTimeLimit <= 0 && (ScoreKeeper.LevelScore + players[0].GetComponent<HealthBar>().healthSlider.value) <= levelScoreBronze && levelOver == false) // only adds first player's remaining HP to score currently
        {
            levelOver = true;
            Lose(levelScoreBronze);
        }
        
        foreach (var playerHp in playerHps)
        {
            playerHps[x] = players[x].GetComponent<HealthBar>().healthSlider.value;
            if (playerHp == 0 && levelOver == false)
            {
                levelOver = true;
                Lose(levelScoreBronze);
            }
            x--;
        }

        if (m_AllTierIndicatorsOn == false) CheckTierHUDIndicators();

        CheckForTimeLimitExpire();
    }

    public void TimeKeeper () // keeps time for "Time Remaining" in the UI and for the game
    {
        if (m_Seconds <= 0)
        {
            m_Minutes--;
            m_Seconds = 59f;
        }
        
        m_Seconds -= Time.deltaTime;   // decreased remaining visual time by 1 second
        timeValue.text = m_Minutes.ToString("00") + ":" + Mathf.RoundToInt(m_Seconds).ToString("00");  // displays the time in minutes and seconds to the timeValue TextMeshPro referenced in the Inspector
        levelTimeLimit -= Time.deltaTime; // decreased time limit value by 1 second
        if (levelTimeLimit <= 0) levelTimeLimit = 0;
    }

    public void Win(int scoreTier)
    {
        Time.timeScale = 0f;
        _storedScoreTier = scoreTier;
        switch (scoreTier)
        {
            case 1:
                m_WinMessageInstance = Instantiate(bronzeTierMessage);
                finalLevelScore = m_WinMessageInstance.GetComponent<TextMeshProUGUI>();
                finalLevelScore.text = ScoreKeeper.LevelScore.ToString("0000");
                break;
            case 2:
                m_WinMessageInstance = Instantiate(sliverTierMessage);
                finalLevelScore = m_WinMessageInstance.GetComponent<TextMeshProUGUI>();
                finalLevelScore.text = ScoreKeeper.LevelScore.ToString("0000");
                break;
            case 3:
                m_WinMessageInstance = Instantiate(goldTierMessage);
                finalLevelScore = m_WinMessageInstance.GetComponent<TextMeshProUGUI>();
                finalLevelScore.text = ScoreKeeper.LevelScore.ToString("0000");
                break;
        }
    }

    public void Lose(int minimumScoreNeeded)
    {
        Time.timeScale = 0f;
        m_LoseMessageInstance = Instantiate(loseMessage);
        minimumLevelScore = m_LoseMessageInstance.GetComponent<TextMeshProUGUI>();
        minimumLevelScore.text = minimumScoreNeeded.ToString("0000");
    }

    private void CheckTierHUDIndicators()
    {
        if (ScoreKeeper.LevelScore >= levelScoreBronze && m_BrozneTierIndicatorOn == false)
        {
            Debug.Log("Bronze tier achieved");
            bronzeTierHUDSpriteRef.transform.localScale = Vector3.one;
            m_BrozneTierIndicatorOn = true;
        }
        
        if (ScoreKeeper.LevelScore >= levelScoreSliver && m_SliverTierIndicatorOn == false)
        {
            Debug.Log("Sliver tier achieved");
            sliverTierHUDSpriteRef.transform.localScale = Vector3.one;
            m_SliverTierIndicatorOn = true;
        }
        
        if (ScoreKeeper.LevelScore >= levelScoreSliver && m_GoldTierIndicatorOn == false)
        {
            Debug.Log("Gold tier achieved");
            goldTierHUDSpriteRef.transform.localScale = Vector3.one;
            m_GoldTierIndicatorOn = true;
        }

        if (m_BrozneTierIndicatorOn && m_SliverTierIndicatorOn && m_GoldTierIndicatorOn)
        {
            m_AllTierIndicatorsOn = true;
        }
    }
    
    private void CheckForTimeLimitExpire()
    {
        if (levelTimeLimit <= 0f && levelOver == false)
        {
            if (ScoreKeeper.LevelScore < levelScoreBronze)
            {
                Lose(levelScoreBronze);
            }
            if (ScoreKeeper.LevelScore >= levelScoreBronze && ScoreKeeper.LevelScore < levelScoreSliver)
            {
                Debug.Log("Win condition 1 fulfilled");
                levelOver = true;
                Win(1);
            }
            else if (ScoreKeeper.LevelScore >= levelScoreSliver && ScoreKeeper.LevelScore < levelScoreGold)
            {
                Debug.Log("Win condition 2 fulfilled");
                levelOver = true;
                Win(2);
            }
            else if (ScoreKeeper.LevelScore >= levelScoreGold)
            {
                Debug.Log("Win condition 3 fulfilled");
                levelOver = true;
                Win(3);
            }
        }
    }
    
}