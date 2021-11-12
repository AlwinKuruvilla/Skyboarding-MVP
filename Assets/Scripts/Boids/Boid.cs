using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boids {
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

		[SerializeField] private Animator _animator;
		public int pointValue;
		[HideInInspector] public Material _material;
		public float capturedSize = 0.2f;

		[Header("Evade Type Up")] 
		public Color evadeTypeUpColorA;
		public Color evadeTypeUpColorB;
		public Color evadeTypeUpColorBase;
		public int evadeTypeUpPointValue = 200;
		
		[Header("Evade Type Down")]
		public Color evadeTypeDownColorA;
		public Color evadeTypeDownColorB;
		public Color evadeTypeDownColorBase;
		public int evadeTypeDownPointValue = 400;
		
		[Header("Evade Type Left")]
		public Color evadeTypeLeftColorA;
		public Color evadeTypeLeftColorB;
		public Color evadeTypeLeftColorBase;
		public int evadeTypeLeftPointValue = 600;
		
		[Header("Evade Type Right")]
		public Color evadeTypeRightColorA;
		public Color evadeTypeRightColorB;
		public Color evadeTypeRightColorBase;
		public int evadeTypeRightPointValue = 700;

		// Cached
		Transform _cachedTransform;
		Transform _target;
		private static readonly int Stop = Animator.StringToHash("Stop");
		private static readonly int Move = Animator.StringToHash("Move");


		public enum EvasionType {
			EvadeUp = 0,
			EvadeDown = 1,
			EvadeLeft = 2,
			EvadeRight = 3
		}

		void Awake() {
			_cachedTransform = transform;
			_material = GetComponentInChildren<SkinnedMeshRenderer>().material;
		}

		public void Initialize(BoidSettings settings, Transform target) {
			this._target = target;
			this._settings = settings;

			position = _cachedTransform.position;
			forward = _cachedTransform.forward;

			float startSpeed = (settings.minSpeed + settings.maxSpeed) / 2;
			_velocity = transform.forward * startSpeed;

			_evasionType = (EvasionType)Random.Range(0, 4);
			SetColorAndPoints();
		}

		public void SetColorAndPoints() {
			switch (_evasionType) {
				case EvasionType.EvadeUp:
					_material.SetColor("ColorA", evadeTypeUpColorA);
					_material.SetColor("ColorB", evadeTypeUpColorB);
					_material.SetColor("BaseColor", evadeTypeUpColorBase);
					pointValue = evadeTypeUpPointValue;
					break;
				case EvasionType.EvadeDown:
					_material.SetColor("ColorA", evadeTypeDownColorA);
					_material.SetColor("ColorB", evadeTypeDownColorB);
					_material.SetColor("BaseColor", evadeTypeDownColorBase);
					pointValue = evadeTypeDownPointValue;
					break;
				case EvasionType.EvadeLeft:
					_material.SetColor("ColorA", evadeTypeLeftColorA);
					_material.SetColor("ColorB", evadeTypeLeftColorB);
					_material.SetColor("BaseColor", evadeTypeLeftColorBase);
					pointValue = evadeTypeLeftPointValue;
					break;
				case EvasionType.EvadeRight:
					_material.SetColor("ColorA", evadeTypeRightColorA);
					_material.SetColor("ColorB", evadeTypeRightColorB);
					_material.SetColor("BaseColor", evadeTypeRightColorBase);
					pointValue = evadeTypeRightPointValue;
					break;
			}
		}

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

				if (gameObject) {
					_cachedTransform.position += _velocity * Time.deltaTime;
					_cachedTransform.forward = dir;
					position = _cachedTransform.position;
				}
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
			_animator.SetTrigger(Stop);
			transform.localScale = new Vector3(capturedSize, capturedSize, capturedSize);
		}

		public void IsReleased() {
			_isCaptured = false;
			_animator.SetTrigger(Move);
			transform.localScale = new Vector3(1f, 1f, 1f);
		}

		public bool CheckCaptureStatus()
		{
			return _isCaptured;
		}

		bool IsHeadingForCollision() {
			RaycastHit hit;
			if (Physics.SphereCast(position, _settings.boundsRadius, forward, out hit, _settings.collisionAvoidDst,
				_settings.obstacleMask)) {
				return true;
			}

			return false;
		}

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

		private void OnDestroy() {
			_isCaptured = false;
		}
	}
}