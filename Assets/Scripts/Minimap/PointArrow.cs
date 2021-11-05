using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointArrow : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemies;
    
    private Transform _targetEnemyPos = null;

    private void Start()
    {
        _enemies = GameObject.FindGameObjectsWithTag("EnemyObject");
    }

    private void LateUpdate()
    {
        _targetEnemyPos = GetClosestEnemy();

        Vector3 dirToEnemy = transform.position - _targetEnemyPos.position;
        transform.rotation = Quaternion.LookRotation(dirToEnemy);
    }
    
    Transform GetClosestEnemy()
    { 
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        
        foreach(GameObject potentialTarget in _enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }
        return bestTarget;
    }
}
