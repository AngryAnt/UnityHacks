using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class SightCamera : CameraBehaviour
	{
		public AnimationCurve blend = new AnimationCurve (
			new Keyframe (0.0f, 0.0f),
			new Keyframe (0.25f, 0.05f),
			new Keyframe (0.4f, 0.2f),
			new Keyframe (0.65f, 0.04f),
			new Keyframe (1.00f, 0.00f)
		);


		public float blendDelay = 4.0f, blendDuration = 10.0f;


		Vector3 GetRelaxOffset (Vector3 direction)
		{
			return camera.followOffset + direction * Utility.GetFieldOfViewRadius (camera.camera, camera.horizontalHinge.position) * 0.2f;
		}


		protected override void UpdateCamera ()
		{
			float interpolation = blend.Evaluate ((Time.time - camera.lastActivity - blendDelay) / blendDuration);
			Vector3 relaxOffset = GetRelaxOffset (camera.horizontalHinge.right * (Vector3.Dot (camera.followTarget.forward, camera.horizontalHinge.right) > 0.0f ? 1.0f : -1.0f));

			camera.targetOffset = Vector3.Lerp (camera.targetOffset, relaxOffset, interpolation);
			camera.targetVerticalRotation = Quaternion.Slerp (camera.targetVerticalRotation, Quaternion.AngleAxis (camera.verticalSpan[0].time + camera.VerticalSpan * 0.5f, Vector3.right), interpolation);

			base.UpdateCamera ();
		}
	}
}
