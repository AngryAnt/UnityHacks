using UnityEngine;
using System.Collections;


public class RotateOnStart : MonoBehaviour
{
	public Transform target, goal;
	public float time = 1.0f;

	
	IEnumerator Start ()
	{
		Quaternion original = target.rotation;
		float start = Time.time;

		while (Quaternion.Angle (target.rotation, goal.rotation) > 1.0f)
		{
			target.rotation = Quaternion.Slerp (original, goal.rotation, (Time.time - start) / time);

			yield return null;
		}

		target.rotation = goal.rotation;
	}
}
