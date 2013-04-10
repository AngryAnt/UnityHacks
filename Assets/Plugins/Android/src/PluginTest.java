package com.unity3d.UnityHacks;


import com.unity3d.player.*;


public class PluginTest
{
	private static String s_ControllerName;


	public static void Start (String controllerName)
	{
		s_ControllerName = controllerName;
	}


	public static void Bark ()
	{
		MessageController ("JavaLog", "Woof!");
	}


	private static void MessageController (String function, String message)
	{
		UnityPlayer.UnitySendMessage (s_ControllerName, function, message);
	}
}
