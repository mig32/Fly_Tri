using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameProgress
{
	public static Dictionary<int, bool> maps = new Dictionary<int, bool>();

	public static void Save()
	{
		string temp = "";
		foreach (int mapIdx in maps.Keys)
		{
			temp += mapIdx + "\n" + (maps[mapIdx] ? "1" : "0") + "\n";
		}
		PlayerPrefs.SetString("maps", temp);

		PlayerPrefs.Save();
	}

	public static void Load()
	{
		Init();
		if (PlayerPrefs.HasKey("maps"))
		{
			string[] temp = PlayerPrefs.GetString("maps").Split('\n');
			for (int i = 0; i < temp.Length - 1; i += 2)
			{
				//Debug.Log(temp[i] + temp[i+1]);
				maps.Add(Int32.Parse(temp[i]), (temp[i + 1] == "1"));
			}
		}
	}

	public static void Clear()
	{
		PlayerPrefs.DeleteAll();
		Init();
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.Init();
		}
	}

	private static void Init()
	{
		maps.Clear();
	}

	public static void SetMap(int mapIdx, bool isCompleted)
	{
		maps[mapIdx] = isCompleted;
	}
}
