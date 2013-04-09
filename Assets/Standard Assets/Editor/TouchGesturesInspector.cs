using UnityEngine;
using UnityEditor;
using System.Collections;


namespace UnityAssets
{
	[CustomEditor (typeof (TouchGestures))]
	public class TouchGesturesInspector : PropertyEditor
	{
		SerializedProperty
			maxDurationProperty,
			minSwipeScreenTravelProperty,
			minPinchScreenTravelProperty,
			maxTapScreenTravelProperty,
			maxTapDurationProperty,
			receiversProperty,
			trackingMaskProperty,
			trackMultipleProperty;


		protected override void Initialize ()
		{
			maxDurationProperty = serializedObject.FindProperty ("maxDuration");
			minSwipeScreenTravelProperty = serializedObject.FindProperty ("minSwipeScreenTravel");
			minPinchScreenTravelProperty = serializedObject.FindProperty ("minPinchScreenTravel");
			maxTapScreenTravelProperty = serializedObject.FindProperty ("maxTapScreenTravel");
			maxTapDurationProperty = serializedObject.FindProperty ("maxTapDuration");
			receiversProperty = serializedObject.FindProperty ("receivers");
			trackingMaskProperty = serializedObject.FindProperty ("trackingMask");
			trackMultipleProperty = serializedObject.FindProperty ("trackMultiple");
		}


		public override void OnInspectorGUI ()
		{
			EditorGUIUtility.LookLikeInspector ();

			BeginEdit ();
				BeginSection ("Tracing");
					PropertyField ("Track multiple gestures", trackMultipleProperty);
					EnumMaskPropertyField<GestureType> ("Tracked gestures", trackingMaskProperty);
					PropertyField ("Receivers", receiversProperty);
				EndSection ();
				BeginSection ("Calibration");
					PropertyField ("Max gesture duration", maxDurationProperty);
					PropertyField ("Min swipe screen travel", minSwipeScreenTravelProperty);
					PropertyField ("Min pinch screen travel", minPinchScreenTravelProperty);
					PropertyField ("Max tap screen travel", maxTapScreenTravelProperty);
					PropertyField ("Max tap duration", maxTapDurationProperty);
				EndSection ();
			EndEdit ();
		}
	}
}
