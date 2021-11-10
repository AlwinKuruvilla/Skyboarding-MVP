using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour {
	BoidSettings _settings;

	// State
	[HideInInspector] public Vector3 position;
	[HideInInspector] public Vector3 forward;
	Vector3 _velocity;
	[HideInInspector] public bool isEvading;
	private bool _isCaptured;
	private EvasionType _evasionType;

	// To update:
	[HideInInspector] public Vector3 avgFlockHeading;
	[HideInInspector] public Vector3 avgAvoidanceHeading;
	[HideInInspector] public Vector3 centreOfFlockmates;
	[HideInInspector] public int numPerceivedFlockmates;

	// Cached
	Material _material;
	Transform _cachedTransform;
	Transform _target;
	
	public enum EvasionType {
		EvadeUp = 0,
		EvadeDown = 1,
		EvadeLeft = 2,
		EvadeRight = 3
	}

	void Awake() {
		//_material = transform.GetComponentInChildren<MeshRenderer>().material;
		_cachedTransform = transform;
	}

	public void Initialize(BoidSettings settings, Transform target) {
		this._target = target;
		this._settings = settings;

		position = _cachedTransform.position;
		forward = _cachedTransform.forward;

		float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
		_velocity = transform.forward * startSpeed;

		_evasionType = (EvasionType)Random.Range(0, 4);
		//SetColour();
	}

	// public void SetColour() {
	// 	switch (_evasionType) {
	// 		case EvasionType.EvadeUp:
	// 			_material.color = Color.red;
	// 			break;
	// 		case EvasionType.EvadeDown:
	// 			_material.color = Color.blue;
	// 			break;
	// 		case EvasionType.EvadeLeft:
	// 			_material.color = Color.green;
	// 			break;
	// 		case EvasionType.EvadeRight:
	// 			_material.color = Color.yellow;
	// 			break;
	// 		default:
	// 			_material.color = Color.black;
	// 			break;
	// 	}
	// }

	public void UpdateBoid() {
		Vector3 acceleration = Vector3.zero;
		if (!_isCaptured) {
			if (_target != null) {
				Vector3 offsetToTarget = (_target.position - position);
				acceleration = SteerTowards(offsetToTarget) * _settings.targetWeight;
			}

			if (numPerceivedFlockmates != 0) {
				centreOfFlockmates /= numPerceivedFlockmates;

				Vector3 offsetToFlockmatesCentre = (centreOfFlockmates - position);

				var alignmentForce = SteerTowards(avgFlockHeading) * _settings.alignWeight;
				var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * _settings.cohesionWeight;
				var seperationForce = SteerTowards(avgAvoidanceHeading) * _settings.seperateWeight;

				acceleration += alignmentForce;
				acceleration += cohesionForce;
				acceleration += seperationForce;
			}

			if (IsHeadingForCollision()) {
				Vector3 collisionAvoidDir = ObstacleRays();
				Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * _settings.avoidCollisionWeight;
				acceleration += collisionAvoidForce;
			}

			if (isEvading) {
				Vector3 evadeForce = SteerTowards(EvasionDir()) * _settings.evadeWeight;
				acceleration = evadeForce;
				isEvading = false;
			}

			_velocity += acceleration * Time.deltaTime;
			float speed = _velocity.magnitude;
			Vector3 dir = _velocity / speed;
			speed = Mathf.Clamp(speed, _settings.minSpeed, _settings.maxSpeed);
			_velocity = dir * speed;

			_cachedTransform.position += _velocity * Time.deltaTime;
			_cachedTransform.forward = dir;
			position = _cachedTransform.position;
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
		_isCaptured = true;
	}

	public void IsReleased() {
		_isCaptured = false;
	}

	bool IsHeadingForCollision() {
		RaycastHit hit;
		if (Physics.SphereCast(position, _settings.boundsRadius, forward, out hit, _settings.collisionAvoidDst,
			_settings.obstacleMask)) {
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
			Vector3 dir = _cachedTransform.TransformDirection(rayDirections[i]);
			Ray ray = new Ray(position, dir);
			if (!Physics.SphereCast(ray, _settings.boundsRadius, _settings.collisionAvoidDst, _settings.obstacleMask)) {
				return dir;
			}
		}
		return forward;
	}
	
	

	Vector3 SteerTowards(Vector3 vector) {
		Vector3 v = vector.normalized * _settings.maxSpeed - _velocity;
		return Vector3.ClampMagnitude(v, _settings.maxSteerForce);
	}
}