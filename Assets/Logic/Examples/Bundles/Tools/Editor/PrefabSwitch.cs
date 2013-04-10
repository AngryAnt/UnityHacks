using UnityEngine;
using UnityEditor;
using System.Collections;


namespace CrossPlatformTools
{
	public class PrefabSwitch
	{
		enum PrefabLevelOfDetail
		{
			Low,
			High
		}


		[MenuItem ("Assets/PrefabSwitch/High")]
		static void SetLevelHigh ()
		{
			SetLevel (PrefabLevelOfDetail.High);
		}


		[MenuItem ("Assets/PrefabSwitch/Low")]
		static void SetLevelLow ()
		{
			SetLevel (PrefabLevelOfDetail.Low);
		}


		static void SetLevel (PrefabLevelOfDetail level)
		{
			foreach (string prefabPath in GetLODPrefabs ())
			{
				ReplacePrefab (
					prefabPath + "/" + level.ToString () + ".prefab",
					prefabPath + "/Current.prefab"
				);
			}
		}


		public static void ReplacePrefab (string fromPath, string toPath)
		{
			ReplacePrefab (
				AssetDatabase.LoadMainAssetAtPath (fromPath),
				AssetDatabase.LoadMainAssetAtPath (toPath)
			);
		}


		public static void ReplacePrefab (Object from, Object to)
		{
			PrefabUtility.ReplacePrefab (
				(GameObject)from,
				to,
				ReplacePrefabOptions.ReplaceNameBased
			);
		}


		static string[] GetLODPrefabs ()
		{
			return new string[] {"Assets/Prefabs/AlienVillager.lodPrefab"};
		}
	}
}
