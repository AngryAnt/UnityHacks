using UnityEngine;
using System.Collections;


[RequireComponent (typeof (GUIText))]
public class JSInterface : MonoBehaviour
{
	public TextAsset externalJS;


	void Start ()
	{
		Application.ExternalEval (externalJS.text);
	}
	

	void JSCallback (string message)
	{
		guiText.text = "JS says: " + message;
	}
}
