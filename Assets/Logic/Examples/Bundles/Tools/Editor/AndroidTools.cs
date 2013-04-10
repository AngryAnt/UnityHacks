using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;

using Debug = UnityEngine.Debug;


public class AndroidTools
{
	[MenuItem ("File/Launch Android Emulator")]
	static void LaunchEmulator ()
	{
		ProcessStartInfo startInfo = new ProcessStartInfo ()
		{
			WorkingDirectory = Application.dataPath,
			FileName = JavaBuildSettings.AndroidSDKPath + "/../../tools/emulator",
			Arguments = string.Format (
				"-avd {0}",
				"OUYA"
			),
			UseShellExecute = false,
			RedirectStandardError = true,
		};

		Process process = Process.Start (startInfo);
		process.Exited += new EventHandler (
			(object sender, EventArgs e) =>
			{
				if (!process.StandardError.EndOfStream)
				{
					Debug.LogError (
						"Android emulator error: " +
							process.StandardError.ReadToEnd ()
					);
				}
			}
		);
	}
}