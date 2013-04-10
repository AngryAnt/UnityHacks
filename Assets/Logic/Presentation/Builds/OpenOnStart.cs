using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityAssets;
#endif
using System.Collections;


public class OpenOnStart : MonoBehaviour
{
	public Object asset;
	
#if UNITY_EDITOR
	void Start ()
	{
		AssetDatabase.OpenAsset (asset);
	}
#endif
}
