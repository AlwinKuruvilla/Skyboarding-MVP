using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkyboardController : MonoBehaviour
{
    [SerializeField] private InputActionReference _leftTurnButton;
    [SerializeField] private InputActionReference _rightTurnButton;
    [SerializeField] private AddForces _addForces;
    [SerializeField] private float _turnDampening = 2f;
    [SerializeField] private Transform _headset;
    
    // lerp between these two values based on percent
    // first float determines size of "deadzone"
    [SerializeField] private float _headsetZThresh = 0.1f;
    [SerializeField] private float _headsetZEndThresh = 1.0f;
    
    [SerializeField] private float _headsetXThresh = 0.05f;
    [SerializeField] private float _headsetXEndThresh = 0.5f;
    
    public float headsetZDistance;
    public float headsetXDistance;
    public float headsetYDistance;
    
    private Vector3 _headsetIniPos;

    private bool _leftTurn = false;
    private bool _rightTurn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //keep thrust off until positions are initialized
        _addForces.moveForce = 0f;
        
        //eventually change this to a pos initializer like in beatsaber
        Invoke(nameof(InitializePositions), 5f);
        
        //listen for button presses
        _leftTurnButton.action.started += OnLeftTurnButton;
        _rightTurnButton.action.started += OnRightTurnButton;
        
        //listen for button cancels
        _leftTurnButton.action.canceled += OnLeftTurnCancel;
        _rightTurnButton.action.canceled += OnRightTurnCancel;
    }

    private void OnRightTurnCancel(InputAction.CallbackContext obj)
    {
        _rightTurn = false;
    }

    private void OnLeftTurnCancel(InputAction.CallbackContext obj)
    {
        _leftTurn = false;
    }

    private void OnRightTurnButton(InputAction.CallbackContext obj)
    {
        _rightTurn = true;
    }

    private void OnLeftTurnButton(InputAction.CallbackContext obj)
    {
        _leftTurn = true;
    }

    private void InitializePositions()
    {
        _headsetIniPos = Vector3.zero - _headset.localPosition;
        _addForces.moveForce = 25f;
    }
    
    // Update is called once per frame
    void Update()
    {
        // this controls pitch
        headsetZDistance = (_headsetIniPos.z - (0f - _headset.localPosition.z)); // take the initial position as the center and calculate offset
        
        // this controls roll
        headsetXDistance = (0 - _headset.localPosition.x); 
        
        // this can be used to increase or decrease drag
        // CURRENTLY NO BEING USED
        headsetYDistance = (_headsetIniPos.y - (0f - _headset.localPosition.y));
        
        //Change Pitch
        if (headsetZDistance < -_headsetZThresh)
        {
            float lerpPct = headsetZDistance / (_headsetZEndThresh - _headsetZThresh);
            // changes the percentage value of the position of the headset within the range to match a number between a and b
            _addForces.pitchTorque = Mathf.Lerp(0, -1f, -lerpPct); 
        }
        else if (headsetZDistance > _headsetZThresh)
        {
            float lerpPct = headsetZDistance / (_headsetZEndThresh - _headsetZThresh);
            _addForces.pitchTorque = Mathf.Lerp(0, 1f, lerpPct);
        }
        else
        {
            _addForces.pitchTorque = 0f; //if in deadzone just set to nothing
        }
        
        //Change Roll
        if (headsetXDistance < -_headsetXThresh)
        {
            float lerpPct = headsetXDistance / (_headsetXEndThresh - _headsetXThresh);
            _addForces.rollTorque = Mathf.Lerp(0, -1, -lerpPct);
        }
        else if (headsetXDistance > _headsetXThresh)
        {
            float lerpPct = headsetXDistance / (_headsetXEndThresh - _headsetXThresh);
            _addForces.rollTorque = Mathf.Lerp(0, 1, lerpPct);
        }
        else
        {
            _addForces.rollTorque = 0f;
        }

        //control turning or yaw
        if (_leftTurn)
        {
            //change Yaw toward the left incremently
            _addForces.turnTorque -= Time.deltaTime;
            //add dampening
            _addForces.turnTorque += _addForces.turnTorque * Time.deltaTime * _turnDampening;
        }

        if (_rightTurn)
        {
            //change Yaw toward the right incremently
            _addForces.turnTorque += Time.deltaTime;
            //add dampening
            _addForces.turnTorque -= _addForces.turnTorque * Time.deltaTime * -_turnDampening;
        }

        //eventually change this to be paired individually
        if (!_leftTurn && !_rightTurn)
        {
            _addForces.turnTorque = 0;
        }
        
    }
}
