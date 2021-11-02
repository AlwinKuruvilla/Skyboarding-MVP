using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{
   private Vector3 _originalRot;
   
   private void Start()
   {
      _originalRot = transform.rotation.eulerAngles;
   }

   private void LateUpdate()
   {
      transform.rotation = Quaternion.Euler(_originalRot);
   }
}
