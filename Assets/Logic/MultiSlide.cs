using UnityEngine;
using System.Collections;


public class MultiSlide : MonoBehaviour
{
	public int duration = 2;


	int origin = -1;


	void Awake ()
	{
		DontDestroyOnLoad (gameObject);

		if (origin == -1)
		{
			origin = Application.loadedLevel;
		}
	}


	void OnLevelWasLoaded (int level)
	{
		if (level > origin + duration - 1)
		{
			Destroy (gameObject);
			return;
		}

		// TODO: Destroy if out of bounds
		// TODO: Destroy if duplicate found
	}
}
