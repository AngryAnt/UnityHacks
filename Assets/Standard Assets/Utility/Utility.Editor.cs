using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System;
using System.Linq;

namespace UnityAssets
{
	public partial class Utility
	{
		public static T FindInstance<T> (string typeName) where T : class
		{
			Type type = null, baseType = typeof (T);
			T instance = null;
			
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies ())
			{
				type = assembly.GetType (typeName);
				
				if (type != null && type.IsSubclassOf (baseType))
				{
					instance = Resources.FindObjectsOfTypeAll (type).FirstOrDefault () as T;

					if (instance != null)
					{
						break;
					}
				}
			}
			
			return instance;
		}


		public static MonoScript FindScript (string typeName)
		{
			MonoScript script = null;
			
			Type type = null;
			object instance = null;
			
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies ())
			{
				type = assembly.GetType (typeName);
				
				if (type != null)
				{
					instance = Resources.FindObjectsOfTypeAll (type).FirstOrDefault ();

					if (instance != null)
					{
						break;
					}
				}
			}
			
			if (instance == null)
			{
				Debug.LogError ("Could not find script for type: " + typeName);
				return null;
			}
			else if (type.IsSubclassOf (typeof (MonoBehaviour)))
			{
				script = MonoScript.FromMonoBehaviour ((MonoBehaviour)instance);
			}
			else if (type.IsSubclassOf (typeof (ScriptableObject)))
			{
				script = MonoScript.FromScriptableObject ((ScriptableObject)instance);
			}
			else
			{
				Debug.LogError (string.Format ("Can only find MonoBehaviours or ScriptableObjects. {0} is neither.", typeName));
			}

			return script;
		}
	}
}
#endif
