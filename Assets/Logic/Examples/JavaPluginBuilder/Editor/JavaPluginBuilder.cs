using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Debug = UnityEngine.Debug;


public class JavaPluginBuilder : AssetPostprocessor
{
	const string kBuildPath = "Assets/Plugins/Android/src/";


	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		List<string> changedPathes = new List<string> ();

		changedPathes.AddRange (importedAssets);
		changedPathes.AddRange (deletedAssets);
		changedPathes.AddRange (movedAssets);
		changedPathes.AddRange (movedFromPath);

		foreach (string path in changedPathes)
		{
			if (path.IndexOf (kBuildPath) == 0 && path.IndexOf (".java") == path.Length - ".java".Length)
			{
				Build ();
				return;
			}
		}
	}


	[MenuItem ("Assets/Build Java interface")]
	public static void Build ()
	{
		string workingDirectory = Application.dataPath;
		workingDirectory = workingDirectory.Substring (0, workingDirectory.Length - "Assets/".Length) + "/" + kBuildPath;

		// Cleanup previous jar
		File.Delete (Path.Combine (Path.Combine (workingDirectory, ".."), JavaBuildSettings.JarFileName));

		string[]
			sourceFiles = Directory.GetFiles (workingDirectory, "*.java", SearchOption.AllDirectories),
			jarFiles = Directory.GetFiles (workingDirectory + "..", "*.jar", SearchOption.AllDirectories);

		// Compile classes
		ProcessStartInfo startInfo = new ProcessStartInfo ()
		{
			WorkingDirectory = workingDirectory,
			FileName = Path.Combine (JavaBuildSettings.JavaBasePath, "javac"),
			Arguments = string.Format (
				"{0} -bootclasspath {1} -classpath \"{2}{3}\" -d .",
				"\"" + string.Join ("\" \"", sourceFiles) + "\"",
				"\"" + Path.Combine (JavaBuildSettings.AndroidSDKPath, "android.jar") + "\"",
				jarFiles.Length < 1 ?
					""
				:
					(string.Join (":", jarFiles) + ":"),
				EditorApplication.applicationContentsPath + "/PlaybackEngines/AndroidPlayer/bin/classes.jar"
			),
			UseShellExecute = false,
			RedirectStandardError = true,

		};
		if (!RunBuildProcess (startInfo))
		{
			return;
		}

		// Create signature
		startInfo.FileName = Path.Combine (JavaBuildSettings.JavaBasePath, "javap");
		startInfo.Arguments = "-s " + JavaBuildSettings.MainJavaClass;
		if (!RunBuildProcess (startInfo, "Error creating signature: "))
		{
			return;
		}

		// Create jar
		string classPathBase = JavaBuildSettings.MainJavaClass.Substring (0, JavaBuildSettings.MainJavaClass.IndexOf ('.'));
		startInfo.FileName = Path.Combine (JavaBuildSettings.JavaBasePath, "jar");
		startInfo.Arguments = "cvfM " +
			Path.Combine ("..", JavaBuildSettings.JarFileName) + " " +
			classPathBase +
			Path.DirectorySeparatorChar;

		if (!RunBuildProcess (startInfo, "Error building jar: "))
		{
			return;
		}

		// Cleanup
		Directory.Delete (Path.Combine (workingDirectory, classPathBase), true);

		AssetDatabase.Refresh ();
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
