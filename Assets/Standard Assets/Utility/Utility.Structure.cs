using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


namespace UnityAssets
{
	public enum RequirementPunishment
	{
		Nothing,
		Disable,
		Destroy,
		DestroyGameObject
	};

	public enum RequirementSeverity
	{
		Failure,
		Warning
	}


	public partial class Utility
	{
		public static Dictionary<T, Action> GetStateHandlers<T> (object owner, string prefix = null, string postfix = null) where T : struct, IConvertible
		// Returns a dictionary of delegates indexed by an enum. The delegates are created based on reflection of the owner object, the enum values and any given pre- or postfix
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException ("T must be an enum type");
			}

			if (owner == null)
			{
				throw new ArgumentException ("Invalid owner");
			}

			Dictionary<T, Action> handlers = new Dictionary<T, Action> ();

			MethodInfo[] methods = owner.GetType ().GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Static);
			foreach (MethodInfo method in methods)
			// Consider all public, private and protected static and non-static methods
			{
				if (method.IsAbstract || method.ReturnType != typeof (void) || method.GetParameters ().Length > 0)
				// Ignore abstract methods or methods not matching the Action delegate
				{
					continue;
				}

				if (prefix != null && method.Name.IndexOf (prefix) != 0)
				// If a prefix is required, but not used in this methods name, bypass it
				{
					continue;
				}

				if (postfix != null && method.Name.LastIndexOf (postfix) != method.Name.Length - postfix.Length)
				// If a postfix is required, but not used in this methods name, bypass it
				{
					continue;
				}

				// Match the method name minus any pre- and postfixes to the possible values of the passed enum //

				string handlerName = method.Name.Substring (0, postfix == null ? method.Name.Length : method.Name.Length - postfix.Length).Substring (prefix == null ? 0 : prefix.Length);
				T handlerEnum;

				try
				{
					handlerEnum = (T)Enum.Parse (typeof (T), handlerName);
				}
				catch (Exception)
				{
					continue;
				}

				handlers[handlerEnum] = (Action)Delegate.CreateDelegate (typeof (Action), owner, method);
					// Add the new delegate
			}

			return handlers;
		}


		public static bool Require (bool mustBeTrue, string failureMessage, MonoBehaviour responsible, RequirementPunishment punishment = RequirementPunishment.Disable, RequirementSeverity severity = RequirementSeverity.Failure)
		// Used for verifying a MonoBehaviour setup
		{
			if (!mustBeTrue)
			{
				if (severity == RequirementSeverity.Warning)
				{
					Debug.LogWarning (failureMessage + ".", responsible);
				}
				else
				{
					Debug.LogError (failureMessage + ". Please correct and restart.", responsible);
				}

				switch (punishment)
				{
					case RequirementPunishment.Disable:
						responsible.enabled = false;
					break;
					case RequirementPunishment.Destroy:
						Object.Destroy (responsible);
					break;
					case RequirementPunishment.DestroyGameObject:
						Object.Destroy (responsible.gameObject);
					break;
				}

				return severity == RequirementSeverity.Warning;
			}

			return true;
		}


		public static bool RequireSet (object mustBeNonNull, string name, MonoBehaviour responsible)
		// Used for verifying that a particular object reference has been set
		{
			return Require (mustBeNonNull != null, "No " + name.ToLower () + " set", responsible);
		}


		public static bool RequireSingleton (MonoBehaviour responsible)
		// Used to ensure that only one instance of the given monibehaviour derived class is active
		{
			Type type = responsible.GetType ();

			Require (
				Object.FindObjectsOfType (type).Length < 2,
				string.Format ("Multiple instances of {0} found. Destroying new instance", type.Name),
				responsible,
				RequirementPunishment.Destroy,
				RequirementSeverity.Warning
			);

			return true;
		}
	}
}
