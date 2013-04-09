using UnityEngine;


namespace UnityAssets
{
	public class MouseLock : MonoBehaviour
	{
		void Update ()
		{
			Screen.lockCursor = true;
		}


		void OnDisable ()
		{
			Screen.lockCursor = false;
		}
	}
}
