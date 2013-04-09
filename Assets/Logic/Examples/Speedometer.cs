using UnityEngine;
using System.Collections;


[RequireComponent (typeof (GUIText))]
public class Speedometer : MonoBehaviour
{
	public DirectionalSpeed target;
	public Transform facing;
	public Vector3 axis = Vector3.up;


	float RelativeDirection
	{
		get
		{
			if (facing != null)
			{
				return Vector3.Angle (
					facing.TransformDirection (axis),
					target.transform.TransformDirection (axis)
				);
			}
			else
			{
				Vector3 direction = target.Direction;

				if (direction.magnitude > 0.0f)
				{
					return Vector3.Angle (
						direction,
						target.transform.TransformDirection (axis)
					);
				}

				return 0.0f;
			}
		}
	}


	void Update ()
	{
		guiText.text = string.Format ("{0:f2}", target.speedScale.Evaluate (RelativeDirection / 180.0f));
	}
}
