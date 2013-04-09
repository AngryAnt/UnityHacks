using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityAssets;
#endif
using System.Collections;


public class HighlightScriptOnStart : MonoBehaviour
{
	public string typeName;

#if UNITY_EDITOR
	void Start ()
	{
		MonoScript script = Utility.FindScript (typeName);

		if (script == null)
		{
			return;
		}

		Selection.activeObject = script;
		EditorUtility.FocusProjectWindow ();
		EditorGUIUtility.PingObject (script);
	}
#endif
}
