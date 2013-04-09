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
		public static bool Serialize (BitStream stream, bool boolean)
		// Serialize a bool to a BitStream
		{
			int integer = boolean ? 1 : 0;

			stream.Serialize (ref integer);

			return integer == 1;
		}


		public static Vector3 Serialize (BitStream stream, Vector3 vector)
		// Serialize a Vector3 to a BitStream
		{
			float x = vector.x, y = vector.y, z = vector.z;

			stream.Serialize (ref x);
			stream.Serialize (ref y);
			stream.Serialize (ref z);

			return new Vector3 (x, y, z);
		}


		public static Quaternion Serialize (BitStream stream, Quaternion quaternion)
		// Serialize a Quaternion to a BitStream
		{
			float x = quaternion.x, y = quaternion.y, z = quaternion.z, w = quaternion.w;

			stream.Serialize (ref x);
			stream.Serialize (ref y);
			stream.Serialize (ref z);
			stream.Serialize (ref w);

			return new Quaternion (x, y, z, w);
		}
	}
}
