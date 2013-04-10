using UnityEngine;
using System.Collections;


namespace CrossPlatformTools
{
	public class BundledPrefab : MonoBehaviour
	{
		public string m_BundleID;
		public int m_PrefabID;


		public static BundledPrefab Create (string bundleID, int id)
		{
			BundledPrefab bundledPrefab = new GameObject (bundleID + "_" + id).AddComponent<BundledPrefab> ();
			bundledPrefab.m_BundleID = bundleID;
			bundledPrefab.m_PrefabID = id;

			return bundledPrefab;
		}


		void Awake ()
		{
			BundleManager.LoadPrefab (this);
		}


		void OnDrawGizmos ()
		{
			Gizmos.DrawIcon (transform.position, "File.png");
		}
	}
}
