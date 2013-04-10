using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


namespace UnityAssets
{
	public class InputInterface
	{
		static SerializedObject GetInputManager ()
		{
			return new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset")[0]);
		}


		public static List<AxisDefinition> GetAxes ()
		{
			List<AxisDefinition> result = new List<AxisDefinition> ();

			SerializedObject inputManager = GetInputManager ();
			SerializedProperty axes = inputManager.FindProperty ("m_Axes");

			for (int i = 0; i < axes.arraySize; ++i)
			{
				result.Add (new AxisDefinition (axes.GetArrayElementAtIndex (i)));
			}

			return result;
		}


		public static void SaveAxis (AxisDefinition axis, int index)
		{
			SerializedObject inputManager = GetInputManager ();
			SerializedProperty axes = inputManager.FindProperty ("m_Axes");
			AxisDefinition.Save (axis, axes.GetArrayElementAtIndex (index));

			inputManager.ApplyModifiedProperties ();
		}


		public static void AddAxis (AxisDefinition axis)
		{
			SerializedObject inputManager = GetInputManager ();
			SerializedProperty axes = inputManager.FindProperty ("m_Axes");
			axes.InsertArrayElementAtIndex (axes.arraySize - 1);
			AxisDefinition.Save (axis, axes.GetArrayElementAtIndex (axes.arraySize - 1));

			inputManager.ApplyModifiedProperties ();
		}
	}
}
