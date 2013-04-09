using UnityEngine;
using System.Collections;

namespace UnityAssets
{
	public class MouseCameraController : MonoBehaviour
	{
		public CameraBehaviour[] cameraBehaviours;


		void OnEnable ()
		{
			if (cameraBehaviours.Length == 0)
			{
				cameraBehaviours = (CameraBehaviour[])FindObjectsOfTypeAll (typeof (CameraBehaviour));
			}
		}


		void Update ()
		{
			foreach (CameraBehaviour behaviour in cameraBehaviours)
			{
				if (behaviour.enabled)
				{
					Vector2 movement = new Vector2 (Input.GetAxis ("Mouse X"), Input.GetAxis ("Mouse Y"));

					if (movement.sqrMagnitude > 0.0f)
					{
						behaviour.Move (movement);
					}
					else if (Input.anyKey)
					{
						behaviour.camera.ActivityPing ();
					}
				}
			}
		}
	}
}
