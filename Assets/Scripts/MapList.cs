using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public struct MapPaths
{
	public string mapPath;
	public string logoPath;
}

public static class MapList {
	private static Dictionary<string, MapPaths> mapList = new Dictionary<string, MapPaths>();

	public static string getMapLogoPath(string name)
	{
		if (mapList.ContainsKey(name))
		{
			return mapList[name].logoPath;
		}
		else
		{
			Debug.Log ("Map with name:" + name + " doesnt exist");
			return null;
		}
	}

	public static string getMapPath(string name)
	{
		if (mapList.ContainsKey(name))
		{
			return mapList[name].mapPath;
		}
		else
		{
			Debug.Log ("Map with name:" + name + " doesnt exist");
			return null;
		}
	}

	public static int getMapsCount()
	{
		return mapList.Count;
	}

	public static bool addMap(string name, string logoPath, string mapPath)
	{
		if (mapList.ContainsKey(name))
		{
			Debug.Log ("Map with name:" + name + " allready exist");
			return false;
		}
		else
		{
			MapPaths temp;
			temp.logoPath = logoPath;
			temp.mapPath = mapPath;
			mapList.Add(name, temp);
			return true;
		}
	}

	public static void removeMap(string name)
	{
		mapList.Remove (name);
	}

	public static void clearMapList()
	{
		mapList.Clear ();
	}

	public static bool contains(string name)
	{
		return mapList.ContainsKey(name);
	}

	public static string[] getMapNamesList()
	{
		string[] result = new string[mapList.Count];
		int i = 0;
		foreach (string key in mapList.Keys)
		{
			result[i] = key;
			i++;
		}
		return result;
	}
}
