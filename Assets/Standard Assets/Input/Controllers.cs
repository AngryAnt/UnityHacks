using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
using System.Collections;
using System;
using System.Collections.Generic;


namespace UnityAssets
{
	public abstract class Controller
	{
		protected int index = 0;


		public int Index { get { return index; } }


		public virtual int Select { get { return -1; } }
		public virtual int Start { get { return -1; } }
		public virtual int System { get { return -1; } }

		public virtual int L1 { get { return -1; } }
		public virtual int L2 { get { return -1; } }
		public virtual int R1 { get { return -1; } }
		public virtual int R2 { get { return -1; } }

		public virtual int LStickPress { get { return -1; } }
		public virtual int RStickPress { get { return -1; } }

		public virtual int DPadN { get { return -1; } }
		public virtual int DPadS { get { return -1; } }
		public virtual int DPadE { get { return -1; } }
		public virtual int DPadW { get { return -1; } }

		public virtual int ActionButtonN { get { return -1; } }
		public virtual int ActionButtonS { get { return -1; } }
		public virtual int ActionButtonE { get { return -1; } }
		public virtual int ActionButtonW { get { return -1; } }

		public virtual float LeftAxisMin { get { return 0.0f; } }
		public virtual float LeftAxisScale { get { return 1.0f; } }
		public virtual float RightAxisMin { get { return 0.0f; } }
		public virtual float RightAxisScale { get { return 1.0f; } }


		static List<Controller> controllers = new List<Controller> ();


		protected Controller (int index)
		{
			this.index = index;
		}


		public static string[] Axis
		{
			get
			{
				List<string> axis = new List<string> (PS3Controller.Axis);
				axis.AddRange (Xbox360Controller.Axis);
				return axis.ToArray ();
			}
		}


		#if UNITY_EDITOR
			public enum AxisType
			{
				KeyOrMouseButton = 0,
				MouseMovement = 1,
				JoystickAxis = 2
			};


			public class AxisDefinition
			{
				public string
					name,
					descriptiveName,
					descriptiveNegativeName,
					negativeButton,
					positiveButton,
					altNegativeButton,
					altPositiveButton;

				public float
					gravity,
					dead,
					sensitivity;

				public bool
					snap = false,
					invert = false;

				public AxisType type;

				public int
					axis,
					joyNum;
			}


			public static bool AxisDefined (string axisName)
			{
				SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset")[0]);
				SerializedProperty axes = serializedObject.FindProperty ("m_Axes");

				axes.Next (true);
				axes.Next (true);

				while (axes.Next (false))
				{
					SerializedProperty axis = axes.Copy ();
					axis.Next (true);

					if (axis.stringValue == axisName)
					{
						return true;
					}
				}

				return false;
			}


			public static bool AllAxesDefined
			{
				get
				{
					foreach (string axis in Axis)
					{
						if (!AxisDefined (axis))
						{
							return false;
						}
					}

					return true;
				}
			}


			public static void SetupAxes ()
			{
				PS3Controller.SetupAxes ();
				Xbox360Controller.SetupAxes ();
			}


			public static SerializedProperty GetChildProperty (SerializedProperty parent, string name)
			{
				SerializedProperty child = parent.Copy ();
				child.Next (true);

				do
				{
					if (child.name == name)
					{
						return child;
					}
				}
				while (child.Next (false));

				return null;
			}


			public static void AddAxis (AxisDefinition axis)
			{
				if (AxisDefined (axis.name))
				{
					throw new System.ArgumentException (string.Format ("Specified axis \"{0}\" already exists", axis.name));
				}

				SerializedObject serializedObject = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/InputManager.asset")[0]);
				SerializedProperty axesProperty = serializedObject.FindProperty ("m_Axes");

				axesProperty.arraySize++;
				serializedObject.ApplyModifiedProperties ();

				SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex (axesProperty.arraySize - 1);

				GetChildProperty (axisProperty, "m_Name").stringValue = axis.name;
				GetChildProperty (axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
				GetChildProperty (axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
				GetChildProperty (axisProperty, "negativeButton").stringValue = axis.negativeButton;
				GetChildProperty (axisProperty, "positiveButton").stringValue = axis.positiveButton;
				GetChildProperty (axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
				GetChildProperty (axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
				GetChildProperty (axisProperty, "gravity").floatValue = axis.gravity;
				GetChildProperty (axisProperty, "dead").floatValue = axis.dead;
				GetChildProperty (axisProperty, "sensitivity").floatValue = axis.sensitivity;
				GetChildProperty (axisProperty, "snap").boolValue = axis.snap;
				GetChildProperty (axisProperty, "invert").boolValue = axis.invert;
				GetChildProperty (axisProperty, "type").intValue = (int)axis.type;
				GetChildProperty (axisProperty, "axis").intValue = axis.axis - 1;
				GetChildProperty (axisProperty, "joyNum").intValue = axis.joyNum;

				serializedObject.ApplyModifiedProperties ();
			}
		#endif


		public static int Count
		{
			get
			{
				return Input.GetJoystickNames ().Length;
			}
		}


		public static Type ControllerType (int index)
		{
			string[] names = Input.GetJoystickNames ();

			if (index < 1 || index > names.Length)
			{
				return null;
			}

			string name = names[index - 1];
			if (name == "Sony PLAYSTATION(R)3 Controller")
			{
				return typeof (PS3Controller);
			}
			else if (name == "Controller (XBOX 360 For Windows)")
			{
				return typeof (Xbox360Controller);
			}
			else
			{
				return null;
			}
		}


		static Controller CreateController (int index)
		{
			Type type = ControllerType (index);

			if (type == null)
			{
				return null;
			}

			if (type == typeof (PS3Controller))
			{
				return new PS3Controller (index);
			}
			else if (type == typeof (Xbox360Controller))
			{
				return new Xbox360Controller (index);
			}

			throw new ApplicationException ("Internally unhandled controller type: " + type.Name);
		}


		public static Controller Get (int index)
		{
			for (int i = Mathf.Max (1, controllers.Count); i <= index; i++)
			// Make sure that the controllers list if populated up to and including the given index
			{
				controllers.Add (CreateController (i));
			}

			Controller controller = controllers[index - 1];
			Type currentType = ControllerType (index);

			if (controller == null && currentType != null ||
				controller != null && controller.GetType () != currentType)
			// This controller reference is out of date (previous null is no longer null or type changed)
			{
				controller = CreateController (index);
				controllers[index - 1] = controller;
			}

			return controller;
		}


		static string GetButtonName (int joystick, int button)
		{
			if (button < 0)
			{
				return "Unsupported button";
			}

			return string.Format ("joystick {0} button {1}", joystick, button);
		}


		public virtual bool GetButton (int button)
		{
			return Input.GetKey (GetButtonName (index, button));
		}


		public virtual bool GetButtonDown (int button)
		{
			return Input.GetKeyDown (GetButtonName (index, button));
		}


		public virtual bool GetButtonUp (int button)
		{
			return Input.GetKeyUp (GetButtonName (index, button));
		}


		public virtual void OnGUI ()
		{
			AxisLabel ("Horizontal");
			AxisLabel ("Vertical");
			AxisLabel (GetType ().Name + "RightX");
			AxisLabel (GetType ().Name + "RightY");

			MappingLabel ("Select", Select);
			MappingLabel ("Start", Start);
			MappingLabel ("System", System);
			MappingLabel ("L1", L1);
			MappingLabel ("L2", L2);
			MappingLabel ("R1", R1);
			MappingLabel ("R2", R2);
			MappingLabel ("LStickPress", LStickPress);
			MappingLabel ("RStickPress", RStickPress);
			MappingLabel ("DPadN", DPadN);
			MappingLabel ("DPadS", DPadS);
			MappingLabel ("DPadE", DPadE);
			MappingLabel ("DPadW", DPadW);
			MappingLabel ("ActionButtonN", ActionButtonN);
			MappingLabel ("ActionButtonS", ActionButtonS);
			MappingLabel ("ActionButtonE", ActionButtonE);
			MappingLabel ("ActionButtonW", ActionButtonW);
		}


		protected void MappingLabel (string name, int value)
		{
			GUILayout.Label (string.Format ("{0}: {1}", name, GetButton (value)));
		}


		protected void AxisLabel (string name)
		{
			GUILayout.Label (string.Format ("{0}: {1:f2}", name, Input.GetAxis (name)));
		}
	}


	public class Xbox360Controller : Controller
	{
		new public static string[] Axis
		{
			get
			{
				return new string[]
				{
					typeof (Xbox360Controller).Name + "RightX",
					typeof (Xbox360Controller).Name + "RightY",
					typeof (Xbox360Controller).Name + "DPadX",
					typeof (Xbox360Controller).Name + "DPadY",
					typeof (Xbox360Controller).Name + "Triggers"
				};
			}
		}


		#if UNITY_EDITOR
			new public static void SetupAxes ()
			{
				if (!AxisDefined ((typeof (Xbox360Controller)).Name + "RightX"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (Xbox360Controller)).Name + "RightX",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 4,
						joyNum = 0
					});
				}

				if (!AxisDefined ((typeof (Xbox360Controller)).Name + "RightY"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (Xbox360Controller)).Name + "RightY",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 5,
						joyNum = 0
					});
				}

				if (!AxisDefined ((typeof (Xbox360Controller)).Name + "DPadX"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (Xbox360Controller)).Name + "DPadX",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 6,
						joyNum = 0
					});
				}

				if (!AxisDefined ((typeof (Xbox360Controller)).Name + "DPadY"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (Xbox360Controller)).Name + "DPadY",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 7,
						joyNum = 0
					});
				}

				if (!AxisDefined ((typeof (Xbox360Controller)).Name + "Triggers"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (Xbox360Controller)).Name + "Triggers",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 3,
						joyNum = 0
					});
				}
			}
		#endif


		Dictionary<int, int>
			buttonPressFrames = new Dictionary<int, int> (),
			buttonReleaseFrames = new Dictionary<int, int> ();


		public Xbox360Controller (int index, bool force = false) : base (index)
		{
			Type type = ControllerType (index);

			if (!force && type != typeof (Xbox360Controller))
			{
				throw new ArgumentException (
					string.Format (
						"Tried to force create a 360 controller mapping to controller {0} when actual controller type is {1}",
						index,
						type
					)
				);
			}
		}


		public override int Select { get { return 6; } }
		public override int Start { get { return 7; } }
		public override int System { get { return 8; } }

		public override int L1 { get { return 4; } }
		public override int L2 { get { return -1; } }
		public override int R1 { get { return 5; } }
		public override int R2 { get { return -2; } }

		public override int LStickPress { get { return 8; } }
		public override int RStickPress { get { return 9; } }

		public override int DPadN { get { return -3; } }
		public override int DPadS { get { return -4; } }
		public override int DPadE { get { return -5; } }
		public override int DPadW { get { return -6; } }

		public override int ActionButtonN { get { return 3; } }
		public override int ActionButtonS { get { return 0; } }
		public override int ActionButtonE { get { return 1; } }
		public override int ActionButtonW { get { return 2; } }

		public override float LeftAxisMin { get { return 0.5f; } }
		public override float LeftAxisScale { get { return 3.5f; } }
		public override float RightAxisMin { get { return 0.5f; } }
		public override float RightAxisScale { get { return 3.5f; } }


		public override bool GetButton (int button)
		{
			if (button < 0)
			{
				bool down = false;

				switch (button)
				{
					case -3:	// DPadN
						down = Input.GetAxis (GetType ().Name + "DPadY") > 0;
					break;
					case -4:	// DPadS
						down = Input.GetAxis (GetType ().Name + "DPadY") < 0;
					break;
					case -5:	// DPadE
						down = Input.GetAxis (GetType ().Name + "DPadX") > 0;
					break;
					case -6:	// DPadW
						down = Input.GetAxis (GetType ().Name + "DPadX") < 0;
					break;
					case -1:	// L2
						down = Input.GetAxis (GetType ().Name + "Triggers") > 0;
					break;
					case -2:	// R2
						down = Input.GetAxis (GetType ().Name + "Triggers") < 0;
					break;
					default:
						return false;
				}

				if (down)
				{
					if (!buttonPressFrames.ContainsKey (button) || buttonPressFrames[button] == -1)
					{
						buttonPressFrames[button] = Time.frameCount;
					}

					buttonReleaseFrames[button] = -1;

					return true;
				}
				else
				{
					if (!buttonReleaseFrames.ContainsKey (button) || buttonReleaseFrames[button] == -1)
					{
						buttonReleaseFrames[button] = Time.frameCount;
					}

					buttonPressFrames[button] = -1;

					return false;
				}
			}

			return base.GetButton (button);
		}


		public override bool GetButtonUp (int button)
		{
			if (button < 0)
			{
				if (GetButton (button))
				{
					return false;
				}
				else
				{
					return !buttonReleaseFrames.ContainsKey (button) || buttonReleaseFrames[button] == Time.frameCount - 1;
				}
			}

			return base.GetButtonUp (button);
		}


		public override bool GetButtonDown (int button)
		{
			if (button < 0)
			{
				if (!GetButton (button))
				{
					return false;
				}
				else
				{
					return !buttonPressFrames.ContainsKey (button) || buttonPressFrames[button] == Time.frameCount - 1;
				}
			}

			return base.GetButtonDown (button);
		}


		public override void OnGUI ()
		{
			AxisLabel (GetType ().Name + "DPadX");
			AxisLabel (GetType ().Name + "DPadY");
			AxisLabel (GetType ().Name + "Triggers");

			base.OnGUI ();
		}


		public override string ToString ()
		{
			return "Microsoft Xbox 360 controller";
		}
	}


	public class PS3Controller : Controller
	{
		new public static string[] Axis
		{
			get
			{
				return new string[]
				{
					typeof (PS3Controller).Name + "RightX",
					typeof (PS3Controller).Name + "RightY"
				};
			}
		}


		#if UNITY_EDITOR
			new public static void SetupAxes ()
			{
				if (!AxisDefined ((typeof (PS3Controller)).Name + "RightX"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (PS3Controller)).Name + "RightX",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 3,
						joyNum = 0
					});
				}

				if (!AxisDefined ((typeof (PS3Controller)).Name + "RightY"))
				{
					AddAxis (new AxisDefinition ()
					{
						name = (typeof (PS3Controller)).Name + "RightY",
						dead = 0.19f,
						sensitivity = 1.0f,
						type = AxisType.JoystickAxis,
						axis = 4,
						joyNum = 0
					});
				}
			}
		#endif


		public PS3Controller (int index, bool force = false) : base (index)
		{
			Type type = ControllerType (index);

			if (!force && type != typeof (PS3Controller))
			{
				throw new ArgumentException (
					string.Format (
						"Tried to force create a PS3 controller mapping to controller {0} when actual controller type is {1}",
						index,
						type
					)
				);
			}
		}


		public override int Select { get { return 0; } }
		public override int Start { get { return 3; } }
		public override int System { get { return 16; } }

		public override int L1 { get { return 10; } }
		public override int L2 { get { return 8; } }
		public override int R1 { get { return 11; } }
		public override int R2 { get { return 9; } }

		public override int LStickPress { get { return 1; } }
		public override int RStickPress { get { return 2; } }

		public override int DPadN { get { return 4; } }
		public override int DPadS { get { return 6; } }
		public override int DPadE { get { return 5; } }
		public override int DPadW { get { return 7; } }

		public override int ActionButtonN { get { return 12; } }
		public override int ActionButtonS { get { return 14; } }
		public override int ActionButtonE { get { return 13; } }
		public override int ActionButtonW { get { return 15; } }

		public override float LeftAxisMin { get { return 0.2f; } }
		public override float LeftAxisScale { get { return 3.5f; } }
		public override float RightAxisMin { get { return 0.2f; } }
		public override float RightAxisScale { get { return 3.5f; } }


		public override string ToString ()
		{
			return "Sony Playstation 3 controller";
		}
	}
}
