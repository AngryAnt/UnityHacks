using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace CrossPlatformTools
{
	// TODO: For extra sexiness, create a custom inspector which uses the preview to show the combined preview of all prefabs in the manifest

	public class PrefabBundleManifest : ScriptableObject
	{
		public static PrefabBundleManifest Create ()
		{
			return ScriptableObject.CreateInstance<PrefabBundleManifest> ();
		}


		static string GetUniqueBundleID ()
		{
			return "UniqueBundle42";
		}


		[SerializeField]
		string m_ID;
		[SerializeField]
		List<Object> m_Prefabs = new List<Object> ();


		public string ID
		{
			get
			{
				return m_ID;
			}
		}


		public PrefabBundleManifest ()
		{
			m_ID = GetUniqueBundleID ();
		}


		public BundledPrefab Add (Object prefab)
		{
			m_Prefabs.Add (prefab);

			return BundledPrefab.Create (m_ID, m_Prefabs.Count - 1);
		}


		public Object GetPrefab (int id)
		{
			return m_Prefabs[id];
		}
	}
}
