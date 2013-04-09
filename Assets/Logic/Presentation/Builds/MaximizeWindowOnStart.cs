using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityAssets;
#endif
using System.Collections;


public class MaximizeWindowOnStart : MonoBehaviour
{
	public string typeName;
	public bool maximize = true;

	
#if UNITY_EDITOR
	void Start ()
	{
		EditorWindow window = Utility.FindInstance<EditorWindow> (typeName);

		if (window == null)
		{
			Debug.LogError ("Could not find EditorWindow instance of type: " + typeName);
		}
		else
		{
			window.maximized = maximize;
		}
	}
#endif
}
