using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CrossPlatformTools
{
	public class PrefabBundle
	{
		[MenuItem ("Assets/PrefabBundle")]
		public static void BuildBundles ()
		{
			foreach (string bundlePath in GetPrefabBundles ())
			{
				BuildBundle (bundlePath);
			}
		}


		public static bool BuildBundle (string path)
		// TODO: Hold prefab paths in its own list so you only load the prefabs once
		{
			string manifestPath = path + "/" + "Manifest.asset";

			if (AssetDatabase.LoadMainAssetAtPath (manifestPath) != null)
			// Bail if manifest is found. Don't want to build a bundle twice.
			{
				Debug.LogError (string.Format (
					"Bundle at {0} already built. Found manifest.",
					path
				));

				return false;
			}

			// TODO: Bail if a non-prefab asset from this bundle is referenced from outside of it

			List<string> prefabs = new List<string> (ListPrefabs (path));
				// TODO: Remove all prefabs not referenced outside of the bundle

			List<BundledPrefab> bundledPrefabs = new List<BundledPrefab> ();
			PrefabBundleManifest manifest = PrefabBundleManifest.Create ();

			foreach (string prefab in prefabs)
			{
				bundledPrefabs.Add (manifest.Add (AssetDatabase.LoadMainAssetAtPath (prefab)));
			}

			AssetDatabase.CreateAsset (manifest, manifestPath);

			AssetDatabase.Refresh ();

			List<string> allAssetPaths = ListAllAssets (path);

			BuildPipeline.BuildAssetBundle (
				AssetDatabase.LoadMainAssetAtPath (manifestPath),
				allAssetPaths.ConvertAll (
					assetPath => AssetDatabase.LoadMainAssetAtPath (assetPath)
				).ToArray (),
				Application.dataPath + "/../" + manifest.ID + ".unity3d",
				BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies,
				EditorUserBuildSettings.activeBuildTarget
			);

			foreach (string prefab in prefabs)
			{
				AssetDatabase.CopyAsset (
					prefab,
					prefab.Substring (0, prefab.Length - "prefab".Length) + "original.prefab"
				);
			}

			AssetDatabase.Refresh ();

			for (int i = 0; i < prefabs.Count; i++)
			{
				PrefabUtility.ReplacePrefab (
					bundledPrefabs[i].gameObject,
					AssetDatabase.LoadMainAssetAtPath (prefabs[i]),
					ReplacePrefabOptions.ReplaceNameBased
				);

				Object.DestroyImmediate (bundledPrefabs[i].gameObject);
			}

			return true;
		}


		static string[] ListPrefabs (string path)
		{
			string pathPrefix = Application.dataPath + "/../";
			path = pathPrefix + path;

			return System.Array.ConvertAll (
				Directory.GetFiles (
					path,
					"*.prefab",
					SearchOption.AllDirectories
				),
				fullPath => fullPath.Substring (pathPrefix.Length)
			);
		}


		static List<string> ListAllAssets (string path, List<string> excluded = null)
		{
			string pathPrefix = Application.dataPath + "/../";
			path = pathPrefix + path;

			List<string> assets = new List<string> ();

			foreach (string filePath in Directory.GetFiles (path, "*", SearchOption.AllDirectories))
			{
				string assetPath = filePath.Substring (pathPrefix.Length);

				if (Path.GetExtension (assetPath) == ".meta" || (excluded != null && excluded.Contains (assetPath)))
				{
					continue;
				}

				assets.Add (assetPath);
			}

			return assets;
		}


		static string[] GetPrefabBundles ()
		{
			return new string[] { "Assets/Prefabs/Example.prefabBundle" };
		}
	}
}
