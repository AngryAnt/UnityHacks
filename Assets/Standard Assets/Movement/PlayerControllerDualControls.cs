using UnityEngine;


namespace UnityAssets
{
	public class PlayerControllerDualControls : MovementController
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
				DualControls.Left.y * transform.forward * speed +
				DualControls.Left.x * transform.right * speed,
				speed
			);

			mover.targetRotation = mover.transform.rotation * Quaternion.AngleAxis (
				DualControls.Right.x * maxTurningAngle,
				mover.transform.up
			);
		}
	}
}
