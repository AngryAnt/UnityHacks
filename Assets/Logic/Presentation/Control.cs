using UnityEngine;
using System.Collections;


public class Control : MonoBehaviour
{
	public const string kLastLevelPref = "Loaded level";

	
	public GameObject[] build;
	public bool autoBuildFirst = false;


	int buildIndex = -1;


	public int BuildIndex
	{
		get
		{
			return buildIndex;
		}
	}


	void Awake ()
	{
		int targetLevel = PlayerPrefs.GetInt (kLastLevelPref, -1);
		if (targetLevel != -1 && targetLevel != Application.loadedLevel)
		{
			LoadLevel (targetLevel);
			return;
		}

		foreach (GameObject item in build)
		{
			item.SetActive (false);
		}
	}


	void Start ()
	{
		if (autoBuildFirst && build.Length > 0)
		{
			Next ();
		}
	}


	public void Previous ()
	{
		if (Application.loadedLevel > 0)
		{
			LoadLevel (Application.loadedLevel - 1);
		}
#if UNITY_ANDROID
		else
		{
			LoadLevel (Application.levelCount - 1);
		}
#endif
	}


	public void Next ()
	{
		if (build.Length > ++buildIndex)
		{
			build[buildIndex].SetActive (true);
			return;
		}

		if (Application.loadedLevel < Application.levelCount - 1)
		{
			LoadLevel (Application.loadedLevel + 1);
		}
	}


	void LoadLevel (int index)
	{
		PlayerPrefs.SetInt (kLastLevelPref, index);
		Application.LoadLevel (index);
	}


	void OnGesture (Gesture gesture)
	{
		switch (gesture.type)
		{
			case GestureType.Swipe:
				if (Vector2.Dot (gesture.direction, new Vector2 (1.0f, 0.0f)) > 0.8f)
				{
					Previous ();
				}
				else if (Vector2.Dot (gesture.direction, new Vector2 (-1.0f, 0.0f)) > 0.8f)
				{
					Next ();
				}
			break;
		}
	}


	void Update ()
	{
		if (Input.GetButtonDown ("Previous"))
		{
			Previous ();
		}
		else if (Input.GetButtonDown ("Next"))
		{
			Next ();
		}
	}
}
