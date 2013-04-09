using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
using System.Collections;


public class ResetShowOnStart : MonoBehaviour
{
	public bool exitWhenDone = true;


	void Start ()
	{
		PlayerPrefs.SetInt (Control.kLastLevelPref, -1);

#if UNITY_EDITOR
		EditorApplication.isPlaying = !exitWhenDone;
#endif
	}
}
