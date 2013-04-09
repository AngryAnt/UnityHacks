using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class ObserverCamera : CameraBehaviour
	{
		public Vector3 point;
		public Transform target;
		public float observerDistance = 1.0f;
		public float speed = 1.0f;


		Vector3 GetObservationOffset (Vector3 direction)
		{
			return camera.followOffset + direction * Utility.GetFieldOfViewRadius (camera.camera, camera.horizontalHinge.position) * 0.2f;
		}


		protected override void UpdateCamera ()
		{
			if (target != null)
			{
				point = target.position;
			}

			Vector3 direction = point - camera.transform.position;

			Vector3 horizontalForward = direction;
			horizontalForward.Scale (Vector3.one - Vector3.up);
			camera.targetHorizontalRotation = Quaternion.LookRotation (horizontalForward);

			float verticalAngle = Vector3.Angle (horizontalForward, direction) * (Vector3.Dot (direction - horizontalForward, Vector3.up) > 0 ? -1.0f : 1.0f);
			camera.targetVerticalRotation = Quaternion.AngleAxis (verticalAngle, Vector3.right);

			camera.targetOffset = GetObservationOffset (camera.horizontalHinge.right * (Vector3.Dot (camera.followTarget.forward, camera.horizontalHinge.right) > 0.0f ? 1.0f : -1.0f));

			camera.transform.localPosition = Vector3.Lerp (
				camera.transform.localPosition,
				camera.transform.localPosition.normalized * observerDistance,
				Time.deltaTime * speed
			);

			base.UpdateCamera ();
		}
	}
}
