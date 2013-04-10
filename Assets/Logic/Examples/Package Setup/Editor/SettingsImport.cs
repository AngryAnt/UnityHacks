using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// TODO: Get this back in the namespace once AssetDatabase.GetAssetPath (MonoScript.FromScriptableObject (asset)) works with namespaces
using UnityAssets;
//namespace UnityAssets
//{
	public class SettingsImportPostprocessor : AssetPostprocessor
	{
		static void OnPostprocessAllAssets (string[] imported, string[] deleted, string[] moved, string[] movedFrom)
		{
			foreach (string path in imported)
			{
				if (AssetDatabase.LoadMainAssetAtPath (path) is SettingsImport)
				{
					SettingsImport.Import ();
					return;
				}
			}
		}
	}


	[InitializeOnLoad]
	public class SettingsImport : ScriptableObject
	{
		const string kUserLayerPrefix = "User Layer ", kLastImportFailPref = "Last settings import fail";


		public bool forceImport = false;
		public string
			abortTitle = "Settings import aborted",
			abortDescription = "When attempting to import additional settings required by this package, an existing setting was found colliding with one being imported.\n\nIf you wish to continue, please change your project to use a different setting, clear the name of the setting in question and resume import from the Assets menu.",
			abortOk = "OK";
		public string[] tags, layers = new string[32 - 8];
		public List<AxisDefinition> axes = new List<AxisDefinition> ();


		static SettingsImport ()
		{
			EditorApplication.delayCall += OnDelayedImport;
		}


		static void OnDelayedImport ()
		{
			EditorApplication.delayCall -= OnDelayedImport;
			Import ();
		}


		static SerializedObject GetTagManager ()
		{
			return new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset")[0]);
		}


		[MenuItem ("Assets/Stored Settings/Export")]
		public static void Export ()
		{
			SettingsImport asset = CreateInstance<SettingsImport> ();
			SerializedObject tagManager = GetTagManager ();

			SerializedProperty tags = tagManager.FindProperty ("tags");
			asset.tags = new string[tags.arraySize];
			for (int i = 0; i < tags.arraySize; ++i)
			{
				asset.tags[i] = tags.GetArrayElementAtIndex (i).stringValue;
			}

			for (int i = 8; i < 32; ++i)
			{
				asset.layers[i - 8] = tagManager.FindProperty (kUserLayerPrefix + i).stringValue;
			}

			asset.axes = InputInterface.GetAxes ();

			string path = AssetDatabase.GetAssetPath (MonoScript.FromScriptableObject (asset));
			Debug.Log (path);
			path = path.Substring (0, path.LastIndexOf ('/')) + "/SettingsExport.asset";

			AssetDatabase.CreateAsset (asset, path);
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath (path);
		}


		[MenuItem ("Assets/Stored Settings/Import")]
		public static void ImportManually ()
		{
			Import (true);
		}


		public static void Import (bool manual = false)
		{
			Object[] assets = Resources.FindObjectsOfTypeAll (typeof (SettingsImport));

			if (assets.Length < 1)
			{
				if (manual)
				{
					Debug.LogError ("Did not find a settings export asset for importing");
				}
				return;
			}

			SettingsImport asset = (SettingsImport)assets[0];
			SerializedObject tagManager = GetTagManager ();

			Debug.Log ("Importing settings...", asset);

			SerializedProperty tags = tagManager.FindProperty ("tags");
			for (int i = 0; i < asset.tags.Length; ++i)
			{
				if (i > tags.arraySize - 1)
				{
					tags.InsertArrayElementAtIndex (i);
					SerializedProperty userTag = tags.GetArrayElementAtIndex (i);
					userTag.stringValue = asset.tags[i];
				}
				else
				{
					SerializedProperty userTag = tags.GetArrayElementAtIndex (i);

					if (asset.Collision (userTag.stringValue, asset.tags[i], "Tag", manual))
					{
						tagManager.ApplyModifiedProperties ();
						return;
					}

					userTag.stringValue = asset.tags[i];
				}
			}

			for (int i = 0; i < asset.layers.Length; ++i)
			{
				SerializedProperty userLayer = tagManager.FindProperty (kUserLayerPrefix + (i + 8));

				if (asset.Collision (userLayer.stringValue, asset.layers[i], "Layer", manual))
				{
					tagManager.ApplyModifiedProperties ();
					return;
				}
				
				userLayer.stringValue = asset.layers[i];
			}

			List<AxisDefinition> axes = InputInterface.GetAxes ();
			foreach (AxisDefinition axis in asset.axes)
			{
				if (axes.Contains (axis))
				{
					continue;
				}

				AxisDefinition collision = axes.Where (other => axis.InterfaceEquals (other)).FirstOrDefault ();
				
				if (collision != null)
				{
					InputInterface.SaveAxis (axis, axes.IndexOf (collision));
					axes = InputInterface.GetAxes ();
					continue;
				}

				InputInterface.AddAxis (axis);
				axes = InputInterface.GetAxes ();
			}


			tagManager.ApplyModifiedProperties ();
		}


		bool Collision (string existingValue, string incomingValue, string typeName, bool manual = false)
		{
			if (!forceImport && !string.IsNullOrEmpty (existingValue) && !string.IsNullOrEmpty (incomingValue) && existingValue != incomingValue)
			{
				HandleCollision<string> (existingValue, typeName, manual);

				return true;
			}

			return false;
		}


		void HandleCollision<T> (T existingValue, string typeName, bool manual)
		{
			if (manual || EditorPrefs.GetString (kLastImportFailPref) != (typeName + ":" + existingValue))
			{
				EditorUtility.DisplayDialog (
					abortTitle,
					string.Format ("{0}\n\n{1}: {2}", abortDescription, typeName, existingValue),
					abortOk
					);
				
				Selection.activeObject = GetTagManager ().targetObject;
			}
			
			EditorPrefs.SetString (kLastImportFailPref, typeName + ":" + existingValue);
		}
	}
//}
