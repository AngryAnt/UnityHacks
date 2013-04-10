using UnityEngine;
using UnityEditor;
using System.Collections;


[InitializeOnLoad]
public class JavaBuildSettings : ScriptableObject
{
	const string kBuildSettingsPath = "Assets/JavaBuildSettings.asset";


	static JavaBuildSettings s_Instance;


	static JavaBuildSettings Instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = (JavaBuildSettings)AssetDatabase.LoadAssetAtPath (kBuildSettingsPath, typeof (JavaBuildSettings));

				if (s_Instance == null)
				{
					s_Instance = CreateInstance<JavaBuildSettings> ();

					AssetDatabase.CreateAsset (s_Instance, kBuildSettingsPath);

					Selection.activeObject = s_Instance;
					EditorUtility.DisplayDialog (
						"Validate Settings",
						"Default build settings have been created for the Java plugin build process. You should validate these before proceeding.",
						"OK"
					);
				}
			}

			return s_Instance;
		}
	}


	static JavaBuildSettings ()
	{
		if (Instance == null)
		{
			Debug.LogError ("Failed to create build settings");
		}
	}


	[MenuItem ("Edit/Project Settings/Java Build Settings")]
	public static void Edit ()
	{
		Selection.activeObject = Instance;
	}


	public static string MainJavaClass
	{
		get
		{
			return Instance.m_MainJavaClass;
		}
		set
		{
			if (Instance.m_MainJavaClass != value)
			{
				Instance.m_MainJavaClass = value;
				EditorUtility.SetDirty (Instance);
			}
		}
	}


	public static string JarFileName
	{
		get
		{
			return Instance.m_JarFileName;
		}
		set
		{
			if (Instance.m_JarFileName != value)
			{
				Instance.m_JarFileName = value;
				EditorUtility.SetDirty (Instance);
			}
		}
	}


	public static string JavaBasePath
	{
		get
		{
			return Instance.m_JavaBasePath;
		}
		set
		{
			if (Instance.m_JavaBasePath != value)
			{
				Instance.m_JavaBasePath = value;
				EditorUtility.SetDirty (Instance);
			}
		}
	}


	public static string AndroidSDKPath
	{
		get
		{
			return Instance.m_AndroidSDKPath;
		}
		set
		{
			if (Instance.m_AndroidSDKPath != value)
			{
				Instance.m_AndroidSDKPath = value;
				EditorUtility.SetDirty (Instance);
			}
		}
	}


	[SerializeField]
	string
		m_MainJavaClass = "org.Company.Application.Class",
		m_JarFileName = "JavaInterface.jar",
		m_JavaBasePath = "/usr/bin",
		m_AndroidSDKPath = "/android-sdk-mac_86/platforms/android-8";
}
