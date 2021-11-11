using System;
using System.Collections;
using System.Collections.Generic;
using Boids;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tempWindowPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other) // when objects enter trigger...
    {
        if (other.gameObject.GetComponent<Boid>() && other.gameObject.GetComponent<Boid>().CheckCaptureStatus() == true) // ... check to see if object has the MovingChaseObject script attached to it
        {
            int objectPointValue = other.gameObject.GetComponent<Boid>().pointValue; // get pointValue from the MovingChaseObject script
            ScoreKeeper.IncreaseScore(objectPointValue); // call the ScoreKeeper script and increase the LevelScore by the objectPointValue using the IncreaseScore function
            Destroy(other.gameObject); // destroy the MovingChaseObject that entered the trigger
        }

        /* TEMPORARY STATEMENT FOR TESTING/DEMO
            if (ScoreKeeper.LevelScore >= 200)
            {
                Debug.LogWarning("WARNING: using testing statement for win condition in GameManger.cs");
                Instantiate(tempWindowPrefab);
                Time.timeScale = 0;
            }
        */
    }
    
}
