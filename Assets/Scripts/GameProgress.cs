using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class GameProgress
{
	public static Dictionary<int, bool> maps = new Dictionary<int, bool>();
	public static Dictionary<string, RocketInfo> buyedRockets = new Dictionary<string, RocketInfo>();
	public static Dictionary<string, EngineInfo> buyedEngines = new Dictionary<string, EngineInfo>();

	public static void Save()
	{
		string temp = "";
		foreach (int mapIdx in maps.Keys)
		{
			temp += mapIdx + "\n" + (maps[mapIdx] ? "1" : "0") + "\n";
		}
		PlayerPrefs.SetString("maps", temp);

		temp = "";
		RocketInfo rInfo;
		foreach (string rocket in buyedRockets.Keys)
		{
			rInfo = buyedRockets[rocket];
			temp += rocket + "\n" + rInfo.current_max_agility + "\n" + rInfo.agility + "\n" + rInfo.current_min_agility + "\n";
			temp += rInfo.current_max_mass + "\n" + rInfo.mass + "\n" + rInfo.current_min_mass + "\n";
		}
		PlayerPrefs.SetString("buyedRockets", temp);

		temp = "";
		EngineInfo eInfo;
		foreach (string engine in buyedEngines.Keys)
		{
			eInfo = buyedEngines[engine];
			temp += engine + "\n" + eInfo.current_max_power + "\n" + eInfo.enginePower + "\n" + eInfo.current_min_power + "\n";
		}
		PlayerPrefs.SetString("buyedEngines", temp);

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

		if (PlayerPrefs.HasKey("buyedRockets"))
		{
			string[] temp = PlayerPrefs.GetString("buyedRockets").Split('\n');
			for (int i = 0; i < temp.Length - 1; i += 7)
			{
				RocketInfo rInfo = new RocketInfo();
				rInfo.current_max_agility = float.Parse(temp[i + 1]);
				rInfo.agility = float.Parse(temp[i + 2]);
				rInfo.current_min_agility = float.Parse(temp[i + 3]);
				rInfo.current_max_mass = float.Parse(temp[i + 4]);
				rInfo.mass = float.Parse(temp[i + 5]);
				rInfo.current_min_mass = float.Parse(temp[i + 6]);
				buyedRockets.Add(temp[i], rInfo);
			}
		}

		if (PlayerPrefs.HasKey("buyedEngines"))
		{
			string[] temp = PlayerPrefs.GetString("buyedEngines").Split('\n');
			for (int i = 0; i < temp.Length - 1; i += 4)
			{
				EngineInfo eInfo = new EngineInfo();
				eInfo.current_max_power = float.Parse(temp[i + 1]);
				eInfo.enginePower = float.Parse(temp[i + 2]);
				eInfo.current_min_power = float.Parse(temp[i + 3]);
				buyedEngines.Add(temp[i], eInfo);
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
		buyedRockets.Clear();
		buyedEngines.Clear();
	}

	public static void setBuyedRocket(string rocketName, RocketInfo rocketInfo)
	{
		if (buyedRockets.ContainsKey(rocketName))
		{
			buyedRockets[rocketName] = rocketInfo;
		}
		else
		{
			buyedRockets.Add(rocketName, rocketInfo);
		}
	}

	public static void setBuyedEngine(string engineName, EngineInfo engineInfo)
	{
		if (buyedEngines.ContainsKey(engineName))
		{
			buyedEngines[engineName] = engineInfo;
		}
		else
		{
			buyedEngines.Add(engineName, engineInfo);
		}
	}

	public static void SetMap(int mapIdx, bool isCompleted)
	{
		maps[mapIdx] = isCompleted;
	}
}
