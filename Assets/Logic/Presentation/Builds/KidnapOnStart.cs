using UnityEngine;
using System.Collections;


public class KidnapOnStart : MonoBehaviour
{
	public Transform target, kidnapper;


	void Start ()
	{
		target.parent = kidnapper;
	}
}
