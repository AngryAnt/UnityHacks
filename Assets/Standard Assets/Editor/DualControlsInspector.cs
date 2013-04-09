using UnityEngine;
using UnityEditor;
using System.Collections;


namespace UnityAssets
{
	[CustomEditor (typeof (DualControls))]
	public class DualControlsInspector : PropertyEditor
	{
		SerializedProperty
			autoConnectControllerProperty,

			inputSensitivityMinProperty,
			inputSensitivityDefaultProperty,
			inputSensitivityMaxProperty,

			inputVerticalSkewMinProperty,
			inputVerticalSkewDefaultProperty,
			inputVerticalSkewMaxProperty,

			windowsMouseSensitivityScaleProperty,

			inputMobilityMinProperty,
			inputMobilityDefaultProperty,
			inputMobilityMaxProperty,

			inputStickinessMinProperty,
			inputStickinessDefaultProperty,
			inputStickinessMaxProperty,

			inputReleaseDelayMinProperty,
			inputReleaseDelayDefaultProperty,
			inputReleaseDelayMaxProperty;


		protected override void Initialize ()
		{
			autoConnectControllerProperty = serializedObject.FindProperty ("autoConnectController");

			inputSensitivityMinProperty = serializedObject.FindProperty ("inputSensitivityMin");
			inputSensitivityDefaultProperty = serializedObject.FindProperty ("inputSensitivityDefault");
			inputSensitivityMaxProperty = serializedObject.FindProperty ("inputSensitivityMax");

			inputVerticalSkewMinProperty = serializedObject.FindProperty ("inputVerticalSkewMin");
			inputVerticalSkewDefaultProperty = serializedObject.FindProperty ("inputVerticalSkewDefault");
			inputVerticalSkewMaxProperty = serializedObject.FindProperty ("inputVerticalSkewMax");

			windowsMouseSensitivityScaleProperty = serializedObject.FindProperty ("windowsMouseSensitivityScale");

			inputMobilityMinProperty = serializedObject.FindProperty ("inputMobilityMin");
			inputMobilityDefaultProperty = serializedObject.FindProperty ("inputMobilityDefault");
			inputMobilityMaxProperty = serializedObject.FindProperty ("inputMobilityMax");

			inputStickinessMinProperty = serializedObject.FindProperty ("inputStickinessMin");
			inputStickinessDefaultProperty = serializedObject.FindProperty ("inputStickinessDefault");
			inputStickinessMaxProperty = serializedObject.FindProperty ("inputStickinessMax");

			inputReleaseDelayMinProperty = serializedObject.FindProperty ("inputReleaseDelayMin");
			inputReleaseDelayDefaultProperty = serializedObject.FindProperty ("inputReleaseDelayDefault");
			inputReleaseDelayMaxProperty = serializedObject.FindProperty ("inputReleaseDelayMax");
		}


		public override void OnInspectorGUI ()
		{
			EditorGUIUtility.LookLikeInspector ();

			BeginEdit ();
				BeginSection ("Base");
					PropertyField ("Auto-connect controller", autoConnectControllerProperty);
				EndSection ();

				BeginSection ("Sensitivity");
					MinDefaultMaxField ("General", inputSensitivityMinProperty, inputSensitivityDefaultProperty, inputSensitivityMaxProperty);
					MinDefaultMaxField ("Vertical skew", inputVerticalSkewMinProperty, inputVerticalSkewDefaultProperty, inputVerticalSkewMaxProperty);
					PropertyField ("Windows mouse scale", windowsMouseSensitivityScaleProperty);
				EndSection ();

				BeginSection ("Virtual sticks");
					MinDefaultMaxField ("Mobility", inputMobilityMinProperty, inputMobilityDefaultProperty, inputMobilityMaxProperty);
					MinDefaultMaxField ("Stickiness", inputStickinessMinProperty, inputStickinessDefaultProperty, inputStickinessMaxProperty);
					MinDefaultMaxField ("Release delay", inputReleaseDelayMinProperty, inputReleaseDelayDefaultProperty, inputReleaseDelayMaxProperty);
				EndSection ();
			EndEdit ();
		}


		void MinDefaultMaxField (string label, SerializedProperty minProperty, SerializedProperty defaultProperty, SerializedProperty maxProperty)
		{
			const float kMinMaxWidth = 30.0f, kFieldPadding = 10.0f, kDefaultIndentationWidth = 9.0f;

			EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.PrefixLabel (label, EditorStyles.textField);//minProperty.prefabOverride || defaultProperty.prefabOverride || maxProperty.prefabOverride ? EditorStyles.boldLabel : EditorStyles.label);

				int level = EditorGUI.indentLevel;
				EditorGUI.indentLevel = 0;

				GUILayout.Space (-kDefaultIndentationWidth);

				Rect componentRect = GUILayoutUtility.GetRect (kMinMaxWidth * 3.0f + kFieldPadding * 2.0f, 20.0f, GUILayout.ExpandHeight (false));

				EditorGUI.PropertyField (
					new Rect (componentRect.x, componentRect.y, kMinMaxWidth, componentRect.height),
					minProperty,
					GUIContent.none,
					true
				);

				float newDefault = GUI.HorizontalSlider (
					new Rect (componentRect.x + kMinMaxWidth + kFieldPadding, componentRect.y, componentRect.width - (kMinMaxWidth + kFieldPadding) * 2.0f, componentRect.height),
					defaultProperty.floatValue,
					minProperty.floatValue,
					maxProperty.floatValue
				);
				if (newDefault != defaultProperty.floatValue)
				{
					defaultProperty.floatValue = newDefault;
				}

				EditorGUI.PropertyField (
					new Rect (componentRect.x + componentRect.width - kMinMaxWidth, componentRect.y, kMinMaxWidth, componentRect.height),
					maxProperty,
					GUIContent.none,
					true
				);

				EditorGUI.indentLevel = level;
			EditorGUILayout.EndHorizontal ();
		}
	}
}
