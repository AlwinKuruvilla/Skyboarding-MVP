using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Copied from: https://github.com/anigokul/HoverBoard/commit/1aaa90baacf5ab3bde4f6302c34e776a22c140a3

public class Hover : MonoBehaviour
{
    Rigidbody hb;
    public float mult;
    public float moveForce;
    public float turnTorque;

    void Start()
    {
        hb = GetComponent<Rigidbody>();
    }

    public Transform[] anchors = new Transform[4];
    public RaycastHit[] hits = new RaycastHit[4];

    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
            ApplyF(anchors[i], hits[i]);

        hb.AddForce(Input.GetAxis("Vertical") * moveForce * transform.forward);
        hb.AddTorque(Input.GetAxis("Horizontal") * turnTorque * transform.up);

    }

    // Apply upward force at each anchor
    void ApplyF(Transform anchor, RaycastHit hit)
    {
        if (Physics.Raycast(anchor.position, -anchor.up, out hit))
        {
            float force = 0;
            force = Mathf.Abs(1 / (hit.point.y - anchor.position.y));
            hb.AddForceAtPosition(transform.up * force * mult, anchor.position, ForceMode.Acceleration);
        }
    }

}