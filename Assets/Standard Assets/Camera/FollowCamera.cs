using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class FollowCamera : CameraBehaviour
	{
		public AnimationCurve
			followSpeed =
				new AnimationCurve (new Keyframe (0.0f, 0.0f), new Keyframe (0.1f, 2.0f), new Keyframe (1.0f, 1.0f)),
			diveSpeed =
				new AnimationCurve (new Keyframe (0.0f, 1.0f), new Keyframe (0.1f, 2.0f), new Keyframe (1.0f, 1.0f));


		protected override void UpdateCamera ()
		{
			Vector3 targetForward = camera.followTarget.forward;
			targetForward = new Vector3 (targetForward.x, 0.0f, targetForward.z).normalized;
			float interpolation = Time.deltaTime * followSpeed.Evaluate (Vector3.Angle (targetForward, camera.horizontalHinge.forward) / 180.0f);

			camera.targetHorizontalRotation = Quaternion.Slerp (
				camera.targetHorizontalRotation,
				Quaternion.LookRotation (targetForward, Vector3.up),
				interpolation
			);

			float verticalOffset = (camera.followTarget.position + camera.targetOffset).y - camera.horizontalHinge.position.y;
			verticalOffset *= -100.0f;
			interpolation = Time.deltaTime * diveSpeed.Evaluate (Mathf.Abs (camera.VerticalPosition - 0.5f));

			camera.targetVerticalRotation = Quaternion.Slerp (
				camera.targetVerticalRotation,
				Quaternion.AngleAxis ((camera.verticalSpan[0].time + camera.VerticalSpan * 0.5f) + verticalOffset, Vector3.right),
				interpolation
			);

			camera.targetOffset = Vector3.Lerp (camera.targetOffset, camera.followOffset, Time.deltaTime);

			base.UpdateCamera ();
		}
	}
}
