using UnityEngine;
using System.Collections;

namespace UnityAssets
{
	public class RightStickCameraController : MonoBehaviour
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
					if (DualControls.Right.Value.magnitude > 0.0f)
					{
						behaviour.Move (DualControls.Right.Value);
					}
					else if (DualControls.Interaction)
					{
						behaviour.camera.ActivityPing ();
					}
				}
			}
		}
	}
}
