using UnityEngine;
using System.Collections;


public class NewBehaviourScript : MonoBehaviour
{
	const float kSpeed = 30.0f;


	void Start ()
	{
		Debug.Log ("We're in!");
		#if SCRIPT_BUNDLE
			Debug.Log ("From a script bundle even.");
		#endif
	}


	void Update ()
	{
		float rotation = kSpeed * Time.deltaTime;
		transform.Rotate (new Vector3 (rotation, rotation, rotation));
	}
}
