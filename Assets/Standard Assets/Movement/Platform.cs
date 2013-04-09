using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace UnityAssets
{
	public class Platform : MonoBehaviour
	{
		const float kExitReCheckDelay = 0.3f;


		List<PlatformRider> passengers = new List<PlatformRider> ();
		Vector3 lastPosition;
		Collider[] triggers;


		void OnEnable ()
		{
			lastPosition = transform.position;
			triggers = GetComponentsInChildren<Collider> ().Where (collider => collider.isTrigger).ToArray ();
		}


		public void BoardPlatform (PlatformRider passenger)
		{
			passengers.Add (passenger);
		}


		public void LeavePlatform (PlatformRider passenger)
		{
			passengers.Remove (passenger);
		}


		void OnTriggerStay (Collider other)
		{
			PlatformRider passenger = other.transform.root.GetComponentInChildren<PlatformRider> ();

			if (passenger == null || passengers.Contains (passenger))
			{
				return;
			}

			if (passenger.BoardPlatform (this))
			{
				BoardPlatform (passenger);
			}
		}


		bool IntersectingTrigger (Collider other)
		{
			foreach (Collider collider in triggers)
			{
				if (collider.bounds.Intersects (other.bounds))
				{
					return true;
				}
			}

			return false;
		}


		bool ContainsPoint (Vector3 point)
		{
			foreach (Collider collider in triggers)
			{
				if (collider.bounds.Contains (point))
				{
					return true;
				}
			}

			return false;
		}


		void LateUpdate ()
		{
			Vector3 delta = transform.position - lastPosition;

			if (delta != Vector3.zero)
			{
				for (int i = 0; i < passengers.Count;)
				{
					PlatformRider passenger = passengers[i];
					bool didUpdate = false;

					foreach (Collider collider in passenger.transform.root.GetComponentsInChildren<Collider> ())
					{
						if (
							ContainsPoint (passenger.transform.position) ||
							(Vector3.Dot (passenger.transform.position - transform.position, transform.up) > 0.0f && IntersectingTrigger (collider))
						)
						{
							passenger.UpdatePlatform (delta);
							didUpdate = true;
							break;
						}
					}

					if (!didUpdate && passenger.LeavePlatform (this))
					{
						LeavePlatform (passenger);
					}
					else
					{
						i++;
					}
				}
			}

			lastPosition = transform.position;
		}
	}
}
