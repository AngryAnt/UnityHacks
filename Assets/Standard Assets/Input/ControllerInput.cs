using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class ControllerInput : MonoBehaviour
	{
		protected Controller controller;


		int ControllerIndex
		{
			get
			{
				return controller == null ? 0 : controller.Index;
			}
		}


		void OnGUI ()
		{
			GUILayout.BeginHorizontal ();
				GUILayout.BeginVertical ();
					GUILayout.Box ("All joysticks");
					for (int i = 1; i <= Controller.Count; i++)
					{
						if (GUILayout.Toggle (ControllerIndex == i, string.Format ("{0}: {1}", i, Controller.ControllerType (i)), GUI.skin.button) && ControllerIndex != i)
						{
							controller = Controller.Get (i);
						}
					}
				GUILayout.EndVertical ();

				if (controller != null)
				{
					GUILayout.BeginVertical ();
						GUILayout.Box ("Buttons");
						for (int i = 0; i < 20; i++)
						{
							GUILayout.Label (string.Format ("{0}: {1}", i, controller.GetButton (i)));
						}
					GUILayout.EndVertical ();

					GUILayout.BeginVertical ();
						GUILayout.Box (string.Format ("Mapping: {0} {1}", controller, controller.Index));
						controller.OnGUI ();
					GUILayout.EndVertical ();
				}
			GUILayout.EndHorizontal ();
		}
	}
}
