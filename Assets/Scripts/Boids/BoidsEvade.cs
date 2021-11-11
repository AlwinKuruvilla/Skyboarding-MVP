using System;
using System.Collections;
using System.Collections.Generic;
using Boids;
using UnityEngine;

public class BoidsEvade : MonoBehaviour {
   public Boid boid;

   private void OnTriggerEnter(Collider other) {
      if (other.gameObject.CompareTag("Player")) {
         boid.isEvading = true;
      }
   }
}
