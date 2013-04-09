using UnityEngine;
using System.Collections;


public class MoveOnStart : MonoBehaviour
{
	public Transform target, goal;
	public float time = 1.0f;
	
	
	IEnumerator Start ()
	{
		Vector3 original = target.position;
		float start = Time.time;
		
		while ((target.position - goal.position).magnitude > 0.1f)
		{
			target.position = Vector3.Lerp (original, goal.position, (Time.time - start) / time);
			
			yield return null;
		}
		
		target.position = goal.position;
	}
}
