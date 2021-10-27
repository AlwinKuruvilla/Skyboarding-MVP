using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkyboardController : MonoBehaviour
{
    [SerializeField] private InputActionReference _speedUpInput;
    [SerializeField] private InputActionReference _brakeInput;
    [SerializeField] private InputActionReference _leftTurnButton;
    [SerializeField] private InputActionReference _rightTurnButton;
    [SerializeField] private float _turnDampening = 2f;
    [SerializeField] private Transform _headset;
    
    // Make sure XRRig is zeroed out
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

    [SerializeField] private bool _leftTurn = false;
    [SerializeField] private bool _rightTurn = false;

    public Rigidbody rb;
    public float speed = 12.5f;
    public float drag = 0f;
    public float percentage;

    private Vector3 rot;
    
    //collision detection
    [SerializeField] private bool _collided = false;
    
    //change these to enum
    private bool _brakes;
    private bool _stunned;
    private bool _speedUp;
    
    // Start is called before the first frame update
    void Start()
    {
        Time.fixedDeltaTime = 1f / 72;
            
        rb = GetComponent<Rigidbody>();
        rot = transform.eulerAngles;
        
        rb.velocity = Vector3.zero;
        
        Invoke(nameof(InitializePositions), 2f);

        //listen for button presses
        _brakeInput.action.started += OnBrakePressed;
        _leftTurnButton.action.started += OnLeftTurnButton;
        _rightTurnButton.action.started += OnRightTurnButton;
        _speedUpInput.action.started += OnSpeedUp;
        
        //listen for button cancels
        _leftTurnButton.action.canceled += OnLeftTurnCancel;
        _rightTurnButton.action.canceled += OnRightTurnCancel;
        _speedUpInput.action.canceled += OnSpeedUpCancel;
    }

    private void InitializePositions()
    {
        _headsetIniPos = _headset.localPosition;
    }

    private void OnBrakePressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)

    {
        Debug.Log("brakes pressed");
        _brakes = true;
    }

    private void OnSpeedUpCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _speedUp = false;
    }

    private void OnSpeedUp(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _speedUp = true;

        _brakes = false;
    }

    private void OnRightTurnCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _rightTurn = false;
    }

    private void OnLeftTurnCancel(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _leftTurn = false;
    }

    private void OnRightTurnButton(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _rightTurn = true;
    }

    private void OnLeftTurnButton(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _leftTurn = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float FlyingSpeed = 20f;
    public float FlyingAdjustmentSpeed = 2f; //how quickly our velocity adjusts to the flying speed
    public float FlyingAcceleration = 4f;
    public float FlyingMinSpeed = 6f; //our flying slow down speed
    public float FlyingDecelleration = 0.1f; //how quickly we slow down when flying
    private float FlyingAdjustmentLerp = 0; //the lerp for our adjustment amount
    private float ActGravAmt; //the actual gravity that is applied to our character
    
    [Header("Flying Physics")]
    public float FlyingGravityAmt = 2f; //how much gravity will pull us down when flying
    public float GlideGravityAmt = 4f; //how much gravity affects us when just gliding
    public float FlyingGravBuildSpeed = 0.2f; //how much our gravity is lerped when stopping flying
    
    [Header("Wall Impact")]
    public float StunnedTime = 0.25f; //how long we are stunned for
    private float StunTimer; //the in use stun timer
    
    [Header("Turning")]
    public float pitchTorque;
    public float rollTorque;
    [SerializeField] private float _pitchForce = 50f;
    [SerializeField] private float _rollForce = 50f;


    private Vector3 velocityforLU;
    private Vector3 angularVelocityForLU;
    private void FixedUpdate()
    {
        if (_brakes)
        {
            FlyingAdjustmentLerp = 0;    //reset flying adjustment
            rb.velocity = rb.velocity * 0.95f;
            
            rb.angularVelocity = rb.angularVelocity * 0.95f;

            return;
        }
        
        if (_stunned)
        {
            //reduce stun timer
            if (StunTimer > 0)
            {
                StunTimer -= Time.deltaTime;

                if (StunTimer > StunnedTime * 0.5f)
                    return;
            }
            
            //lerp mesh slower when not on ground
            Vector3 DownwardDirection = Vector3.up;
            RotateSelf(DownwardDirection, Time.deltaTime, 8f);

            float turnSpeed = 2f;
            RotateMesh(Time.deltaTime, transform.forward, turnSpeed);

            //push backwards while we fall
            Vector3 FallDir = -transform.forward * 4f;
            FallDir.y = rb.velocity.y;
            rb.velocity = Vector3.Lerp(rb.velocity, FallDir, Time.deltaTime * 2f);

            _stunned = false;
            return;
        }
        
        //copied from https://assetstore.unity.com/packages/tools/physics/third-person-flying-controller-181621
        //lerp controls
        if (FlyingAdjustmentLerp < 1.1)
            FlyingAdjustmentLerp += Time.deltaTime * FlyingAdjustmentSpeed;

        //lerp speed
        float YAmt = rb.velocity.y;
        float FlyAccel = FlyingAcceleration * FlyingAdjustmentLerp;
        float Spd = FlyingSpeed;
        if (!_speedUp)  //we are not holding speedup/fly, slow down
        {
            Spd = FlyingMinSpeed; 
            if(speed > FlyingMinSpeed)
                FlyAccel = FlyingDecelleration * FlyingAdjustmentLerp;
        }
        else
        {
            //flying visual effects 
        }
        
        if (speed > FlyingSpeed) //we are over out max speed, slow down slower
            FlyAccel = FlyAccel * 0.8f;
        
        //handle how our speed is increased or decreased when flying
        float targetSpeed = Spd;
        
        if (YAmt < -6) //we are flying down! boost speed
        {
            targetSpeed = targetSpeed + (2 * (YAmt * -0.5f));
        }
        else if (YAmt > 7) //we are flying up! reduce speed
        {
            targetSpeed = targetSpeed - (0.5f * YAmt);
            speed -= (0.5f * YAmt) * Time.deltaTime;
        }
        
        //clamp speed
        targetSpeed = Mathf.Clamp(targetSpeed, -50, 50);
        //lerp speed
        speed = Mathf.Lerp(speed, targetSpeed, FlyAccel * Time.deltaTime);
        
        //apply speed
        float FlyLerpSpd = FlyingAdjustmentSpeed * FlyingAdjustmentLerp;
        Vector3 targetVelocity = transform.forward * speed;
        
        //CHANGE THESE INTO ROTATIONS
        //INPUT HANDLE IN A SEPARATE SCRIPT Used joe's inputs for reference
        
        // this controls pitch
        headsetZDistance = (-(0f - _headset.localPosition.z)); // take the initial position as the center and calculate offset
        
        // this controls roll
        headsetXDistance = (0f -_headset.localPosition.x); 
        
        // this can be used to increase or decrease drag
        // CURRENTLY NO BEING USED
        headsetYDistance = (_headsetIniPos.y - (0f - _headset.localPosition.y));

        //Change Roll
        if (headsetXDistance < -_headsetXThresh)
        {
            float lerpPct = headsetXDistance / (_headsetXEndThresh - _headsetXThresh);
            rollTorque = Mathf.Lerp(0, -1, -lerpPct);
        }
        else if (headsetXDistance > _headsetXThresh)
        {
            float lerpPct = headsetXDistance / (_headsetXEndThresh - _headsetXThresh);
            rollTorque = Mathf.Lerp(0, 1, lerpPct);
        }
        else
        { 
            rollTorque = 0f;
        }
        
        //Change Pitch
        if (headsetZDistance < -_headsetZThresh)
        {
            float lerpPct = headsetZDistance / (_headsetZEndThresh - _headsetZThresh);
            // changes the percentage value of the position of the headset within the range to match a number between a and b
            pitchTorque = Mathf.Lerp(0, -1f, -lerpPct); 
        }
        else if (headsetZDistance > _headsetZThresh)
        {
            float lerpPct = headsetZDistance / (_headsetZEndThresh - _headsetZThresh);
            pitchTorque = Mathf.Lerp(0, 1f, lerpPct);
        }
        else
        {
            pitchTorque = 0f; //if in deadzone just set to nothing
        }

        float turnAnglePerFixedUpdate = 0.1f;
        float torqueAmount = 3f;
        Quaternion leftQ;
        //control turning or yaw
        if (_leftTurn)
        {
            var rot = Quaternion.AngleAxis(-5,transform.up);
            // copied from https://www.reddit.com/r/Unity3D/comments/30vhyl/struggling_with_smoothly_rotating_a_rigidbody/
            Vector3 direction = rot * transform.forward;
            // Create a quaternion (rotation) 
            Quaternion q = Quaternion.AngleAxis(turnAnglePerFixedUpdate, rb.transform.up) * rb.rotation;

            //get the angle between transform.forward and target delta
            float angleDiff = Vector3.Angle(transform.forward, direction);
		
            // get its cross product, which is the axis of rotation to
            // get from one vector to the other
            Vector3 cross = Vector3.Cross(transform.forward, direction);
            
            // apply torque along that axis according to the magnitude of the angle.
            rb.angularVelocity = (cross * angleDiff  * torqueAmount);
        }

        if (_rightTurn)
        {
            var rot = Quaternion.AngleAxis(5,transform.up);
            // copied from https://www.reddit.com/r/Unity3D/comments/30vhyl/struggling_with_smoothly_rotating_a_rigidbody/
            Vector3 direction = rot * transform.forward;
            // Create a quaternion (rotation) 
            Quaternion q = Quaternion.AngleAxis(turnAnglePerFixedUpdate, rb.transform.up) * rb.rotation;

            //get the angle between transform.forward and target delta
            float angleDiff = Vector3.Angle(transform.forward, direction);
		
            // get its cross product, which is the axis of rotation to
            // get from one vector to the other
            Vector3 cross = Vector3.Cross(transform.forward, direction);
		
            // apply torque along that axis according to the magnitude of the angle.
            rb.angularVelocity = cross * angleDiff  * torqueAmount;
        }
        
        //CHANGE PITCH AND ROLL BASED ON HEAD ANGLE TO BOTTOM OF XRRIG
        //and then eventually to pos of feet relative
        
        //change Pitch
        rb.angularVelocity += (pitchTorque * rb.transform.right * _pitchForce);

        angularVelocityForLU = rb.angularVelocity;

        //change Roll
        rb.angularVelocity += (rollTorque * rb.transform.forward * _rollForce);
        //rb.AddTorque(rollTorque * rb.transform.forward * _rollForce, ForceMode.Force);

        //push down more when not pressing fly
        if(_speedUp)
            ActGravAmt = Mathf.Lerp(ActGravAmt, FlyingGravityAmt, FlyingGravBuildSpeed * 4f * Time.deltaTime);
        else
            ActGravAmt = Mathf.Lerp(ActGravAmt, GlideGravityAmt, FlyingGravBuildSpeed * 0.5f * Time.deltaTime);
        
        targetVelocity -= Vector3.up * ActGravAmt;
      
        //lerp velocity
        Vector3 dir = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * FlyLerpSpd);
        rb.velocity = dir;

        velocityforLU = rb.velocity;
    }

    private void LateUpdate()
    {
        //rb.velocity = velocityforLU;
        //rb.angularVelocity = angularVelocityForLU;
    }

    //rotate our upwards direction
    void RotateSelf(Vector3 Direction, float d, float GravitySpd)
    {
        Vector3 LerpDir = Vector3.Lerp(transform.up, Direction, d * GravitySpd);
        transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
    }
    //rotate our left right direction
    void RotateMesh(float d, Vector3 LookDir, float spd)
    {
        Quaternion SlerpRot = Quaternion.LookRotation(LookDir, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, spd * d);
    }
    //rotate towards the velocity direction
    void RotateToVelocity(float d, float spd)
    {
        Quaternion SlerpRot = Quaternion.LookRotation(rb.velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, spd * d);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_collided) return; //if still in contact with collider don't execute code
        _collided = true;
        
        if (other.gameObject.CompareTag("Ground")) return;
        
        
        float SpeedLimitBeforeCrash = 5f;
        
        if (speed > SpeedLimitBeforeCrash)
        {
            StunTimer = StunnedTime;
            
            //set physics
            speed = 0f;
            rb.velocity = Vector3.zero;

            //push away in the direction of the normal
            Vector3 PushDirection = other.contacts[0].normal;
            float StunPushBack = 5f;
            rb.AddForce(PushDirection * StunPushBack, ForceMode.Impulse);

            _stunned = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (_collided) _collided = false;
        
        ActGravAmt = FlyingGravityAmt; //our gravity is returned to the flying amount
    }
}
