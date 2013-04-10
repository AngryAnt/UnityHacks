using UnityEngine;
using UnityEditor;


namespace UnityAssets
{
	[CustomEditor (typeof (SettingsImport))]
	public class SettingsImportInspector : Editor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Import", EditorStyles.miniButtonLeft))
				{
					SettingsImport.Import (true);
				}

				if (GUILayout.Button ("Re-export", EditorStyles.miniButtonRight))
				{
					SettingsImport.Export ();
				}
			GUILayout.EndHorizontal ();

			EditorGUIUtility.LookLikeInspector ();
			DrawDefaultInspector ();
		}
	}
}
