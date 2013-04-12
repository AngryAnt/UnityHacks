using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
using System.Collections;


public class BuildOnStart : MonoBehaviour
{
	public bool autoBuild = true, autoResume = true;
	public string[] levels;
	public Texture2D buttonIcon;

#if UNITY_EDITOR
	public BuildTarget target = BuildTarget.WebPlayer;


	BuildOptions options = BuildOptions.AutoRunPlayer | BuildOptions.WebPlayerOfflineDeployment;


	void Reset ()
	{
		if (levels.Length < 1)
		{
			levels = new string[] {EditorApplication.currentScene};
		}
	}


	void Start ()
	{
		if (autoBuild)
		{
			Build ();
		}
	}


	void Build ()
	{
		BuildPipeline.BuildPlayer (levels, Application.dataPath + "/../Temp/Build" + GetExtension (target), target, options);
		if (autoResume)
		{
			EditorApplication.isPlaying = true;
		}
	}


	string GetExtension (BuildTarget target)
	{
		switch (target)
		{
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return ".app";
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
				return "";
			default:
				throw new System.ArgumentException ("Unsupported target: " + target);
		}
	}


	void OnGUI ()
	{
		if (!autoBuild &&
		    GUI.Button (
				new Rect (
					Screen.width * transform.position.x,
					Screen.height - Screen.height * transform.position.y,
					buttonIcon.width,
					buttonIcon.height
				),
				buttonIcon,
				GUIStyle.none
			)
		)
		{
			Build ();
		}
	}
#endif
}
