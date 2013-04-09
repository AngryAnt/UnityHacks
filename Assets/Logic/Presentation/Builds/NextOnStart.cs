using UnityEngine;
using System.Collections;


public class NextOnStart : MonoBehaviour
{
	void Start ()
	{
		((Control)FindObjectOfType (typeof (Control))).Next ();
	}
}
