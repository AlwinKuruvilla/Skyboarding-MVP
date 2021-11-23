using System;
using Boids;
using Unity.Mathematics;
using UnityEngine;

public class BoidsFollowPosition : MonoBehaviour {
    public Transform[] positionGameObjects;
    private static Transform[] positions;
    private static int positionValue = 0;

    private void Start() {
        positions = positionGameObjects;
    }

    public static void SetPosition(GameObject boid) {
        if (positionValue < positions.Length) {
            boid.transform.parent = positions[positionValue].transform;
            boid.transform.localPosition = Vector3.zero;
            boid.transform.localRotation = quaternion.Euler(0, 0, 0);
            positionValue++;
        }
        else {
            boid.GetComponent<Boid>().IsSetFree();
        }
    }

    public void ResetPositions() {
        positionValue = 0;
    }
}
