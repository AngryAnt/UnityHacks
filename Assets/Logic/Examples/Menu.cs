using UnityEngine;
using UnityAssets;
using System.Collections;
using System.Collections.Generic;
using System;


public class Menu : MonoBehaviour
{
	enum State
	{
		Main,
		Levels,
		Settings,
		Credits
	}


	State state = State.Main;
	Dictionary<State, Action> stateHandlers;


	void Start ()
	{
		stateHandlers = Utility.GetStateHandlers<State> (this, "On", "MenuGUI");
	}
	

	void OnGUI ()
	{
		const float kMenuWidth = 200.0f, kMenuHeight = 300.0f;

		GUILayout.BeginArea (new Rect (
			(Screen.width - kMenuWidth) * 0.5f,
			(Screen.height - kMenuHeight) * 0.5f,
			kMenuWidth,
			kMenuHeight
		), state.ToString (), GUI.skin.window);
			stateHandlers[state] ();
		GUILayout.EndArea ();
	}


	void OnMainMenuGUI ()
	{
		GUILayout.Label ("Welcome to the best game ever!");

		StateButton (State.Levels);
		StateButton (State.Settings);
		StateButton (State.Credits);

		MenuSpace ();

		if (GUILayout.Button ("Quit"))
		{
			Application.Quit ();
		}
	}


	void OnLevelsMenuGUI ()
	{
		GUILayout.Label ("Here you would find a number of levels to load");

		LevelButton ("One");
		LevelButton ("Two");
		LevelButton ("Three");

		MenuSpace ();

		StateButton (State.Main, "Back");
	}


	void OnSettingsMenuGUI ()
	{
		GUILayout.Label ("Configure the settings of this amazing game");

		MenuSpace ();
		
		StateButton (State.Main, "Back");
	}


	void OnCreditsMenuGUI ()
	{
		GUILayout.Label ("This awesome game was made by:");
		GUILayout.Label ("Mr. Pixels");
		GUILayout.Label ("That programmer guy");
		GUILayout.Label ("Senõr business");

		MenuSpace ();
		
		StateButton (State.Main, "Back");
	}


	void StateButton (State state, string label = null)
	{
		if (GUILayout.Button (label ?? state.ToString ()))
		{
			this.state = state;
		}
	}


	bool LevelButton (string name)
	{
		return GUILayout.Button (name, GUILayout.Width (50.0f), GUILayout.Height (50.0f));
	}


	void MenuSpace ()
	{
		GUILayout.FlexibleSpace ();
	}
}
