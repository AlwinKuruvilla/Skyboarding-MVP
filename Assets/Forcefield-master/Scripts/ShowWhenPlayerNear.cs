using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ShowWhenPlayerNear : MonoBehaviour
{
    public int pointValue = 300;

    public Transform objectTarget;
    public float range = 5f;
    public float fadeDuration = 3f;

    static public Material forceFieldMaterialRef;
    public GameObject[] players;

    private int _shaderAlphaMultiplierIndex = forceFieldMaterialRef.shader.FindPropertyIndex("AlphaMultiplier");

    public string enemyTag = "Player";

    private void FixedUpdate()
    {
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

        if (nearestPlayer != null && shortestDistance <= range) // checks to see if the nearestPlayer object is in range then makes the force field visible
        {
            StartCoroutine(FadeIn(fadeDuration));
        }
        else // if no player is in range then make the force field invisible
        {
            StartCoroutine(FadeOut(fadeDuration));
        }
    }
    
    IEnumerator FadeIn(float duration) //LERP function to slow down an object until it stops
    {
        float time = 0;
        float startAlphaMultiplierValue = forceFieldMaterialRef.shader.GetPropertyDefaultFloatValue(_shaderAlphaMultiplierIndex);
        float shaderAlphaMultiplier; 

        while (time < duration)
        {
            shaderAlphaMultiplier = Mathf.Lerp(startAlphaMultiplierValue, 1f /* << target vector */, time / duration);
            Shader.SetGlobalFloat("AlphaMultiplier", shaderAlphaMultiplier);
            time += Time.deltaTime;
            yield return null;
        }

        Shader.SetGlobalFloat("AlphaMultiplier", 1f);
    }
    
    IEnumerator FadeOut(float duration) //LERP function to slow down an object until it stops
    {
        float time = 0;
        float startAlphaMultiplierValue = forceFieldMaterialRef.shader.GetPropertyDefaultFloatValue(_shaderAlphaMultiplierIndex);
        float shaderAlphaMultiplier;

        while (time < duration)
        {
            shaderAlphaMultiplier = Mathf.Lerp(startAlphaMultiplierValue, 1f /* << target vector */, time / duration);
            Shader.SetGlobalFloat("AlphaMultiplier", shaderAlphaMultiplier);
            time += Time.deltaTime;
            yield return null;
        }

        Shader.SetGlobalFloat("AlphaMultiplier", 0f);
    }
}
