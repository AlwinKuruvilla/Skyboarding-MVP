using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceCamOffset : MonoBehaviour
{
    [SerializeField] private Transform _mainCamera;

    private void LateUpdate()
    {
    Vector3 offset = new Vector3(_mainCamera.localPosition.x, 0f, _mainCamera.localPosition.z);

        transform.localPosition = Vector3.zero - offset;
    }
}
