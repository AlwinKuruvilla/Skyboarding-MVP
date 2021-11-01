using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingChaseObjectCollider : MonoBehaviour
{
    public GameObject parentMovingChaseObject;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }

    public void OnTriggerStay(Collider other)
    {
        throw new NotImplementedException();
    }

    public void OnTriggerExit(Collider other)
    {
    }
}
