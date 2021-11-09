using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.XR.Interaction.Toolkit;
#endif
using UnityEngine.InputSystem;

public class TestingMovement : MonoBehaviour
{
    public InputActionProperty inputAction; // input action reference to be filled in the inspector
    public Transform targetTransform;       // the transform of where you want the XR rig to move towards to be filled in the inspector
    public Transform xrRigTransform;        //  transform of the XR rig itself to be filled in the inspector
    public float maxDistanceToMovePerCall;  // equivalent to the speed of the movement 

    // Update is called once per frame
    void Update()
    {
        float inputValue = inputAction.action.ReadValue<float>(); // gets the value of input action; input action needs to be able to return a float value or there may be an error
        if (inputValue > 0)                                       // checks to see if the value is more than 1 (i.e. input action activated)
        {
            // Debug.Log("move called");
            xrRigTransform.position = Vector3.MoveTowards(xrRigTransform.position, targetTransform.position,inputValue*maxDistanceToMovePerCall); // changes the position of the XR rig by moving it towards the target transform at the rate defined by the inputValue multiplied by maxDistanceToMovePerCall
        }
    }
}
