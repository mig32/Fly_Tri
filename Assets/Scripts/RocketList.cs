using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public static class RocketList 
{
	private static Dictionary<string, string> rocketList = new Dictionary<string, string>();
	private static Dictionary<string, string> engineList = new Dictionary<string, string>();
	
	public static string getRocketPath(string name)
	{
		if (rocketList.ContainsKey(name))
		{
			return rocketList[name];
		}
		else
		{
			Debug.Log ("Rocket with name:" + name + " doesnt exist");
			return null;
		}
	}

	public static string getEnginePath(string name)
	{
		if (engineList.ContainsKey(name))
		{
			return engineList[name];
		}
		else
		{
			Debug.Log ("Engine with name:" + name + " doesnt exist");
			return null;
		}
	}
	
	public static int getRocketsCount()
	{
		return rocketList.Count;
	}

	public static int getEnginesCount()
	{
		return engineList.Count;
	}
	
	public static bool addRocket(string name, string rocketPath)
	{
		if (rocketList.ContainsKey(name))
		{
			Debug.Log ("Rocket with name:" + name + " allready exist");
			return false;
		}
		else
		{
			rocketList.Add(name, rocketPath);
			return true;
		}
	}

	public static bool addEngine(string name, string enginePath)
	{
		if (engineList.ContainsKey(name))
		{
			Debug.Log ("Engine with name:" + name + " allready exist");
			return false;
		}
		else
		{
			engineList.Add(name, enginePath);
			return true;
		}
	}

	public static void removeRocket(string name)
	{
		rocketList.Remove (name);
	}

	public static void removeEngine(string name)
	{
		engineList.Remove (name);
	}
	
	public static void clearRocketList()
	{
		rocketList.Clear ();
		engineList.Clear ();
	}
	
	public static bool containsRocket(string name)
	{
		return rocketList.ContainsKey(name);
	}

	public static bool containsEngine(string name)
	{
		return engineList.ContainsKey(name);
	}
	
	public static string[] getRocketNamesList()
	{
		string[] result = new string[rocketList.Count];
		int i = 0;
		foreach (string key in rocketList.Keys)
		{
			result[i] = key;
			i++;
		}
		return result;
	}

	public static string[] getEngineNamesList()
	{
		string[] result = new string[engineList.Count];
		int i = 0;
		foreach (string key in engineList.Keys)
		{
			result[i] = key;
			i++;
		}
		return result;
	}
}
