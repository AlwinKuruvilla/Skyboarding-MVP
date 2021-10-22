using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scene/Level Variables")]
    public static float LevelTimeLimit = 120f; // set default time time for the level to 2 minutes

    [Header("References")]
    public TMP_Text timeValue;

    private float _minutes;
    private float _seconds;
    
    

    // Start is called before the first frame update
    void Start()
    {
        _minutes = Mathf.RoundToInt(LevelTimeLimit / 60);     // sets _minutes variable to rounded integer after dividing remaining time by 6o
        _seconds = Mathf.RoundToInt(LevelTimeLimit % 60);     // sets _seconds variable to rounded integer after getting the remainder of the division of remaining time by 60
    }

    // Update is called once per frame
    void Update()
    {
        if (_seconds <= 0)
        {
            _minutes--;
            _seconds = 59f;
        }
        
        _seconds -= Time.deltaTime;                       // decreased remaining time by 1 second
        timeValue.text = _minutes.ToString("00") + ":" + Mathf.RoundToInt(_seconds).ToString("00");             // displays the time in minutes and seconds to the timeValue TextMeshPro referenced in the Inspector
    }
}
