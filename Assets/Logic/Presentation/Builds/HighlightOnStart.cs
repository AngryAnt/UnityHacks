using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityAssets;
#endif
using System.Collections;


public class HighlightOnStart : MonoBehaviour
{
	public Object asset;
	
	#if UNITY_EDITOR
	void Start ()
	{
		Selection.activeObject = asset;
		EditorUtility.FocusProjectWindow ();
		EditorGUIUtility.PingObject (asset);
	}
	#endif
}
