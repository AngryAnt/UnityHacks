using UnityEngine;


namespace UnityAssets
{
	public class PlayerControllerAxis : MovementController
	{
		public float speed = 5.0f, maxTurningAngle = 90.0f;


		public virtual void Update ()
		{
			if (!mover.Grounded)
			{
				mover.targetVelocity = Vector3.zero;
				return;
			}

			mover.targetVelocity = Vector3.ClampMagnitude (
				Input.GetAxis ("Vertical") * transform.forward * speed +
				Input.GetAxis ("Horizontal") * transform.right * speed,
				speed
			);

			mover.targetRotation = mover.transform.rotation * Quaternion.AngleAxis (
				Input.GetAxis ("Mouse X") * maxTurningAngle,
				mover.transform.up
			);
		}
	}
}
