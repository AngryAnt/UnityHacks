using UnityEngine;
using System.Collections;


[RequireComponent (typeof (LineRenderer))]
public class AnimationCurveRenderer : MonoBehaviour
{
	public AnimationCurve curve;
	public Vector2 size = new Vector2 (10.0f, 4.0f);


	Vector3 GetCurvePosition (int index)
	{
		return
			transform.position +
				transform.right * size.x * curve.keys[index].time +
				transform.up * size.y * curve.keys[index].value;
	}


	void Update ()
	// Adapted from http://en.wikibooks.org/wiki/Cg_Programming/Unity/Bézier_Curves
	{
		LineRenderer lineRenderer = GetComponent<LineRenderer> ();

		/*lineRenderer.SetVertexCount (curve.keys.Length);

		int i = 0;
		foreach (Keyframe key in curve.keys)
		{
			lineRenderer.SetPosition (
				i++,
				transform.position + transform.right * size.x * key.time + transform.up * size.y * key.value
			);
		}*/

		int numberOfPoints = 20;
		int vertexCount = numberOfPoints * (curve.keys.Length - 2) + 2;

		lineRenderer.SetVertexCount (vertexCount);

		lineRenderer.SetPosition (0, GetCurvePosition (0));
		lineRenderer.SetPosition (vertexCount - 1, GetCurvePosition (curve.keys.Length - 1));
		
		// loop over segments of spline
		Vector3 p0, p1, p2;
		for (int j = 0; j < curve.keys.Length - 2; j++)
		{
			// determine control points of segment
			p0 = 0.5f * (GetCurvePosition (j) + GetCurvePosition (j + 1));
			p1 = GetCurvePosition (j + 1);
			p2 = 0.5f * (GetCurvePosition (j + 1) + GetCurvePosition (j + 2));
			
			// set points of quadratic Bezier curve
			Vector3 position;
			float t; 
			float pointStep = 1.0f / numberOfPoints;

			if (j == curve.keys.Length - 3)
			{
				pointStep = 1.0f / (numberOfPoints - 1.0f);
				// last point of last segment should reach p2
			}  

			for (int i = 0; i < numberOfPoints; i++) 
			{
				t = i * pointStep;

				position =
					(1.0f - t) * (1.0f - t) * p0 +
					2.0f * (1.0f - t) * t * p1 +
					t * t * p2;

				lineRenderer.SetPosition (i + j * numberOfPoints + 1, position);
			}
		}
	}
}
