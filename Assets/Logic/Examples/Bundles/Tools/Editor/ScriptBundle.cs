using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Linq;


namespace CrossPlatformTools
{
	public class ScriptBundle : AssetPostprocessor
	{
		const string kBuildPath = "Assets/WebPlayerTemplates/ScriptBundles";


		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
		{
			List<string> changedPathes = new List<string> ();

			changedPathes.AddRange (importedAssets);
			changedPathes.AddRange (deletedAssets);
			changedPathes.AddRange (movedAssets);
			changedPathes.AddRange (movedFromPath);

			foreach (string path in changedPathes)
			{
				if (path.IndexOf (kBuildPath) == 0 && path.IndexOf (".cs") == path.Length - ".cs".Length)
				{
					Build ();
					return;
				}
			}
		}


		[MenuItem ("Assets/Build script bundles")]
		public static void Build ()
		{
			string[] bundleSources = Directory.GetDirectories (kBuildPath);

			foreach (string bundleSourcePath in bundleSources)
			{
				BuildBundle (bundleSourcePath);
			}
		}


		static void BuildBundle (string path)
		{
			string name = path.Substring (path.LastIndexOf ("/") + 1);

			Debug.Log ("Building " + name);

			string[] scripts = Directory.GetFiles (
				Application.dataPath + "/../" + path,
				"*.cs",
				SearchOption.AllDirectories
			);

			string assemblyPath = "Assets/" + name + ".bytes";

			string parameters = string.Format (
				"-target:library -out:\"{0}\" -d:SCRIPT_BUNDLE -r:\"{1}\" -r:\"{2}\" \"{3}\"",
				Application.dataPath + "/../" + assemblyPath,
				Application.dataPath + "/../Library/ScriptAssemblies/Assembly-CSharp.dll",
				string.Format (
					"{0}/PlaybackEngines/{1}/Managed/UnityEngine.dll",
					EditorApplication.applicationContentsPath,
					EditorUserBuildSettings.activeBuildTarget
				),
				string.Join ("\" \"", scripts)
			);

			ProcessStartInfo startInfo = new ProcessStartInfo ()
			{
				WorkingDirectory = Application.dataPath,
				FileName = EditorApplication.applicationContentsPath + "/Frameworks/Mono/bin/gmcs",
				Arguments = parameters,
				UseShellExecute = false,
				RedirectStandardError = true
			};

			if (!RunBuildProcess (startInfo))
			{
				return;
			}

			AssetDatabase.Refresh ();

			Object assemblyAsset = AssetDatabase.LoadMainAssetAtPath (assemblyPath);

			if (assemblyAsset == null)
			{
				Debug.LogError ("Failed to load the generated assembly asset");
			}

			// TODO: Import pipeline regression breaks this. Comment back in when fixed.
			Object[] additionalAssets = new Object[0];/*(
					from assetPath in Directory.GetFiles (
						path,
						"*",
						SearchOption.AllDirectories
					)
					where Path.GetExtension (assetPath) != ".cs"
					select AssetDatabase.LoadMainAssetAtPath (assetPath)
				).ToArray ();*/

			/*foreach (Object asset in additionalAssets)
			{
				Debug.Log (asset == null ? "null" : AssetDatabase.GetAssetPath (asset), asset);
			}*/

			BuildPipeline.BuildAssetBundle (
				assemblyAsset,
				additionalAssets,
				Application.dataPath + "/../" + name + ".unity3d",
				BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies,
				EditorUserBuildSettings.activeBuildTarget
			);

			AssetDatabase.DeleteAsset (assemblyPath);
		}


		static bool RunBuildProcess (ProcessStartInfo startInfo, string errorPrefix = "")
		{
			Process process = Process.Start (startInfo);
			process.WaitForExit ();

			if (!process.StandardError.EndOfStream)
			{
				Debug.LogError (errorPrefix + process.StandardError.ReadToEnd ());

				return false;
			}

			return true;
		}
	}
}
