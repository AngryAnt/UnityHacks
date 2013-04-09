using UnityEngine;
using System.Collections;


public class Fade : MonoBehaviour
{
	public float duration = 2.0f;
	public bool fadeIn = true;


	Color target;


	void Awake ()
	{
		target = renderer.material.color;
	}


	IEnumerator Start ()
	{
		float strength = 0.0f;

		do
		{
			strength += Time.deltaTime / duration;
			renderer.material.color = new Color (target.r, target.g, target.b, fadeIn ? strength : 1.0f - strength);

			yield return null;
		}
		while (strength < 1.0f);
	}
}
