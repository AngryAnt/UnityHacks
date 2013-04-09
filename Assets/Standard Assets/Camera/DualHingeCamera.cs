using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class DualHingeCamera : MonoBehaviour
	{
		new public Camera camera;
		public Transform horizontalHinge, verticalHinge, followTarget;
		public Vector3 followOffset = new Vector3 (0.0f, 1.8f, 0.0f);
		public float updateSpeed = 1.0f;
		public AnimationCurve
			verticalSpan =
				new AnimationCurve (new Keyframe (-20.0f, 1.0f), new Keyframe (30.0f, 1.0f));
		[System.NonSerialized]
		public float lastActivity, lastCameraActivity;
		[System.NonSerialized]
		public Quaternion targetHorizontalRotation, targetVerticalRotation;
		[System.NonSerialized]
		public Vector3 targetOffset;


		public float VerticalAngle
		// The vertical camera angle to the back vector
		{
			get
			{
				return Vector3.Angle (verticalHinge.forward * -1.0f, horizontalHinge.forward * -1.0f) *
					(Vector3.Dot (verticalHinge.forward * -1.0f, horizontalHinge.up) > 0.0f ? 1.0f : -1.0f);
			}
		}


		public float VerticalSpan
		// The vertical span, in degrees, of the camera
		{
			get
			{
				return verticalSpan[verticalSpan.length - 1].time - verticalSpan[0].time;
			}
		}


		public float VerticalPosition
		// Where in the span is the camera, normalized
		{
			get
			{
				return (VerticalAngle - verticalSpan[0].time) / VerticalSpan;
			}
		}


		void Reset ()
		{
			camera = GetComponentInChildren<Camera> ();
			verticalHinge = camera == null ? null : camera.transform.parent;
			horizontalHinge = verticalHinge == null ? null : verticalHinge.parent;
		}


		protected virtual void OnEnable ()
		{
			if (!Utility.RequireSet (camera, "Camera", this) ||
				!Utility.RequireSet (horizontalHinge, "Horizontal hinge", this) ||
				!Utility.RequireSet (verticalHinge, "Vertical hinge", this) ||
				!Utility.RequireSet (followTarget, "Follow target", this) ||
				!Utility.Require (verticalHinge.parent == horizontalHinge, "Vertical hinge must be parented to the horizontal hinge", this) ||
				!Utility.Require (verticalHinge.localPosition == Vector3.zero, "Horizontal hinge must have zero offset from the horizontal hinge", this))
			{
				return;
			}

			targetHorizontalRotation = horizontalHinge.rotation;
			targetVerticalRotation = verticalHinge.localRotation;
			targetOffset = followOffset;
			lastCameraActivity = lastActivity = Time.time;
		}


		public void UpdateCamera ()
		{
			horizontalHinge.position = Vector3.Lerp (horizontalHinge.position, followTarget.position + targetOffset, Time.deltaTime * updateSpeed);
			verticalHinge.localPosition = Vector3.zero;

			horizontalHinge.rotation = Quaternion.Slerp (horizontalHinge.rotation, targetHorizontalRotation, Time.deltaTime * updateSpeed);

			// Scale update speed by clamp curve

			float verticalUpdateSpeed = updateSpeed;
			if (VerticalPosition > 0.5f)
			// If up
			{
				if (
					Vector3.Dot (targetVerticalRotation * Vector3.up, Vector3.up) <
					Vector3.Dot (verticalHinge.localRotation * Vector3.up, Vector3.up)
				)
				// And going further up
				{
					verticalUpdateSpeed = updateSpeed * verticalSpan.Evaluate (VerticalAngle);
				}
			}
			else
			// If down
			{
				if (
					Vector3.Dot (targetVerticalRotation * -Vector3.up, -Vector3.up) <
					Vector3.Dot (verticalHinge.localRotation * -Vector3.up, -Vector3.up)
				)
				// And going further down
				{
					verticalUpdateSpeed = updateSpeed * verticalSpan.Evaluate (VerticalAngle);
				}
			}

			verticalHinge.localRotation = Quaternion.Slerp (verticalHinge.localRotation, targetVerticalRotation, Time.deltaTime * verticalUpdateSpeed);

			if (VerticalAngle < verticalSpan[0].time)
			// Clamp low
			{
				verticalHinge.localRotation = Quaternion.AngleAxis (verticalSpan[0].time, Vector3.right);
				targetVerticalRotation = verticalHinge.localRotation;
			}
			else if (VerticalAngle > verticalSpan[verticalSpan.length - 1].time)
			// Clamp high
			{
				verticalHinge.localRotation = Quaternion.AngleAxis (verticalSpan[verticalSpan.length - 1].time, Vector3.right);
				targetVerticalRotation = verticalHinge.localRotation;
			}
		}


		public void ActivityPing ()
		{
			lastActivity = Time.time;
		}
	}
}
