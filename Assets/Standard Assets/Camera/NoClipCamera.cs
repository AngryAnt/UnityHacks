using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class NoClipCamera : MonoBehaviour
	{
		new public Camera camera;
		public Collider followTarget;
		public Vector3 followOffset = new Vector3 (0.0f, 1.8f, 0.0f);
		public float optimalDistance = 6.0f, minimumDistance = 1.0f;
		public LayerMask obstacleLayers;
		public AnimationCurve
			cameraZoomFactor =
				new AnimationCurve (new Keyframe (-1.0f, 1.0f), new Keyframe (1.0f, 5.0f));


		float targetDistance;


		float ViewRadius
		// The minimum clear radius between the camera and the target
		{
			get
			{
				return Mathf.Min (Mathf.Max (followTarget.bounds.extents.x, followTarget.bounds.extents.z) * 2.0f, Utility.GetFieldOfViewRadius (camera, followTarget.transform.position));
			}
		}


		void OnEnable ()
		{
			if (camera == null)
			{
				camera = GetComponentInChildren<Camera> ();
			}

			if (!Utility.RequireSet (camera, "Camera", this))
			{
				return;
			}

			ValidateCamera ();
		}


		public void ValidateCamera ()
		{
			Vector3 inverseLineOfSight = camera.transform.position - (followTarget.transform.position + followOffset);

			RaycastHit hit;
			if (Physics.SphereCast (followTarget.transform.position + followOffset, ViewRadius, inverseLineOfSight, out hit, optimalDistance, obstacleLayers))
			{
				targetDistance = Mathf.Max (minimumDistance, Mathf.Min ((hit.point - (followTarget.transform.position + followOffset)).magnitude - ViewRadius, optimalDistance));
			}
			else
			{
				targetDistance = optimalDistance;
			}
		}


		void FixedUpdate ()
		{
			ValidateCamera ();
		}


		void Update ()
		{
			float speed = cameraZoomFactor.Evaluate (
				(camera.transform.localPosition.magnitude - targetDistance) / targetDistance
			);

			camera.transform.localPosition = Vector3.Lerp (
				camera.transform.localPosition,
				camera.transform.localPosition.normalized * targetDistance,
				Time.deltaTime * speed
			);
		}
	}
}
