using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour {
	BoidSettings settings;

	// State
	[HideInInspector] public Vector3 position;
	[HideInInspector] public Vector3 forward;
	Vector3 velocity;
	[HideInInspector] public bool isEvading;
	private bool isCaptured;
	private EvasionType _evasionType;

	// To update:
	Vector3 acceleration;
	[HideInInspector] public Vector3 avgFlockHeading;
	[HideInInspector] public Vector3 avgAvoidanceHeading;
	[HideInInspector] public Vector3 centreOfFlockmates;
	[HideInInspector] public int numPerceivedFlockmates;

	// Cached
	Material material;
	Transform cachedTransform;
	Transform target;
	
	public enum EvasionType {
		EvadeUp = 0,
		EvadeDown = 1,
		EvadeLeft = 2,
		EvadeRight = 3
	}

	void Awake() {
		material = transform.GetComponentInChildren<MeshRenderer>().material;
		cachedTransform = transform;
	}

	public void Initialize(BoidSettings settings, Transform target) {
		this.target = target;
		this.settings = settings;

		position = cachedTransform.position;
		forward = cachedTransform.forward;

		float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
		velocity = transform.forward * startSpeed;

		_evasionType = (EvasionType)Random.Range(0, 4);
		SetColour();
	}

	public void SetColour() {
		switch (_evasionType) {
			case EvasionType.EvadeUp:
				material.color = Color.red;
				break;
			case EvasionType.EvadeDown:
				material.color = Color.blue;
				break;
			case EvasionType.EvadeLeft:
				material.color = Color.green;
				break;
			case EvasionType.EvadeRight:
				material.color = Color.yellow;
				break;
			default:
				material.color = Color.black;
				break;
		}
	}

	public void UpdateBoid() {
		Vector3 acceleration = Vector3.zero;
		if (!isCaptured) {
			if (target != null) {
				Vector3 offsetToTarget = (target.position - position);
				acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
			}

			if (numPerceivedFlockmates != 0) {
				centreOfFlockmates /= numPerceivedFlockmates;

				Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

				var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
				var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * settings.cohesionWeight;
				var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

				acceleration += alignmentForce;
				acceleration += cohesionForce;
				acceleration += seperationForce;
			}

			if (IsHeadingForCollision()) {
				Vector3 collisionAvoidDir = ObstacleRays();
				Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;
				acceleration += collisionAvoidForce;
			}

			if (isEvading) {
				Vector3 evadeForce = SteerTowards(EvasionDir()) * settings.evadeWeight;
				acceleration = evadeForce;
				isEvading = false;
			}

			velocity += acceleration * Time.deltaTime;
			float speed = velocity.magnitude;
			Vector3 dir = velocity / speed;
			speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
			velocity = dir * speed;

			cachedTransform.position += velocity * Time.deltaTime;
			cachedTransform.forward = dir;
			position = cachedTransform.position;
			forward = dir;
		}
	}

	private Vector3 EvasionDir() {
		switch (_evasionType) {
			case EvasionType.EvadeUp:
				return Vector3.up;
			case EvasionType.EvadeDown:
				return Vector3.down;
			case EvasionType.EvadeLeft:
				return Vector3.left;
			case EvasionType.EvadeRight:
				return Vector3.right;
			default:
				return Vector3.back;
		}
	}

	public void IsCaptured() {
		isCaptured = true;
	}

	public void IsReleased() {
		isCaptured = false;
	}

	bool IsHeadingForCollision() {
		RaycastHit hit;
		if (Physics.SphereCast(position, settings.boundsRadius, forward, out hit, settings.collisionAvoidDst,
			settings.obstacleMask)) {
			return true;
		}

		return false;
	}
	
	//Physics.OverlapSphere()

	// private void OnTriggerEnter(Collider other) {
	// 	if (other.gameObject.CompareTag("Player")) {
	// 		Debug.Log("I'm being chased!");
	// 		Vector3 evadeForce = SteerTowards(Vector3.up) * settings.evadeWeight;
	// 		acceleration = evadeForce;
	// 	}
	// 	
	// }

	Vector3 ObstacleRays() {
		Vector3[] rayDirections = BoidHelper.directions;

		for (int i = 0; i < rayDirections.Length; i++) {
			Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
			Ray ray = new Ray(position, dir);
			if (!Physics.SphereCast(ray, settings.boundsRadius, settings.collisionAvoidDst, settings.obstacleMask)) {
				return dir;
			}
		}
		return forward;
	}
	
	

	Vector3 SteerTowards(Vector3 vector) {
		Vector3 v = vector.normalized * settings.maxSpeed - velocity;
		return Vector3.ClampMagnitude(v, settings.maxSteerForce);
	}
}