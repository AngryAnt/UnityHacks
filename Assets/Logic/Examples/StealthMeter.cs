using UnityEngine;
using System.Collections;


public class StealthMeter : MonoBehaviour
{
	const float kStealthBottom = 0.1f, kStealthTop = 0.8f;


	new public Renderer renderer;
	public Texture2D outline, progress;
	
	
	Vector3 common;//, right, top, front;
	
	
	public float Stealth
	{
		get
		{
			return (Mathf.Clamp (
				1.0f - (common.x + common.y + common.z),
				kStealthBottom,
				kStealthTop
			) - kStealthBottom) / (kStealthTop - kStealthBottom);
		}
	}
	
	
	void Update ()
	{
		float[] coefficients = new float[9 * 3];
		
		LightmapSettings.lightProbes.GetInterpolatedLightProbe (renderer.transform.position, renderer, coefficients);
		
		common = new Vector3 (coefficients[0], coefficients[1], coefficients[2]);
		/*right = new Vector3 (coefficients[3], coefficients[4], coefficients[5]);
		top = new Vector3 (coefficients[6], coefficients[7], coefficients[8]);
		front = new Vector3 (coefficients[9], coefficients[10], coefficients[11]);*/
	}
	
	
	void OnGUI ()
	{
		/*LightLabel ("Common", common);
		LightLabel ("Right", right);
		LightLabel ("Top", top);
		LightLabel ("Front", front);
		GUILayout.Box (string.Format ("Stealth: {0:f2}", Stealth));*/
		
		const float kBarWidth = 150.0f, kBarHeight = 15.0f, kOffset = 20.0f;

		GUI.DrawTexture (new Rect (Screen.width - kBarWidth - kOffset, kOffset, Stealth * kBarWidth, kBarHeight), progress);
		GUI.DrawTexture (new Rect (Screen.width - kBarWidth - kOffset, kOffset, kBarWidth, kBarHeight), outline);
	}
	
	
	static void LightLabel (string label, Vector3 coefficients)
	{
		GUILayout.BeginHorizontal (GUI.skin.box);
		GUILayout.Label (label, GUILayout.Width (70.0f));
		GUILayout.Label (string.Format (
			"{0:f2}, {1:f2}, {2:f2}",
			coefficients.x,
			coefficients.y,
			coefficients.z
			));
		GUILayout.EndHorizontal ();
	}
}
