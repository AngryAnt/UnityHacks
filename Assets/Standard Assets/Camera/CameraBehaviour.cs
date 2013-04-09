using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public abstract class CameraBehaviour : MonoBehaviour
	{
		new public DualHingeCamera camera;


		protected virtual void Reset ()
		{
			camera = GetComponent<DualHingeCamera> ();
		}


		protected virtual void LateUpdate ()
		{
			UpdateCamera ();
		}


		protected virtual void UpdateCamera ()
		{
			camera.UpdateCamera ();
		}


		protected virtual void MoveCamera (Vector2 direction)
		{}


		public void Move (Vector2 direction)
		{
			MoveCamera (direction);
			camera.lastCameraActivity = Time.time;
			camera.ActivityPing ();
		}
	}
}
