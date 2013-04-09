using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UnityAssets
{
	public class Mover : PlatformRider
	{
		public enum Mode
		{
			Auto,
				// Select based on which component is attached to the same GameObject
			Basic,
				// Transform movement based on targetVelocity and drag - gravity is ignored
			Physics,
				// Rigidbody movement with drag, gravity and groundedness check
			Navigation
				// NavMeshAgent movement
		};


		public enum RotationMode
		{
			Mover,
			Agent,
			Ignore
		};


		public Mode mode;
			// How should this mover .. er .. move?

		// Any mode //
		public Vector3 targetVelocity, targetPosition;
		public Quaternion targetRotation;
		public float
			rotationInterpolationSpeed = 10.0f,
				// How fast should current, predicted, rotation be interpolated to target rotation?
			positionInterpolationSpeed = 30.0f;
				// How fast should current, predicted, position be interpolated to target position?

		// Mode.Basic and Mode.Physics only //
		public float
			speed = 5.0f,
				// What is the fastest this mover can move?
			rotationSpeed = 5.0f,
				// What is the fastest this mover can rotate?
			arrivalDistance = 1.0f;
				// At which distance to Destination has the mover arrived?

		// Mode.Physics only //
		[System.NonSerialized] public float drag = 0.0f;
			// Drag is tweakable to allow different behaviour when flying/falling vs. grounded
		public Vector3 gravity = new Vector3 (0.0f, -9.81f, 0.0f);
			// Local, tweakable gravity
		public float minimumGravityMovement = 0.001f;
				// Below which position delta should we stop applying gravity in order to stop sliding?
		public LayerMask groundLayers = -1;
			// Which layers should be walkable?
			// NOTICE: Make sure that the target collider is not in any of these layers!
		public float groundedCheckOffset = 0.7f;
			// Tweak so check starts from just within target footing
		public float groundedDistance = 1.0f;
			// Tweak if character lands too soon or gets stuck "in air" often

		// Mode.Navigation only //
		public RotationMode rotationMode;
		public bool
			targetDesiredVelocity = false,
				// Set to true if you plan on doing the velocity constraining around the navMeshAgent
			constrainVelocity = false,
			constrainRotation = false;

		public bool debug = false;
		public float speedScale = 1.0f;
			// TODO: Clean up. Only really relevant when you have a destination.


		// Any mode //
		bool applyPosition = true, firstSync = true, validDestination = false;
		Vector3 destination;

		// Mode.Physics only //
		bool grounded;
		Vector3 lastPosition;


		public NavMeshAgent navMeshAgent
		{
			get
			{
				return GetComponent<NavMeshAgent> ();
			}
		}


		public bool ValidDestination
		{
			get
			{
				return validDestination;
			}
		}


		public Vector3 Destination
		{
			get
			{
				if (!validDestination)
				{
					return Vector3.zero;
				}

				return destination;
			}
			set
			{
				validDestination = true;

				if (mode == Mode.Navigation)
				{
					if (!navMeshAgent.pathPending)
					{
						navMeshAgent.destination = value;
						destination = navMeshAgent.destination;
					}
				}
				else
				{
					destination = value;
				}
			}
		}


		public bool Grounded
		{
			get
			{
				return grounded;
			}
		}


		public bool ApplyPosition
		{
			get
			{
				return applyPosition;
			}
			set
			{
				applyPosition = value;
			}
		}


		public virtual Vector3 ConstrainedVelocity
		{
			get
			{
				return targetVelocity * speedScale;
			}
		}


		public virtual Quaternion ConstrainedRotation
		{
			get
			{
				return targetRotation;
			}
		}


		public virtual Vector3 ConstrainedGravity
		{
			get
			{
				return gravity;
			}
		}


		public float Speed
		{
			get
			{
				switch (mode)
				{
					case Mode.Navigation:
						return navMeshAgent.speed;
					default:
						return speed;
				}
			}
		}


		public Vector3 CurrentVelocity
		{
			get
			{
				switch (mode)
				{
					case Mode.Navigation:
						return navMeshAgent.velocity;
					case Mode.Physics:
						return rigidbody.velocity;
					case Mode.Basic:
					default:
						return targetVelocity;
				}
			}
		}


		public Vector3 CurrentGravityVelocity
		{
			get
			{
				Vector3 gravityNormal = ConstrainedGravity.normalized;
				return Vector3.Dot (CurrentVelocity, gravityNormal) * gravityNormal;
			}
		}


		public Vector3 CurrentPlanarVelocity
		{
			get
			{
				return CurrentVelocity - CurrentGravityVelocity;
			}
		}


		public void ClearDestination ()
		{
			validDestination = false;
			if (mode == Mode.Navigation)
			{
				navMeshAgent.ResetPath ();
			}
		}


		public void TargetCurrent ()
		// Set default state
		{
			targetPosition = transform.position;
			targetRotation = transform.rotation;
			switch (mode)
			{
				case Mode.Basic:
					targetVelocity = Vector3.zero;
					drag = 0.0f;
				break;
				case Mode.Physics:
					targetVelocity = rigidbody.velocity;
					drag = rigidbody.drag;
				break;
				case Mode.Navigation:
					targetVelocity = navMeshAgent.velocity;
					drag = 0.0f;
				break;
			}
		}


		protected virtual void Awake ()
		{
			switch (mode)
			// Verify setup based on set mode
			{
				case Mode.Auto:
					if (navMeshAgent != null)
					{
						mode = Mode.Navigation;
					}
					else if (rigidbody != null)
					{
						mode = Mode.Physics;
					}
					else
					{
						mode = Mode.Basic;
					}
				break;
				case Mode.Physics:
					if (rigidbody == null)
					{
						Debug.LogError ("Physics mode set with no rigidbody available");
						Destroy (this);
					}
				break;
				case Mode.Navigation:
					if (navMeshAgent == null)
					{
						Debug.LogError ("Navigation mode set with no NavMeshAgent available");
						Destroy (this);
					}
				break;
			}

			TargetCurrent ();
				// Set default state
		}


		protected virtual void Start ()
		// Configure rigidbody
		{
			switch (mode)
			{
			 	case Mode.Physics:
					rigidbody.isKinematic = false;
					rigidbody.freezeRotation = true;
					rigidbody.useGravity = false;
				break;
				case Mode.Navigation:
					speed = navMeshAgent.speed;

					navMeshAgent.enabled = true;

					if (applyPosition)
					{
						navMeshAgent.updatePosition = false;
						navMeshAgent.updateRotation = false;
					}
					else
					{
						navMeshAgent.updatePosition = true;

						switch (rotationMode)
						{
							case RotationMode.Agent:
								navMeshAgent.updateRotation = true;
							break;
							case RotationMode.Mover:
							case RotationMode.Ignore:
								navMeshAgent.updateRotation = false;
							break;
						}
					}
				break;
			}
		}


		public virtual void Stop ()
		{
			targetVelocity = Vector3.zero;
			validDestination = false;

			switch (mode)
			{
				case Mode.Navigation:
					navMeshAgent.ResetPath ();
				break;
				case Mode.Physics:
					rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
					rigidbody.Sleep ();
				break;
			}
		}


		void UpdateSimpleDestinationVelocity ()
		{
			if (validDestination)
			// If we have a destination, set velocity towards that
			{
				if ((destination - transform.position).magnitude < arrivalDistance)
				// We arrived, reset the destination
				{
					Stop ();
				}
				else
				{
					targetVelocity = (destination - transform.position).normalized * speed;
				}
			}
		}


		protected virtual void Update ()
		{
			MoverUpdate ();
		}


		protected virtual void MoverUpdate ()
		{
			if (applyPosition)
			{
				Vector3 interpolatedPosition = Vector3.Lerp (transform.position, targetPosition, Time.deltaTime * positionInterpolationSpeed);

				switch (mode)
				{
					case Mode.Basic:
						transform.position = interpolatedPosition;
					break;
					case Mode.Physics:
						rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
						rigidbody.MovePosition (interpolatedPosition);
					break;
					case Mode.Navigation:
						navMeshAgent.velocity = targetVelocity;
						navMeshAgent.Move (interpolatedPosition - transform.position);
						navMeshAgent.nextPosition = interpolatedPosition;
					break;
				}

				PredictPosition (Time.deltaTime);
					// Keep predicting the position until we get a new one from an external source
			}
			else
			{
				switch (mode)
				{
					case Mode.Basic:
						UpdateSimpleDestinationVelocity ();

						transform.position += (constrainVelocity ? ConstrainedVelocity : targetVelocity) * Time.deltaTime;
							// Apply velocity
					break;
					case Mode.Navigation:
						if (!navMeshAgent.enabled || !navMeshAgent.updatePosition)
						{
							transform.position += (constrainVelocity ? ConstrainedVelocity : targetVelocity) * Time.deltaTime;
						}
						else if (!validDestination)
						// No destination - apply velocity
						{
							navMeshAgent.velocity = constrainVelocity ? ConstrainedVelocity : targetVelocity;
						}
						else
						// Go to destination
						{
							if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
							// We arrived, reset the destination
							{
								Stop ();
							}
							else
							{
								navMeshAgent.destination = destination;
								destination = navMeshAgent.destination;
								targetVelocity = targetDesiredVelocity ? navMeshAgent.desiredVelocity : navMeshAgent.velocity;

								if (targetDesiredVelocity || constrainVelocity)
								{
									navMeshAgent.velocity = constrainVelocity ? ConstrainedVelocity : targetVelocity;
								}
							}
						}
					break;
				}

				targetPosition = transform.position;
					// Update the position after having updated
			}

			if (mode == Mode.Physics)
			{
				rigidbody.drag = drag;
			}

			if (applyPosition)
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, constrainRotation ? ConstrainedRotation : targetRotation, Time.deltaTime * rotationInterpolationSpeed);
			}
			else
			{
				if (rotationMode == RotationMode.Mover)
				{
					transform.rotation = Quaternion.Slerp (transform.rotation, constrainRotation ? ConstrainedRotation : targetRotation, Time.deltaTime * rotationSpeed);
				}

				targetRotation = transform.rotation;
			}
		}


		protected virtual void FixedUpdate ()
		{
			if (mode != Mode.Physics)
			{
				grounded = true;
				return;
			}

			grounded = Physics.OverlapSphere (
				transform.position + ConstrainedGravity.normalized * groundedCheckOffset,
				groundedDistance,
				groundLayers
			).Length > 0;
				// Do an overlap sphere at our feet to see if we're touching the ground

			if (!applyPosition)
			{
				UpdateSimpleDestinationVelocity ();

				if (!grounded || ConstrainedVelocity.magnitude > 0.0f)
				// Update velocity if we should be moving
				{
					rigidbody.AddForce (ConstrainedVelocity - rigidbody.velocity + CurrentGravityVelocity, ForceMode.VelocityChange);
				}

				if (!grounded || (transform.position - lastPosition).magnitude > minimumGravityMovement)
				// Only add gravity if not grounded or last position delta exceeds a minimum threshold. Prevents sliding.
				{
					rigidbody.AddForce (ConstrainedGravity);
				}
			}

			lastPosition = transform.position;
		}


		public virtual void Teleport (Vector3 position)
		// Directly move this mover to the specified position
		{
			transform.position = position;
		}


		public override void UpdatePlatform (Vector3 platformDelta)
		// Apply the delta movement of a platform to the mover
		{
			switch (mode)
			{
				case Mode.Basic:
					transform.position += platformDelta;
				break;
				case Mode.Physics:
					transform.position = rigidbody.position + platformDelta;
				break;
				case Mode.Navigation:
					navMeshAgent.Move (platformDelta);
				break;
			}
		}


		public virtual void PredictPosition (float time)
		// Move the target position with the target velocity and drag and gravity over the specified amount of time
		{
			float magnitude = Mathf.Max (0.0f, targetVelocity.magnitude - drag);
			if (magnitude > 0.0f)
			{
				targetPosition += (targetVelocity.normalized * magnitude + (Grounded ? Vector3.zero : ConstrainedGravity)) * time;
			}
		}


		protected virtual void OnDrawGizmos ()
		{
			if (!debug)
			{
				return;
			}

			if (mode == Mode.Physics)
			{
				Gizmos.color = Grounded ? Color.green : Color.red;
				Gizmos.DrawWireSphere (transform.position + transform.up * -groundedCheckOffset, groundedDistance);
			}

			const float kIndicatorScale = 2.0f;

			Gizmos.color = Color.red;
			Gizmos.DrawLine (transform.position, transform.position + targetVelocity.normalized * kIndicatorScale);

			Gizmos.color = new Color (1.0f, 0.3f, 0.3f);
			Gizmos.DrawLine (transform.position, transform.position + ConstrainedVelocity.normalized * kIndicatorScale);
		}


		public virtual void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo messageInfo)
		{
			targetPosition = Utility.Serialize (stream, targetPosition);
			targetRotation = Utility.Serialize (stream, targetRotation);
			targetVelocity = Utility.Serialize (stream, targetVelocity);
			stream.Serialize (ref drag);

			if (!stream.isWriting)
			{
				if (firstSync)
				// On first sync of remote players, set position and rotation directly
				{
					firstSync = false;
					transform.position = targetPosition;
					transform.rotation = targetRotation;
				}
				else
				{
					PredictPosition ((float)(Network.time - messageInfo.timestamp));
						// If we're reading back movement from another client, apply the predicted movement since message send to the target position
				}
			}
		}
	}
}
