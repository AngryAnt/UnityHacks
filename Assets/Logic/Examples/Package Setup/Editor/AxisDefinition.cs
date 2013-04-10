using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;


namespace UnityAssets
{
	[Serializable]
	public enum AxisType
	{
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
	};
	

	[Serializable]
	public class AxisDefinition : IEquatable<AxisDefinition>
	{
		public string
			name,
			descriptiveName,
			descriptiveNegativeName,
			negativeButton,
			positiveButton,
			altNegativeButton,
			altPositiveButton;
		
		public float
			gravity,
			dead,
			sensitivity;
		
		public bool
			snap = false,
			invert = false;
		
		public AxisType type;
		
		public int
			axis,
			joyNum;
		
		
		public AxisDefinition ()
		{}
		
		
		public AxisDefinition (SerializedProperty source)
		{
			Load (source);
		}
		
		
		public static void Save (AxisDefinition axis, SerializedProperty target)
		{
			GetChildProperty (target, "m_Name").stringValue = axis.name;
			GetChildProperty (target, "descriptiveName").stringValue = axis.descriptiveName;
			GetChildProperty (target, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
			GetChildProperty (target, "negativeButton").stringValue = axis.negativeButton;
			GetChildProperty (target, "positiveButton").stringValue = axis.positiveButton;
			GetChildProperty (target, "altNegativeButton").stringValue = axis.altNegativeButton;
			GetChildProperty (target, "altPositiveButton").stringValue = axis.altPositiveButton;
			GetChildProperty (target, "gravity").floatValue = axis.gravity;
			GetChildProperty (target, "dead").floatValue = axis.dead;
			GetChildProperty (target, "sensitivity").floatValue = axis.sensitivity;
			GetChildProperty (target, "snap").boolValue = axis.snap;
			GetChildProperty (target, "invert").boolValue = axis.invert;
			GetChildProperty (target, "type").intValue = (int)axis.type;
			GetChildProperty (target, "axis").intValue = axis.axis - 1;
			GetChildProperty (target, "joyNum").intValue = axis.joyNum;
		}
		
		
		public void Load (SerializedProperty source)
		{
			name = GetChildProperty (source, "m_Name").stringValue;
			descriptiveName = GetChildProperty (source, "descriptiveName").stringValue;
			descriptiveNegativeName = GetChildProperty (source, "descriptiveNegativeName").stringValue;
			negativeButton = GetChildProperty (source, "negativeButton").stringValue;
			positiveButton = GetChildProperty (source, "positiveButton").stringValue;
			altNegativeButton = GetChildProperty (source, "altNegativeButton").stringValue;
			altPositiveButton = GetChildProperty (source, "altPositiveButton").stringValue;
			gravity = GetChildProperty (source, "gravity").floatValue;
			dead = GetChildProperty (source, "dead").floatValue;
			sensitivity = GetChildProperty (source, "sensitivity").floatValue;
			snap = GetChildProperty (source, "snap").boolValue;
			invert = GetChildProperty (source, "invert").boolValue;
			type = (AxisType)GetChildProperty (source, "type").intValue;
			axis = GetChildProperty (source, "axis").intValue + 1;
			joyNum = GetChildProperty (source, "joyNum").intValue;
		}
		
		
		public bool InterfaceEquals (AxisDefinition other)
		{
			if (other.type != type || other.name != name)
			{
				return false;
			}
			
			switch (type)
			{
				case AxisType.KeyOrMouseButton:
				return
					other.negativeButton.Equals (negativeButton) &&
					other.positiveButton.Equals (positiveButton) &&
					other.altNegativeButton.Equals (altNegativeButton) &&
					other.altPositiveButton.Equals (altPositiveButton);
				case AxisType.MouseMovement:
				return other.axis.Equals (axis);
				case AxisType.JoystickAxis:
				return
					other.axis.Equals (axis) &&
					other.joyNum.Equals (joyNum);
			}
			
			throw new ArgumentException ("Unknown axis type: " + type);
		}


		public bool Equals (AxisDefinition other)
		{
			if (other == null)
			{
				return false;
			}
			
			foreach (FieldInfo field in GetType ().GetFields ())
			{
				if (!field.GetValue (other).Equals (field.GetValue (this)))
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		public override bool Equals (object obj)
		{
			return Equals (obj as AxisDefinition);
		}
		
		
		public override int GetHashCode ()
		{
			int value = 0;
			
			foreach (FieldInfo field in GetType ().GetFields ())
			{
				value ^= field.GetValue (this).GetHashCode ();
			}
			
			return value;
		}


		public override string ToString ()
		{
			return name;
		}
		
		
		public static SerializedProperty GetChildProperty (SerializedProperty parent, string name)
		{
			SerializedProperty child = parent.Copy ();
			child.Next (true);
			
			do
			{
				if (child.name == name)
				{
					return child;
				}
			}
			while (child.Next (false));
			
			return null;
		}
	}
}
