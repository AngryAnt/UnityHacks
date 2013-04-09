using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityAssets;
#endif
using System.Collections;


public class OpenScriptOnStart : MonoBehaviour
{
	public string typeName;

#if UNITY_EDITOR
	void Start ()
	{
		MonoScript script = Utility.FindScript (typeName);

		AssetDatabase.OpenAsset (script);
	}
#endif
}
