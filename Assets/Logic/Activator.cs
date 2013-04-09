using UnityEngine;
using System.Collections;


public class Activator : MonoBehaviour
{
	public GameObject[] targets;
	public bool activeState = true, inverseOnAwake = true;


	void Awake ()
	{
		if (!inverseOnAwake)
		{
			return;
		}

		foreach (GameObject target in targets)
		{
			target.SetActive (!activeState);
		}
	}


	void Start ()
	{
		foreach (GameObject target in targets)
		{
			target.SetActive (activeState);
		}
	}
}
