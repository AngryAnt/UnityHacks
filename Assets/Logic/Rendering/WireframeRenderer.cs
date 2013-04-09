using UnityEngine;
using System.Collections.Generic;


// Original source: http://forum.unity3d.com/threads/8814-Wireframe-3D


[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
public class WireframeRenderer : MonoBehaviour
{
	const string kLineshader = @"
		Shader ""Unlit/Color""
		{
			Properties
			{
				_Color(""Color"", Color) = (0, 1, 1, 1)
			}
			SubShader
			{
				Lighting Off
				Color[_Color]
				Pass {}
			}
		}";


	public Color color = Color.red;

	
	Mesh lastMesh;
	Material lastMaterial;
	
	
	public void OnEnable ()
	{
		MeshFilter meshFilter = GetComponent<MeshFilter> ();
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		
		lastMaterial = renderer.material;
		lastMesh = meshFilter.sharedMesh;
		
		Vector3[] vertices = lastMesh.vertices;
		int[] triangles = lastMesh.triangles;
		Vector3[] lines = new Vector3[triangles.Length];
		int[] indexBuffer;
		Mesh generatedMesh = new Mesh();
		
		for (int t = 0; t < triangles.Length; ++t)
		{
			lines[t] = (vertices[triangles[t]]);
		}
		
		generatedMesh.vertices = lines;
		generatedMesh.name = "Generated Wireframe";
		
		int LinesLength = lines.Length;
		indexBuffer = new int[LinesLength];
		Vector2[] uvs = new Vector2[LinesLength];
		Vector3[] normals = new Vector3[LinesLength];
		
		for (int m = 0; m < LinesLength; ++m)
		{
			indexBuffer[m] = m;
			uvs[m] = new Vector2 (0.0f, 1.0f); // sets a fake UV (FAST)
			normals[m] = new Vector3 (1, 1, 1);// sets a fake normal
		}
		
		generatedMesh.uv = uvs;
		generatedMesh.normals = normals;
		generatedMesh.SetIndices (indexBuffer, MeshTopology.LineStrip, 0);
		
		meshFilter.mesh = generatedMesh;
		renderer.material = new Material (kLineshader);

		renderer.material.color = color;
	}
	
	
	void OnDisable ()
	{
		GetComponent<MeshFilter>().mesh = lastMesh;
		GetComponent<MeshRenderer>().material = lastMaterial;
	}
}
