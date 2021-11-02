using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMap : MonoBehaviour
{
    public float MinimapSize;
    
    [SerializeField] private GameObject _enemyIndicator;
    [SerializeField] private Transform[] _enemies;

    private Transform _targetEnemyPos = null;

    private void LateUpdate()
    {
        _targetEnemyPos = GetClosestEnemy();

        _enemyIndicator.transform.localPosition = Vector3.Scale(transform.InverseTransformPointUnscaled(_targetEnemyPos.position), new Vector3(1,1,0)); 
        
        // change or flip if upsidedown
        // fix code here to work in more scenarios
        
        _enemyIndicator.transform.localPosition = new Vector3 (
            Mathf.Clamp(_enemyIndicator.transform.localPosition.x, -MinimapSize, MinimapSize),
            Mathf.Clamp(_enemyIndicator.transform.localPosition.y, -MinimapSize, MinimapSize),
            _enemyIndicator.transform.localPosition.z
        );

        Vector3 dirToEnemy = _enemyIndicator.transform.position - transform.position;
        
        _enemyIndicator.transform.rotation = Quaternion.LookRotation(dirToEnemy, -transform.forward);
        
        Debug.DrawLine(_enemyIndicator.transform.position, transform.position, Color.blue);
    }

    // copied from: https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/
    Transform GetClosestEnemy()
    { ;
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        
        foreach(Transform potentialTarget in _enemies)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        return bestTarget;
    }
}
