#if UNITY_WEBPLAYER || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
	#define DC_CURSOR_LOCK
#endif

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
	#define DC_TOUCH_CONTROLS
#endif

using UnityEngine;
using System.Collections;


namespace UnityAssets
{
	public partial class DualControls : MonoBehaviour
	{
		public class Stick
		{
			string name;
			float
				sensitivityMin, sensitivityDefault, sensitivityMax,
				verticalSkewMin, verticalSkewDefault, verticalSkewMax;
			bool invertXDefault, invertYDefault;


			public Stick (string name, Vector3 sensitivityDefault, Vector3 verticalSkewDefault, bool invertXDefault, bool invertYDefault)
			{
				this.name = name;
				sensitivityMin = sensitivityDefault.x;
				this.sensitivityDefault = sensitivityDefault.y;
				sensitivityMax = sensitivityDefault.z;

				verticalSkewMin = verticalSkewDefault.x;
				this.verticalSkewDefault = verticalSkewDefault.y;
				verticalSkewMax = verticalSkewDefault.z;

				this.invertXDefault = invertXDefault;
				this.invertYDefault = invertYDefault;
			}


			public string Name
			{
				get
				{
					return name;
				}
			}


			public float Sensitivity
			{
				get
				{
					return PlayerPrefs.GetFloat ("DualControls " + Name + " stick sensitivity", sensitivityDefault);
				}
				set
				{
					PlayerPrefs.SetFloat ("DualControls " + Name + " stick sensitivity", Mathf.Clamp (value, sensitivityMin, sensitivityMax));
				}
			}


			public float VerticalSkew
			{
				get
				{
					return PlayerPrefs.GetFloat ("DualControls " + Name + " stick vertical skew", verticalSkewDefault);
				}
				set
				{
					PlayerPrefs.SetFloat ("DualControls " + Name + " stick vertical skew", Mathf.Clamp (value, verticalSkewMin, verticalSkewMax));
				}
			}


			public bool InvertX
			{
				get
				{
					return PlayerPrefs.GetInt ("DualControls " + Name + " stick X invert", invertXDefault ? 1 : 0) != 0;
				}
				set
				{
					PlayerPrefs.SetInt ("DualControls " + Name + " stick X invert", value ? 1 : 0);
				}
			}


			public bool InvertY
			{
				get
				{
					return PlayerPrefs.GetInt ("DualControls " + Name + " stick Y invert", invertYDefault ? 1 : 0) != 0;
				}
				set
				{
					PlayerPrefs.SetInt ("DualControls " + Name + " stick Y invert", value ? 1 : 0);
				}
			}


			public virtual Vector2 RawValue
			{
				get
				{
					return GetRawStickValue (this);
				}
			}


			public Vector2 Value
			{
				get
				{
					Vector2 result = RawValue * Sensitivity;
					return Vector2.ClampMagnitude (new Vector2 (result.x * (InvertX ? -1.0f : 1.0f), result.y * (InvertY ? -1.0f : 1.0f) * VerticalSkew), 1.0f);
				}
			}


			public float x
			{
				get
				{
					return Value.x;
				}
			}


			public float y
			{
				get
				{
					return Value.y;
				}
			}


			public override string ToString ()
			{
				return name;
			}
		}


		public class VirtualStick : Stick
		{
			const float kVirtualStickDeadzone = 0.1f;


			float
				mobilityMin, mobilityDefault, mobilityMax,
				stickinessMin, stickinessDefault, stickinessMax,
				releaseDelayMin, releaseDelayDefault, releaseDelayMax,
				lastUpdateTime = -1.0f, releaseTime = -1.0f;
			Vector2
				startPosition,
				currentPosition;


			public VirtualStick (string name, Vector3 sensitivityDefault, Vector3 verticalSkewDefault, bool invertXDefault, bool invertYDefault, Vector3 mobilityDefault, Vector3 stickinessDefault, Vector3 releaseDelayDefault)
				: base (name, sensitivityDefault, verticalSkewDefault, invertXDefault, invertYDefault)
			{
				mobilityMin = mobilityDefault.x;
				this.mobilityDefault = mobilityDefault.y;
				mobilityMax = mobilityDefault.z;

				stickinessMin = stickinessDefault.x;
				this.stickinessDefault = stickinessDefault.y;
				stickinessMax = stickinessDefault.z;

				releaseDelayMin = releaseDelayDefault.x;
				this.releaseDelayDefault = releaseDelayDefault.y;
				releaseDelayMax = stickinessDefault.z;
			}


			public float Mobility
			{
				get
				{
					return PlayerPrefs.GetFloat ("DualControls " + Name + " stick mobility", mobilityDefault);
				}
				set
				{
					PlayerPrefs.SetFloat ("DualControls " + Name + " stick mobility", Mathf.Clamp (value, mobilityMin, mobilityMax));
				}
			}


			public float Stickiness
			{
				get
				{
					return PlayerPrefs.GetFloat ("DualControls " + Name + " stick stickiness", stickinessDefault);
				}
				set
				{
					PlayerPrefs.SetFloat ("DualControls " + Name + " stick stickiness", Mathf.Clamp (value, stickinessMin, stickinessMax));
				}
			}


			public float ReleaseDelay
			{
				get
				{
					return PlayerPrefs.GetFloat ("DualControls " + Name + " stick release delay", releaseDelayDefault);
				}
				set
				{
					PlayerPrefs.SetFloat ("DualControls " + Name + " stick release delay", Mathf.Clamp (value, releaseDelayMin, releaseDelayMax));
				}
			}


			public Vector2 StartPosition
			{
				get
				{
					return startPosition;
				}
			}


			public Vector2 CurrentPosition
			{
				get
				{
					return currentPosition;
				}
				private set
				{
					SetCurrentPosition (value, Mobility * ScreenUtility.ScaleFactor);
				}
			}


			void SetCurrentPosition (Vector2 position, float clamp)
			{
				currentPosition = startPosition + Vector2.ClampMagnitude (position - startPosition, clamp);
			}


			public override Vector2 RawValue
			{
				get
				{
					return currentPosition == startPosition ? Vector2.zero : (currentPosition - startPosition) / (Mobility * ScreenUtility.ScaleFactor);
				}
			}


			public float LastUpdateTime
			{
				get
				{
					return lastUpdateTime;
				}
			}


			public void Start (Vector2 position)
			{
				startPosition = position;
				Update (position);
			}


			public void Update (Vector2 position)
			{
				CurrentPosition = position;
				lastUpdateTime = Time.time;
				releaseTime = -1.0f;
			}


			public void Update ()
			{
				if (lastUpdateTime <= 0.0f || Time.time - lastUpdateTime < ReleaseDelay)
				{
					return;
				}

				if (releaseTime <= 0.0f)
				{
					releaseTime = Time.time;
				}

				float reducedMagnitude = RawValue.magnitude - (Time.time - releaseTime) / (Stickiness * ScreenUtility.ScaleFactor);

				if (reducedMagnitude <= kVirtualStickDeadzone)
				{
					currentPosition = startPosition;
					lastUpdateTime = -1.0f;
				}
				else
				{
					SetCurrentPosition (currentPosition, Mobility * ScreenUtility.ScaleFactor * reducedMagnitude);
				}
			}
		}
	}
}
