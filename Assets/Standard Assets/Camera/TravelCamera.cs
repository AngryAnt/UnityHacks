#if UNITY_WEBPLAYER || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
	#define DC_CURSOR_LOCK
#endif

using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	[RequireComponent (typeof (FollowCamera))]
	[RequireComponent (typeof (OrbitCamera))]
	[RequireComponent (typeof (SightCamera))]
	public class TravelCamera : MonoBehaviour
	{
		new public DualHingeCamera camera;
		public float followDelay = 2.0f, followActivityLimit = 0.5f;


		FollowCamera followCamera;
		OrbitCamera orbitCamera;
		SightCamera sightCamera;


		void Reset ()
		{
			camera = GetComponent<DualHingeCamera> ();
			camera.verticalSpan =
				new AnimationCurve (new Keyframe (-20.0f, 0.0f), new Keyframe (-9.0f, 0.5f), new Keyframe (30.0f, 0.0f));
					// Mapping angle to move speed factor. Extremes are low and high clamp.
			camera.updateSpeed = 5;
		}


		void Awake ()
		{
			followCamera = GetComponent<FollowCamera> ();
			orbitCamera = GetComponent<OrbitCamera> ();
			sightCamera = GetComponent<SightCamera> ();

			SetActiveBehaviour (null);
		}


		void OnDisable ()
		{
			SetActiveBehaviour (null);
		}


		public void SetActiveBehaviour (CameraBehaviour behaviour)
		{
			followCamera.enabled = orbitCamera.enabled = sightCamera.enabled = false;

			if (behaviour != null)
			{
				behaviour.enabled = true;
			}
		}


		void Update ()
		{
			float timeSinceActivity = Time.time - camera.lastActivity;

			if (timeSinceActivity > sightCamera.blendDelay)
			{
				SetActiveBehaviour (sightCamera);
			}
			else
			{
				float timeSinceCameraActivity = Time.time - camera.lastCameraActivity;

				if (timeSinceCameraActivity > followDelay && timeSinceActivity < followActivityLimit)
				{
					SetActiveBehaviour (followCamera);
				}
				else
				{
					SetActiveBehaviour (orbitCamera);
				}
			}
		}
	}
}
