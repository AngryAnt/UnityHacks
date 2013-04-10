using UnityEngine;
using System.Collections;
using System;
using CrossPlatformTools;


public class ScriptBundleTest : MonoBehaviour
{
	void Start ()
	{
		BundleManager.LoadType ("BestScriptsEver", "NewBehaviourScript", OnReceiveType);
	}


	void OnReceiveType (string requestedTypeName, Type type)
	{
		gameObject.AddComponent (type);
	}
}
