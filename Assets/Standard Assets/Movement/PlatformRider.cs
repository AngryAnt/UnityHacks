using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class PlatformRider : MonoBehaviour
	{
		public virtual bool BoardPlatform (Platform platform)
		// Attempting to board a platform - override to add cases for denying boarding
		{
			return true;
		}


		public virtual bool LeavePlatform (Platform platform)
		// Attempting to leave the platform - override to add cases for denying departure
		{
			return true;
		}


		public virtual void UpdatePlatform (Vector3 platformDelta)
		// Apply the delta movement of a platform to the rider
		{
			transform.position += platformDelta;
		}
	}
}
