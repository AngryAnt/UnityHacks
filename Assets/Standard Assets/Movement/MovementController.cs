using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class MovementController : MonoBehaviour
	{
		public Mover mover;


		void Awake ()
		{
			if (mover == null)
			{
				mover = GetComponent<Mover> ();
			}
		}


		protected virtual void OnEnable ()
		{
			mover.ApplyPosition = false;
				// We're controlling the mover - it's not applying some synced position
		}


		protected virtual void OnDisable ()
		{
			mover.ApplyPosition = true;
				// We're no longer controlling the mover - apply any synced position
		}
	}
}
