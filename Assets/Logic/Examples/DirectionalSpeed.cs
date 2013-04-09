using UnityEngine;
using System.Collections;


public class DirectionalSpeed : MonoBehaviour
{
	public AnimationCurve speedScale;


	float speed = 5.0f;


	public Vector3 Direction
	{
		get
		{
			Vector3 direction = new Vector3 (Input.GetAxis ("Horizontal"), 0.0f, Input.GetAxis ("Vertical"));

			if (direction.magnitude > 0.0f)
			{
				return direction.normalized;
			}

			return Vector3.zero;
		}
	}


	void Update ()
	{
		transform.Translate (
			Direction * speed * Time.deltaTime *
				speedScale.Evaluate (Vector3.Angle (transform.forward, Direction) / 180.0f)
		);
	}
}
