using UnityEngine;
using System.Collections;

public class CameraOrbit : MonoBehaviour
{
//from this: https://www.youtube.com/watch?v=bVo0YLLO43s&list=PLcGThBpoNbL_7RZR3X_iSMcQnIxWpir39&index=91&t=8s

    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;

    protected Vector3 _LocalRotation;
    protected float _CameraDistance = 10f;

    public float MouseSensitivity = 4f;
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;

    public bool CameraDisabled = false;


    // Use this for initialization
    void Start()
    {
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;
    }


    void LateUpdate()
    {
        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

        if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
        {
            this._XForm_Camera.localPosition = new Vector3(0f, 0f,
                Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f,
                    Time.deltaTime * ScrollDampening));
        }
    }
}