using UnityEngine;
using System.Collections;


[RequireComponent (typeof (GUIText))]
public class KeyVisualiser : MonoBehaviour
{
	public KeyCode[] trackedKeys;


	void Update ()
	{
		guiText.text = "";

		foreach (KeyCode code in trackedKeys)
		{
			if (Input.GetKey (code))
			{
				guiText.text += code;
			}
		}
	}
}
