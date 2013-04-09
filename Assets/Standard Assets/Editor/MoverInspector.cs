using UnityEngine;
using UnityEditor;
using System.Collections;


namespace UnityAssets
{
	[CustomEditor (typeof (Mover))]
	public class MoverInspector : PropertyEditor
	{
		SerializedProperty
			modeProperty,

			debugProperty,

			targetVelocityProperty,
			targetPositionProperty,
			targetRotationProperty,

			speedProperty,
			speedScaleProperty,
			arrivalDistanceProperty,
			positionInterpolationSpeedProperty,
			rotationInterpolationSpeedProperty,

			gravityProperty,
			minimumGravityMovementProperty,
			groundLayersProperty,
			groundDistanceProperty,
			groundCheckOffsetProperty,

			constrainVelocityProperty,
			targetDesiredVelocityProperty,
			rotationModeProperty,
			constrainRotationProperty;


		protected override void Initialize ()
		{
			modeProperty = serializedObject.FindProperty ("mode");

			debugProperty = serializedObject.FindProperty ("debug");

			targetVelocityProperty = serializedObject.FindProperty ("targetVelocity");
			targetPositionProperty = serializedObject.FindProperty ("targetPosition");
			targetRotationProperty = serializedObject.FindProperty ("targetRotation");

			speedProperty = serializedObject.FindProperty ("speed");
			speedScaleProperty = serializedObject.FindProperty ("speedScale");
			arrivalDistanceProperty = serializedObject.FindProperty ("arrivalDistance");
			positionInterpolationSpeedProperty = serializedObject.FindProperty ("positionInterpolationSpeed");
			rotationInterpolationSpeedProperty = serializedObject.FindProperty ("rotationInterpolationSpeed");

			gravityProperty = serializedObject.FindProperty ("gravity");
			minimumGravityMovementProperty = serializedObject.FindProperty ("minimumGravityMovement");
			groundLayersProperty = serializedObject.FindProperty ("groundLayers");
			groundDistanceProperty = serializedObject.FindProperty ("groundedDistance");
			groundCheckOffsetProperty = serializedObject.FindProperty ("groundedCheckOffset");

			constrainVelocityProperty = serializedObject.FindProperty ("constrainVelocity");
			targetDesiredVelocityProperty = serializedObject.FindProperty ("targetDesiredVelocity");
			rotationModeProperty = serializedObject.FindProperty ("rotationMode");
			constrainRotationProperty = serializedObject.FindProperty ("constrainRotation");
		}


		public override void OnInspectorGUI ()
		{
			EditorGUIUtility.LookLikeInspector ();

			BeginEdit ();
				BeginSection ("Base");
					PropertyField ("Mode", modeProperty);
					PropertyField ("Debug", debugProperty);
				EndSection ();

				BeginSection ("Steering");
					PropertyField ("Speed", speedProperty);
					PropertyField ("Speed scale", speedScaleProperty);
					PropertyField ("Arrival distance", arrivalDistanceProperty);
					PropertyField ("Position interpolation", positionInterpolationSpeedProperty);
					PropertyField ("Rotation interpolation", rotationInterpolationSpeedProperty);
				EndSection ();

				if (modeProperty.intValue == (int)Mover.Mode.Physics || modeProperty.intValue == (int)Mover.Mode.Auto)
				{
					BeginSection ("Physics settings");
						PropertyField ("Gravity", gravityProperty);
						PropertyField ("Min gravity movement", minimumGravityMovementProperty);
						PropertyField ("Ground layers", groundLayersProperty);
						PropertyField ("Ground distance", groundDistanceProperty);
						PropertyField ("Ground check offset", groundCheckOffsetProperty);
					EndSection ();
				}

				BeginSection ("Steering constraints");
					PropertyField ("Constrain velocity", constrainVelocityProperty);
					if (modeProperty.intValue == (int)Mover.Mode.Navigation || modeProperty.intValue == (int)Mover.Mode.Auto)
					{
						PropertyField ("Target desired velocity", targetDesiredVelocityProperty);
					}
					PropertyField ("Rotation mode", rotationModeProperty);
					if (rotationModeProperty.intValue == (int)Mover.RotationMode.Mover)
					{
						PropertyField ("Constrain rotation", constrainRotationProperty);
					}
				EndSection ();

				BeginSection ("Runtime targets");
					PropertyField ("Velocity", targetVelocityProperty);
					PropertyField ("Position", targetPositionProperty);
					PropertyField ("Rotation", targetRotationProperty);
				EndSection ();
			EndEdit ();
		}


		protected override void DoSceneGUI ()
		{
			BeginEdit ();
				Handles.color = new Color (0.0f, 0.1f, 0.2f, 0.3f);
				Handles.DrawSolidDisc (TargetTransform.position, TargetTransform.up, speedProperty.floatValue);
				Handles.color = new Color (0.0f, 0.1f, 0.2f, 1.0f);
				speedProperty.floatValue = Handles.RadiusHandle (TargetTransform.rotation, TargetTransform.position, speedProperty.floatValue, true);

				Handles.color = new Color (0.0f, 0.2f, 0.1f, 0.3f);
				Handles.DrawSolidDisc (TargetTransform.position, TargetTransform.up, arrivalDistanceProperty.floatValue);
				Handles.color = new Color (0.0f, 0.1f, 0.2f, 1.0f);
				arrivalDistanceProperty.floatValue = Handles.RadiusHandle (TargetTransform.rotation, TargetTransform.position, arrivalDistanceProperty.floatValue, true);
			EndEdit ();
		}
	}
}
