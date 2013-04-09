using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public class ScreenUtility
	{
		static float scaleFactor = 1.0f, targetDPI = 0.0f;
		static int targetScreenWidth = 0;


		public static float TargetDPI
		{
			get
			{
				return targetDPI;
			}
			set
			{
				targetDPI = Mathf.Abs (value);
				targetScreenWidth = 0;

				if (targetDPI != 0.0f && Screen.dpi != 0.0f)
				{
					scaleFactor = Screen.dpi / targetDPI;
				}
				else
				{
					scaleFactor = 1.0f;
				}
			}
		}


		public static int TargetScreenWidth
		{
			get
			{
				return targetScreenWidth;
			}
			set
			{
				targetScreenWidth = Mathf.Abs (value);
				targetDPI = 0.0f;

				if (targetScreenWidth != 0)
				{
					scaleFactor = Screen.width / (float)targetScreenWidth;
				}
				else
				{
					scaleFactor = 1.0f;
				}
			}
		}


		public static float ScaleFactor
		{
			get
			{
				return scaleFactor;
			}
		}


		public static void SetTarget (float dpi, int screenWidth)
		{
			if (Screen.dpi != 0.0f)
			{
				TargetDPI = dpi;
			}
			else
			{
				TargetScreenWidth = screenWidth;
			}
		}


		public static Vector2 ScalePosition (Vector2 position)
		{
			return new Vector2 (
				position.x / (Screen.width * scaleFactor),
				position.y / (Screen.height * scaleFactor)
			);
		}


		public static Vector2 ScaleSize (Vector2 size)
		{
			return size * scaleFactor;
		}


		public static Rect ScaleRect (Rect rect)
		{
			return new Rect (
				rect.x / Screen.width * scaleFactor,
				rect.y / Screen.height * scaleFactor,
				rect.width * scaleFactor,
				rect.height * scaleFactor
			);
		}


		public static void ScaleGUI (TextAnchor anchor = TextAnchor.MiddleCenter, bool allowDownscale = true)
		{
			if (!allowDownscale && scaleFactor < 1.0f)
			{
				return;
			}

			Vector2 pivot;

			switch (anchor)
			{
				case TextAnchor.UpperLeft:
					pivot = new Vector2 (0.0f, 0.0f);
				break;
				case TextAnchor.UpperCenter:
					pivot = new Vector2 (Screen.width * 0.5f, 0.0f);
				break;
				case TextAnchor.UpperRight:
					pivot = new Vector2 (Screen.width, 0.0f);
				break;
				case TextAnchor.MiddleLeft:
					pivot = new Vector2 (0.0f, Screen.height * 0.5f);
				break;
				case TextAnchor.MiddleCenter:
					pivot = new Vector2 (Screen.width * 0.5f, Screen.height * 0.5f);
				break;
				case TextAnchor.MiddleRight:
					pivot = new Vector2 (Screen.width, Screen.height * 0.5f);
				break;
				case TextAnchor.LowerLeft:
					pivot = new Vector2 (0.0f, Screen.height);
				break;
				case TextAnchor.LowerCenter:
					pivot = new Vector2 (Screen.width * 0.5f, Screen.height);
				break;
				case TextAnchor.LowerRight:
					pivot = new Vector2 (Screen.width, Screen.height);
				break;
				default:
					throw new System.ArgumentException ("Invalid anchor: " + anchor);
			}

			GUIUtility.ScaleAroundPivot (new Vector2 (scaleFactor, scaleFactor), pivot);
		}


		public static void DebugGUI ()
		{
			GUILayout.Box (string.Format (
@"ScaleFactor: {0}
TargetDPI: {1}
DPI: {2}
TargetScreenWidth: {3}
Screen with: {4}",
				ScaleFactor,
				TargetDPI,
				Screen.dpi,
				TargetScreenWidth,
				Screen.width
			));
		}
	}
}

