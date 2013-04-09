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
		public delegate bool ConnectControllerCallback (Controller newController);


		static DualControls instance = null;


		static DualControls Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType (typeof (DualControls)) as DualControls;

					if (instance == null)
					{
						instance = (new GameObject ("DualControls")).AddComponent<DualControls> ();
					}
				}

				return instance;
			}
		}


		public static bool Available
		{
			get
			{
				return Instance != null;
			}
		}


		public static ConnectControllerCallback connectControllerCallback;


		public bool
			autoConnectController = true,
			requireCursorLock = false;
		public float
			inputSensitivityMin = 1.0f,
			inputSensitivityMax = 2.0f,
			inputSensitivityDefault = 1.0f,

			inputVerticalSkewMin = 1.0f,
			inputVerticalSkewMax = 2.0f,
			#if DC_TOUCH_CONTROLS
				inputVerticalSkewDefault = 1.5f,
			#else
				inputVerticalSkewDefault = 1.3f,
			#endif

			windowsMouseSensitivityScale = 4.0f,

			inputMobilityMin = 10.0f,
			inputMobilityDefault = 50.0f,
			inputMobilityMax = 100.0f,

			inputStickinessMin = 0.1f,
			inputStickinessDefault = 0.5f,
			inputStickinessMax = 1.0f,

			inputReleaseDelayMin = 0.0f,
			inputReleaseDelayDefault = 0.1f,
			inputReleaseDelayMax = 1.0f;


		bool controlsEnabled = true;
		Stick leftStick, rightStick;
		Controller activeController;


		public static Controller ActiveController
		{
			get
			{
				return Instance.activeController;
			}
			set
			{
				bool didChange = Instance.activeController != value;

				Instance.activeController = value;

				if (didChange)
				{
					ResetSticks ();
				}
			}
		}


		public static bool Enabled
		{
			get
			{
				#if DC_CURSOR_LOCK
					return (ActiveController != null || !Instance.requireCursorLock || Screen.lockCursor) && Available && Instance.controlsEnabled && Instance.enabled;
				#else
					return Available && Instance.controlsEnabled;
				#endif
			}
			set
			{
				#if DC_CURSOR_LOCK
					if (ActiveController == null && Instance.requireCursorLock && value)
					{
						Screen.lockCursor = value;
					}
				#endif

				Instance.controlsEnabled = value;
			}
		}


		public static Stick Left
		{
			get
			{
				return Instance.leftStick;
			}
		}


		public static Stick Right
		{
			get
			{
				return Instance.rightStick;
			}
		}


		public static bool Interaction
		{
			get
			{
				return Left.Value.magnitude + Right.Value.magnitude > 0.0f;
			}
		}


		public static float InputSensitivityMin { get { return Instance.inputSensitivityMin; } set { Instance.inputSensitivityMin = value; } }
		public static float InputSensitivityMax { get { return Instance.inputSensitivityMax; } set { Instance.inputSensitivityMax = value; } }
		public static float InputSensitivityDefault { get { return Instance.inputSensitivityDefault; } set { Instance.inputSensitivityDefault = value; } }

		public static float InputVerticalSkewMin { get { return Instance.inputVerticalSkewMin; } set { Instance.inputVerticalSkewMin = value; } }
		public static float InputVerticalSkewMax { get { return Instance.inputVerticalSkewMax; } set { Instance.inputVerticalSkewMax = value; } }
		public static float InputVerticalSkewDefault { get { return Instance.inputVerticalSkewDefault; } set { Instance.inputVerticalSkewDefault = value; } }

		public static float WindowsMouseSensitivityScale { get { return Instance.windowsMouseSensitivityScale; } set { Instance.windowsMouseSensitivityScale = value; } }

		public static float InputMobilityMin { get { return Instance.inputMobilityMin; } set { Instance.inputMobilityMin = value; } }
		public static float InputMobilityDefault { get { return Instance.inputMobilityDefault; } set { Instance.inputMobilityDefault = value; } }
		public static float InputMobilityMax { get { return Instance.inputMobilityMax; } set { Instance.inputMobilityMax = value; } }

		public static float InputStickinessMin { get { return Instance.inputStickinessMin; } set { Instance.inputStickinessMin = value; } }
		public static float InputStickinessDefault { get { return Instance.inputStickinessDefault; } set { Instance.inputStickinessDefault = value; } }
		public static float InputStickinessMax { get { return Instance.inputStickinessMax; } set { Instance.inputStickinessMax = value; } }

		public static float InputReleaseDelayMin { get { return Instance.inputReleaseDelayMin; } set { Instance.inputReleaseDelayMin = value; } }
		public static float InputReleaseDelayDefault { get { return Instance.inputReleaseDelayDefault; } set { Instance.inputReleaseDelayDefault = value; } }
		public static float InputReleaseDelayMax { get { return Instance.inputReleaseDelayMax; } set { Instance.inputReleaseDelayMax = value; } }


		static Vector2 GetRawStickValue (Stick stick)
		{
			if (!Enabled)
			{
				return Vector3.zero;
			}

			Vector2 input;
			Controller activeController = Instance.activeController;

			if (stick is VirtualStick)
			{
				throw new System.ApplicationException ("No external raw value for virtual sticks");
			}
			else if (stick == Instance.leftStick)
			{
				input = new Vector2 (
					Input.GetAxis ("Horizontal"),
					Input.GetAxis ("Vertical")
				);

				if (activeController != null)
				{
					input = input.magnitude < activeController.LeftAxisMin ? Vector2.zero : input * activeController.LeftAxisScale;
				}

				return input;
			}
			else if (stick == Instance.rightStick)
			{
				if (Instance.activeController != null)
				{
					string axisName = Instance.activeController.GetType ().Name;

					input = new Vector2 (
						Input.GetAxis (axisName + "RightX"),
						Input.GetAxis (axisName + "RightY")
					);

					input = input.magnitude < activeController.RightAxisMin ? Vector2.zero : input * activeController.RightAxisScale;

					return input;
				}
				else
				{
					return new Vector2 (
						Input.GetAxis ("Mouse X"),
						-Input.GetAxis ("Mouse Y")
					) * (Utility.IsWindows ? WindowsMouseSensitivityScale : 1.0f);
				}
			}

			return Vector2.zero;
		}


		public static void ResetSticks ()
		{
			#if DC_TOUCH_CONTROLS
				Instance.leftStick = new VirtualStick (
					"Virtual Left",
					new Vector3 (InputSensitivityMin, InputSensitivityDefault, InputSensitivityMax),
					new Vector3 (InputVerticalSkewMin, InputVerticalSkewDefault, InputVerticalSkewMax),
					false,
					false,
					new Vector3 (InputMobilityMin, InputMobilityDefault, InputMobilityMax),
					new Vector3 (InputStickinessMin, InputStickinessDefault, InputStickinessMax),
					new Vector3 (InputReleaseDelayMin, InputReleaseDelayDefault, InputReleaseDelayMax)
				);

				Instance.rightStick = new VirtualStick (
					"Virtual Right",
					new Vector3 (InputSensitivityMin, InputSensitivityDefault, InputSensitivityMax),
					new Vector3 (InputVerticalSkewMin, InputVerticalSkewDefault, InputVerticalSkewMax),
					false,
					false,
					new Vector3 (InputMobilityMin, InputMobilityDefault, InputMobilityMax),
					new Vector3 (InputStickinessMin, InputStickinessDefault, InputStickinessMax),
					new Vector3 (InputReleaseDelayMin, InputReleaseDelayDefault, InputReleaseDelayMax)
				);
			#else
				string controllerName = ActiveController == null ? "" : ActiveController.ToString () + " ";

				Instance.leftStick = new Stick (
					controllerName + "Left",
					new Vector3 (InputSensitivityMin, InputSensitivityDefault, InputSensitivityMax),
					new Vector3 (InputVerticalSkewMin, InputVerticalSkewDefault, InputVerticalSkewMax),
					false,
					false
				);

				Instance.rightStick = new Stick (
					controllerName + "Right",
					new Vector3 (InputSensitivityMin, InputSensitivityDefault, InputSensitivityMax),
					new Vector3 (InputVerticalSkewMin, InputVerticalSkewDefault, InputVerticalSkewMax),
					false,
					false
				);
			#endif
		}


		#if UNITY_EDITOR
			void Reset ()
			{
				if (
					!Controller.AllAxesDefined &&
					UnityEditor.EditorUtility.DisplayDialog (
						"Configure input?",
						"To support external controllers, additional axes need to be configured in your input settings.\n\nWould you like to have these added automatically?",
						"Add",
						"Do not add"
					)
				)
				{
					Controller.SetupAxes ();
				}
			}
		#endif


		void Awake ()
		{
			if (!Utility.RequireSingleton (this))
			{
				return;
			}
			ResetSticks ();
		}


		void OnDisable ()
		{
			if (instance == this)
			{
				instance = null;
			}
		}


		void Update ()
		{
			if (autoConnectController && Controller.Count > 0)
			{
				for (int i = 1; i <= Controller.Count; i++)
				{
					if (Controller.ControllerType (i) != null)
					{
						Controller controller = Controller.Get (i);

						if (connectControllerCallback == null || connectControllerCallback (controller))
						{
							ActiveController = controller;
						}

						autoConnectController = false;

						break;
					}
				}
			}

			#if DC_TOUCH_CONTROLS
				if (Enabled)
				{
					for (int i = 0; i < 2 && i < Input.touches.Length; i++)
					{
						Touch touch = Input.touches[i];
						VirtualStick stick;

						if (touch.position.x < Screen.width * 0.5f)
						{
							stick = (VirtualStick)leftStick;
						}
						else
						{
							stick = (VirtualStick)rightStick;
						}

						if (touch.phase == TouchPhase.Began)
						{
							stick.Start (touch.position);
						}
						else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
						{
							stick.Update (touch.position);
						}
					}
				}

				if (((VirtualStick)leftStick).LastUpdateTime != Time.time)
				{
					((VirtualStick)leftStick).Update ();
				}

				if (((VirtualStick)rightStick).LastUpdateTime != Time.time)
				{
					((VirtualStick)rightStick).Update ();
				}
			#endif
		}
	}
}
