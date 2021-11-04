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
    [SerializeField] private Transform _earPos;
    
    // Make sure XRRig is zeroed out
    // lerp between these two values based on percent
    // first float determines size of "deadzone"
    [SerializeField] private float _headsetZThresh = 0.1f;
    [SerializeField] private float _headsetZEndThresh = 1.0f;
    
    [SerializeField] private float _headsetXThresh = 0.05f;
    [SerializeField] private float _headsetXEndThresh = 0.5f;
    
    [Header("Debug Fields")]
    public Rigidbody rb;
    public float headsetZDistance;
    public float headsetZAngle;
    
    public float headsetXDistance;
    public float headsetYDistance;
    [SerializeField] private bool _leftTurn = false;
    [SerializeField] private bool _rightTurn = false;
    
    public float speed = 12.5f;
    public float drag = 0f;
    public float percentage;
    
    
    private Vector3 _headsetIniPos;
    private Vector3 _feetPos;
    
    private Vector3 rot;
    
    //collision detection
    [SerializeField] private bool _collided = false;
    
    //change these to enum
    private bool _brakes;
    private bool _stunned;
    private bool _speedUp;
    
    [Header("Raycast Bomb")]
    [SerializeField] private List<Transform> bottomRaycastTransforms;
    [SerializeField] private float _RayXOffset = 1.6f;
    [SerializeField] private float _RayZOffset = 0.5f;
    [SerializeField] private float _angleDegree = 15; //for side rays

    // Start is called before the first frame update
    void Start()
    {
        Time.fixedDeltaTime = 1f / 72; // Prevents stutter: Set to match with headset settings
        
        // Initialize ray positions
        //center rays
        bottomRaycastTransforms[0].position = transform.TransformPoint(bottomRaycastTransforms[0].localPosition.x,
            bottomRaycastTransforms[0].localPosition.y, bottomRaycastTransforms[0].localPosition.z); 
        
        bottomRaycastTransforms[1].position = transform.TransformPoint(bottomRaycastTransforms[0].localPosition.x + _RayXOffset,
            bottomRaycastTransforms[0].localPosition.y, bottomRaycastTransforms[0].localPosition.z); //front ray
        
        bottomRaycastTransforms[2].position = transform.TransformPoint(bottomRaycastTransforms[0].localPosition.x - _RayXOffset,
            bottomRaycastTransforms[0].localPosition.y, bottomRaycastTransforms[0].localPosition.z); //back ray

        //left
        bottomRaycastTransforms[3].position = transform.TransformPoint(bottomRaycastTransforms[0].localPosition.x,
            bottomRaycastTransforms[0].localPosition.y, bottomRaycastTransforms[0].localPosition.z + _RayZOffset); 
        
        bottomRaycastTransforms[4].position = transform.TransformPoint(bottomRaycastTransforms[3].localPosition.x + _RayXOffset,
            bottomRaycastTransforms[3].localPosition.y, bottomRaycastTransforms[3].localPosition.z); //front ray
        
        bottomRaycastTransforms[5].position = transform.TransformPoint(bottomRaycastTransforms[3].localPosition.x - _RayXOffset,
            bottomRaycastTransforms[3].localPosition.y, bottomRaycastTransforms[3].localPosition.z); //back ray
        
        //right
        bottomRaycastTransforms[6].position = transform.TransformPoint(bottomRaycastTransforms[0].localPosition.x,
            bottomRaycastTransforms[0].localPosition.y, bottomRaycastTransforms[0].localPosition.z - _RayZOffset); 
        
        bottomRaycastTransforms[7].position = transform.TransformPoint(bottomRaycastTransforms[6].localPosition.x + _RayXOffset,
            bottomRaycastTransforms[6].localPosition.y, bottomRaycastTransforms[6].localPosition.z); //front ray
        
        bottomRaycastTransforms[8].position = transform.TransformPoint(bottomRaycastTransforms[6].localPosition.x - _RayXOffset,
            bottomRaycastTransforms[6].localPosition.y, bottomRaycastTransforms[6].localPosition.z); //back ray
        
        rb = GetComponent<Rigidbody>();
        rot = transform.eulerAngles;
        
        rb.velocity = Vector3.zero;
        
        Invoke(nameof(InitializePositions), 2f); //need these to initialize after board pos script

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
        _feetPos = new Vector3(0f, 0f, 0f);
    }

    #region InputActionCallbacks
    private void OnBrakePressed(InputAction.CallbackContext obj)
    {
        //Debug.Log("brakes pressed");
        _brakes = true;
    }

    private void OnSpeedUpCancel(InputAction.CallbackContext obj)
    {
        _speedUp = false;
    }

    private void OnSpeedUp(InputAction.CallbackContext obj)
    {
        _speedUp = true;

        _brakes = false;
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

    #endregion
    

    // Update is called once per frame
    void Update ()
    {
    }

    [Header("Flying Physics")]
    public float FlyingSpeed = 20f;
    public float FlyingAdjustmentSpeed = 2f; //how quickly our velocity adjusts to the flying speed
    public float FlyingAcceleration = 4f;
    public float FlyingMinSpeed = 6f; //our flying slow down speed
    public float FlyingDecelleration = 0.1f; //how quickly we slow down when flying
    private float FlyingAdjustmentLerp = 0; //the lerp for our adjustment amount
    private float ActGravAmt; //the actual gravity that is applied to our character
    
    [Header("Board Gravity")]
    public float FlyingGravityAmt = 2f; //how much gravity will pull us down when flying
    public float GlideGravityAmt = 4f; //how much gravity affects us when just gliding
    public float FlyingGravBuildSpeed = 0.2f; //how much our gravity is lerped when stopping flying
    
    [Header("Wall Impact")]
    public float StunnedTime = 0.25f; //how long we are stunned for
    private float StunTimer; //the in use stun timer

    [Header("Turning")] 
    public float pitchHeadAngle;
    public float maxPitchAngle;
    public float rollHeadAngle;
    public float maxRollAngle;
    [SerializeField] private float _pitchDampeningFactor = 0.95f;
    [SerializeField] private float _rollDampeningFactor = 0.95f;
    
    private void FixedUpdate()
    {
        // Create raycast bomb here 
        // NOTE: can change layermask to bitwise operator later to only collide with game level 
        int layerMask =~ LayerMask.GetMask("Ignore Raycast");
        //bottom of board raycasts
        for (int i = 0; i < bottomRaycastTransforms.Count; i++)
        {
            Debug.DrawRay(bottomRaycastTransforms[i].position, -transform.up, Color.magenta);
            
            //create ray
            RaycastHit hit;

            if (Physics.Raycast(bottomRaycastTransforms[i].position, -transform.up, out hit, 1f, layerMask))
            {
//                Debug.Log("bottom ray" + i + " colliding with " + hit.transform.gameObject.name);
                
                //check if grounded, in this case use collided bool
                if (!_collided)
                {
                    rb.useGravity = true;

                    _collided = true;
                    //slow velocity (or apply velocity in the normal's direction?)
                    rb.velocity *= 0.98f;
                }
            }
            else
            {
                rb.useGravity = false;
                _collided = false;
            }
        }
        
        //top of board raycasts
        for (int i = 0; i < bottomRaycastTransforms.Count; i++)
        {
            Debug.DrawRay(bottomRaycastTransforms[i].position, transform.up, Color.yellow);
            
            //create ray
            RaycastHit hit;
            
            if (Physics.Raycast(bottomRaycastTransforms[i].position, transform.up, out hit, 1f, layerMask))
            {
                //Debug.Log("top ray"+ i +" colliding with " + hit.transform.gameObject.name);
                
                //check if grounded, in this case use collided bool
                if (!_collided)
                {
                    //slow velocity (or apply velocity in the normal's direction?)
                    rb.velocity += hit.normal.normalized;
                }
            }
        } 

        //side of board raycasts
        Vector3 noAngle = transform.forward;

        for (int i = 0; i < 360/_angleDegree; i++)
        {
            Quaternion spreadAngle = Quaternion.AngleAxis(_angleDegree*i, transform.up);
            Vector3 newVector = spreadAngle * noAngle;
            Debug.DrawRay(transform.position, newVector*2f, Color.black);
            
            //create ray
            RaycastHit hit;
            
            if (Physics.Raycast(transform.position, newVector, out hit, 2f, layerMask))
            {
                //Debug.Log("side ray"+ i +" colliding with " + hit.transform.gameObject.name);
                
                //check if grounded, in this case use collided bool
                if (!_collided)
                {
                    //rotate to make the board face up in relation to surface
                    RotateSelf(hit.normal, Time.deltaTime, ActGravAmt);
                }
            }
        }

        //change velocity and add torque accordingly
        
        if (_brakes)
        {
            FlyingAdjustmentLerp = 0;    //reset flying adjustment
            
            rb.velocity = rb.velocity * 0.98f;
            
            rb.angularVelocity = rb.angularVelocity * 0.98f;
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
        
        // this along with hand distance can be used to increase or decrease drag
        // CURRENTLY NOT BEING USED
        headsetYDistance = (_headsetIniPos.y - (0f - _headset.localPosition.y));
        
        #region Steering 
        
        // this sets pitch direction
        headsetZDistance = -_headsetIniPos.z-(0f - _headset.localPosition.z); // take the initial position as the center and calculate offset
        
        // this sets roll direction
        headsetXDistance = -_headsetIniPos.x-(0f -_headset.localPosition.x); 
        
        // PITCH //
        //Calculate Pitch
        
        Vector3 earPosRelativeToBoard = transform.InverseTransformPoint(_earPos.position); // convert ear position to board local position
        
        Debug.DrawLine(transform.TransformPoint(_feetPos), 
            transform.TransformPoint(Vector3.Scale(earPosRelativeToBoard, new Vector3(0, 1, 1))), // zero out the x pos
            Color.blue);
        
        float heightDistance = Vector3.Distance(transform.TransformPoint(_feetPos), _earPos.position); // for matching distance (to look nice)
        
        Debug.DrawLine(transform.TransformPoint(new Vector3(_feetPos.x, _feetPos.y + heightDistance, _feetPos.z)), 
            transform.TransformPoint(_feetPos),
            Color.cyan); // Neutral up direction 0 degrees

        Vector3 upDirection = transform.up;
        Vector3 pitchTiltDirection = transform.TransformPoint(Vector3.Scale(earPosRelativeToBoard, new Vector3(0, 1, 1))) - transform.TransformPoint(_feetPos);

        // remember this always returns positive
        pitchHeadAngle = Vector3.Angle(upDirection, pitchTiltDirection);
        
        // if headset is leaning back, make the angle negative
        if (headsetZDistance < 0)
        {
            pitchHeadAngle = -pitchHeadAngle;
        }

        //Apply Pitch
        rb.AddTorque(transform.right * pitchHeadAngle  * _pitchDampeningFactor, ForceMode.Force);

        // ROLL //
        //Calculate Roll
        
        earPosRelativeToBoard = transform.InverseTransformPoint(_earPos.position); // convert ear position to board local position
        
        Debug.DrawLine(transform.TransformPoint(_feetPos), 
            transform.TransformPoint(Vector3.Scale(earPosRelativeToBoard, new Vector3(1, 1, 0))), // zero out the z pos
            Color.yellow);
        
        upDirection = transform.up;
        Vector3 rollTiltDirection = transform.TransformPoint(Vector3.Scale(earPosRelativeToBoard, new Vector3(1, 1, 0))) - transform.TransformPoint(_feetPos);

        // remember this always returns positive
        rollHeadAngle = Vector3.Angle(upDirection, rollTiltDirection);
        
        // if headset is leaning (left?), make the angle negative
        if (headsetXDistance < 0)
        {
            rollHeadAngle = -rollHeadAngle;
        }

        //Apply 
        rb.AddTorque(-transform.forward * rollHeadAngle  * _rollDampeningFactor, ForceMode.Force);
        
        
        // YAW //
        float turnAnglePerFixedUpdate = 0.05f;
        float torqueAmount = 5f;
        Quaternion leftQ;
        //control turning or yaw
        if (_leftTurn)
        {
            var rot = Quaternion.AngleAxis(-15,transform.up);
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
            rb.AddTorque(cross * angleDiff  * torqueAmount * speed/10 * 0.8f, ForceMode.Force); // .95 is for damping
        }

        if (_rightTurn)
        {
            var rot = Quaternion.AngleAxis(15,transform.up);
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
            rb.AddTorque(cross * angleDiff  * torqueAmount * speed/10 * 0.8f, ForceMode.Force); // .95 is for damping
        }
        #endregion
        
        //push down more when not pressing fly
        if(_speedUp)
            ActGravAmt = Mathf.Lerp(ActGravAmt, FlyingGravityAmt, FlyingGravBuildSpeed * 4f * Time.deltaTime);
        else
            ActGravAmt = Mathf.Lerp(ActGravAmt, GlideGravityAmt, FlyingGravBuildSpeed * 0.5f * Time.deltaTime);
        
        targetVelocity -= Vector3.up * ActGravAmt;
      
        //lerp velocity
        Vector3 dir = Vector3.Lerp(rb.velocity, targetVelocity, Time.deltaTime * FlyLerpSpd);
        rb.velocity = dir;
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
