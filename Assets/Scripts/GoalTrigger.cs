using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
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
        if (other.gameObject.GetComponent<MovingChaseObject>()) // ... check to see if object has the MovingChaseObject script attached to it
        {
            int objectPointValue = other.gameObject.GetComponent<MovingChaseObject>().pointValue; // get pointValue from the MovingChaseObject script
            ScoreKeeper.IncreaseScore(objectPointValue); // call the ScoreKeeper script and increase the LevelScore by the objectPointValue using the IncreaseScore function
            Destroy(other.gameObject); // destroy the MovingChaseObject that entered the trigger
        }
    }
    
}
