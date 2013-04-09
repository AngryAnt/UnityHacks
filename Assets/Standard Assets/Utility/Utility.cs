using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace UnityAssets
{
	public partial class Utility
	{
		public static float GetFieldOfViewRadius (Camera camera, Vector3 targetPosition)
		// The minimum clear radius between the camera and the target
		{
			float cameraDistance = (camera.transform.position - targetPosition).magnitude;

			return (cameraDistance / Mathf.Sin (90.0f - camera.fieldOfView / 2.0f)) * Mathf.Sin (camera.fieldOfView / 2.0f);
		}


		public static void SetLayerRecursively (Transform transform, int layer)
		// Set the layer recursively of the given transform and its children
		{
			transform.gameObject.layer = layer;
			foreach (Transform child in transform)
			{
				SetLayerRecursively (child, layer);
			}
		}


		public static void SyncRig (Transform original, Transform target)
		// Sync two itentical transform hierarchies - for synchronising the ragdoll rig with the animated rig
		{
			target.localPosition = original.localPosition;
			target.localRotation = original.localRotation;

			if (original.childCount == 0)
			{
				return;
			}

			IEnumerator originalChild = original.GetEnumerator (), targetChild = target.GetEnumerator ();

			while (originalChild.MoveNext () && targetChild.MoveNext ())
			{
				SyncRig ((Transform)originalChild.Current, (Transform)targetChild.Current);
			}
		}


		public static void DisableRagdoll (Transform root)
		// Given a ragdoll transform root, pick off all joints, rigidbodies and colliders
		{
			CharacterJoint[] joints = root.GetComponentsInChildren<CharacterJoint> ();

			foreach (CharacterJoint part in joints)
			{
				Object.Destroy (part);
			}

			Rigidbody[] rigidbodies = root.GetComponentsInChildren<Rigidbody> ();

			foreach (Rigidbody part in rigidbodies)
			{
				Object.Destroy (part);
			}

			Collider[] colliders = root.GetComponentsInChildren<Collider> ();

			foreach (Collider part in colliders)
			{
				Object.Destroy (part);
			}
		}


		public static T NearestPoint<T> (IEnumerable<T> options, Vector3 point, params T[] exclude) where T : Component
		// Out of the given options, return the one closest to the given point
		{
			float shortestDistance = Mathf.Infinity;
			T nearest = null;

			foreach (T component in options)
			{
				if (Array.IndexOf (exclude, component) > -1)
				{
					continue;
				}

				float distance = (component.transform.position - point).sqrMagnitude;
				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					nearest = component;
				}
			}

			return nearest;
		}


		public static float BlendWeight (float value, float speed, bool direction)
		{
			return Mathf.Clamp (value + Time.deltaTime * speed * (direction ? 1.0f : -1.0f), 0.0f, 1.0f);
		}


		public static float ClampAngle (float angle)
		{
			return angle > 360.0f ? angle - 360.0f : angle < 0.0f ? angle + 360.0f : angle;
		}


		public static Vector3 PlanarDirection (Vector3 from, Vector3 to)
		{
			Vector3 direction = to - from;
			return new Vector3 (direction.x, 0.0f, direction.z);
		}


		public static float AngleToPosition (Transform transform, Vector3 position)
		// Returns the clockwise angle between the forward vector of the given transform and the directional vector between the transform position and the given position
		{
			Vector3
				forward = new Vector3 (transform.forward.x, 0.0f, transform.forward.z),
				direction = PlanarDirection (transform.position, position);

			Quaternion q = Quaternion.identity;
			q.SetFromToRotation (forward, direction);

			Vector3 axis;
			float angle;
			q.ToAngleAxis (out angle, out axis);

			return (axis * angle).y;
		}


		public 	static bool IsWindows
		{
			get
			{
				switch (Application.platform)
				{
					case RuntimePlatform.WindowsPlayer:
					case RuntimePlatform.WindowsEditor:
					case RuntimePlatform.WindowsWebPlayer:
						return true;
					default:
						return false;
				}
			}
		}
	}
}
