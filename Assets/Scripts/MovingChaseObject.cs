using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MovingChaseObject : MonoBehaviour
{
   
    public Transform objectTarget;
    public float turnSpeed = 5f;
    public float speed = 50f;
    public float range = 30f;
    public float slowDownDuration = 3f;
    
    public int pointValue = 200; // set default point value
    public GameObject[] players;
    private Rigidbody _objectRgb;

    [Header("References")]
    
    
    public string enemyTag = "Player";

    void Start()
    {
        _objectRgb = GetComponent<Rigidbody>(); //component reference of the object's rigid body 
    }

    private void FixedUpdate()
    {
        if (! _objectRgb)
            return;

        players = GameObject.FindGameObjectsWithTag(enemyTag); //array that holds found players within range

        float shortestDistance = Mathf.Infinity; //default value set for shortestDistance
        GameObject nearestPlayer = null;         //default value set for nearest player
        
        foreach (GameObject player in players) // runs through the players array to check the distance been each player and the object and update which one is closest to the object
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position); // gets distance between object and player game object
            if (distanceToPlayer < shortestDistance) // checks to see if the distanceToPlayer is less than last recorded shortestDistance
            {
                shortestDistance = distanceToPlayer; // updates the shortestDistance if the distance between the object and the player objects less than the last shortestDistance recorded
                nearestPlayer = player;              // updates the nearestPlayer to the player object
            }
        }

        if (nearestPlayer != null && shortestDistance <= range) //checks to see if the nearestPlayer object is in range then makes object move in the opposite direction of nearestPlayer if it is
        {
           Vector3 direction = nearestPlayer.transform.position -  _objectRgb.position; // direction to the player
           direction.Normalize();
           Vector3 rotationAmount = Vector3.Cross(this.gameObject.transform.forward, direction);
           _objectRgb.angularVelocity = (rotationAmount * turnSpeed);
           _objectRgb.velocity = this.gameObject.transform.forward * -speed; 
        }
        else //stops the object if there is no player in range
        {
            StartCoroutine(SlowDown(_objectRgb, slowDownDuration)); //calls LERP function to slow the object down until it stops
        }
    }
    
    IEnumerator SlowDown(Rigidbody objectToSlow, float duration) //LERP function to slow down an object until it stops
    {
        float time = 0;
        Vector3 startAngularVelocity = objectToSlow.angularVelocity;
        Vector3 startVelocity = objectToSlow.velocity;

        while (time < duration)
        {
            _objectRgb.angularVelocity = Vector3.Lerp(startAngularVelocity, Vector3.zero /* << target vector */, time / duration);
            _objectRgb.velocity = Vector3.Lerp(startVelocity, Vector3.zero /* << target vector */, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        _objectRgb.angularVelocity = Vector3.zero;
        _objectRgb.velocity = Vector3.zero;
    }
    
    private void OnDrawGizmos() // draws wireframe in the editor to show the range of the object
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
