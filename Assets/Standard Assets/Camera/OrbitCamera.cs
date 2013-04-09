using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class OrbitCamera : CameraBehaviour
	{
		public float moveSpeed = 400.0f;


		protected override void UpdateCamera ()
		{
			camera.targetOffset = Vector3.Lerp (camera.targetOffset, camera.followOffset, Time.deltaTime);

			base.UpdateCamera ();
		}


		protected override void MoveCamera (Vector2 direction)
		{
			float verticalMoveSpeed = moveSpeed;

			if ((direction.y > 0 && camera.VerticalPosition > 0.5f) || (direction.y < 0 && camera.VerticalPosition < 0.5f))
			// Apply vertical speed damping if we're moving further in the direction we already are in
			{
				verticalMoveSpeed *= camera.verticalSpan.Evaluate (camera.VerticalAngle);
			}

			camera.targetHorizontalRotation *= Quaternion.AngleAxis (direction.x * moveSpeed * Time.deltaTime, Vector3.up);
			camera.targetVerticalRotation *= Quaternion.AngleAxis (direction.y * verticalMoveSpeed * Time.deltaTime, Vector3.right);

			base.MoveCamera (direction);
		}
	}
}
