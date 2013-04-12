using UnityEngine;
using System.Collections;


[RequireComponent (typeof (GUIText))]
public class ProgressIndicator : MonoBehaviour
{
	Control control;


	void Start ()
	{
		control = FindObjectOfType (typeof (Control)) as Control;
	}


	void Update ()
	{
		guiText.text = string.Format (
			"{0}{1}/{2}",
			Application.loadedLevel,
			control == null ? "" : ("." + (control.build.Length - control.BuildIndex)),
			Application.levelCount
		);
	}
}
