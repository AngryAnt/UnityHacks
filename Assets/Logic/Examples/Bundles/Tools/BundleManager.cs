using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;


namespace CrossPlatformTools
{
	public class BundleManager : MonoBehaviour
	{
		class BundleRequest
		{
			WWW m_WWW;


			public BundleRequest (WWW www)
			{
				m_WWW = www;
			}


			public AssetBundle Bundle
			{
				get
				{
					return !m_WWW.isDone || !string.IsNullOrEmpty (m_WWW.error) ?
						null
					:
						m_WWW.assetBundle;
				}
			}


			public IEnumerator Wait ()
			{
				if (m_WWW.isDone)
				{
					yield break;
				}
				else
				{
					yield return m_WWW;
				}
			}


			public static implicit operator AssetBundle (BundleRequest request)
			{
				return request.Bundle;
			}
		}


		public delegate void TypeLoadedHandler (string requestedTypeName, Type type);


		static BundleManager Instance
		{
			get
			{
				BundleManager instance = (BundleManager)FindObjectOfType (typeof (BundleManager));

				if (instance == null)
				{
					instance = new GameObject ("BundleManager").AddComponent<BundleManager> ();
				}

				return instance;
			}
		}


		public string BundleRootURL
		{
			get
			{
				return "file://" + Application.dataPath + "/../";
			}
		}


		public static void LoadType (string bundleID, string typeName, TypeLoadedHandler resultHandler)
		{
			Instance.StartCoroutine (Instance.DoLoadType (bundleID, typeName, resultHandler));
		}


		public static void LoadPrefab (BundledPrefab bundledPrefab)
		{
			Instance.StartCoroutine (Instance.DoLoadPrefab (bundledPrefab));
		}


		Dictionary<string, BundleRequest> m_Bundles = new Dictionary<string, BundleRequest> ();


		IEnumerator DoLoadType (string bundleID, string typeName, TypeLoadedHandler resultHandler)
		{
			yield return StartCoroutine (DoLoadBundle (bundleID));

			Assembly assembly = Assembly.Load (((TextAsset)m_Bundles[bundleID].Bundle.mainAsset).bytes);

			resultHandler (typeName, assembly.GetType (typeName));
		}


		IEnumerator DoLoadPrefab (BundledPrefab bundledPrefab)
		{
			yield return StartCoroutine (DoLoadBundle (bundledPrefab.m_BundleID));

			PrefabBundleManifest manifest = (PrefabBundleManifest)m_Bundles[bundledPrefab.m_BundleID].Bundle.mainAsset;

			if (manifest == null)
			{
				Debug.LogError ("Found no manifest for bundle " + bundledPrefab.m_BundleID);

				yield break;
			}

			Instantiate (
				manifest.GetPrefab (bundledPrefab.m_PrefabID),
				bundledPrefab.transform.position,
				bundledPrefab.transform.rotation
			);

			// TODO: Copy over parenting, local transformation and game object data
		}


		IEnumerator DoLoadBundle (string bundleID)
		{
			if (!m_Bundles.ContainsKey (bundleID))
			{
				m_Bundles[bundleID] = new BundleRequest (
					new WWW (BundleRootURL + bundleID + ".unity3d")
				);
			}

			if (m_Bundles[bundleID].Bundle == null)
			{
				yield return StartCoroutine (m_Bundles[bundleID].Wait ());

				if (m_Bundles[bundleID].Bundle == null)
				{
					Debug.LogError ("Bundle request failed: " + bundleID);

					yield break;
				}
			}
		}
	}
}
