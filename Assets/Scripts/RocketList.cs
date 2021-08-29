using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public struct StringPrefabPair
{
	public StringPrefabPair(string name, GameObject prefab)
	{
		_name = name;
		_prefab = prefab;
	}

	public string _name;
	public GameObject _prefab;
};

[System.Serializable]
public struct StringPrefabPairsArray
{
	public List<StringPrefabPair> m_array;

	public GameObject GetGrefabByName(string name)
	{
		foreach (StringPrefabPair pair in m_array)
		{
			if (pair._name == name)
			{
				return pair._prefab;
			}
		}
		return null;
	}

	public bool HasName(string name)
	{
		foreach (StringPrefabPair pair in m_array)
		{
			if (pair._name == name)
			{
				return true;
			}
		}
		return false;
	}

	public bool Add(string name, GameObject prefab)
	{
		if (HasName(name))
		{
			return false;
		}

		m_array.Add(new StringPrefabPair(name, prefab));
		return true;
	}

	public int Count()
	{
		return m_array.Count;
	}

	public void Remove(string name)
	{
		for (int i = 0; i < m_array.Count; ++i)
		{
			if (m_array[i]._name == name)
			{
				m_array.RemoveAt(i);
				return;
			}
		}
	}

	public void Clear()
	{
		m_array.Clear();
	}

	public string[] GetNames()
	{
		string[] names = new string[m_array.Count];
		for (int i = 0; i < m_array.Count; ++i)
		{
			names[i] = m_array[i]._name;
		}
		return names;
	}
};

public class RocketList : MonoBehaviour
{
	private static RocketList m_instance;

	public StringPrefabPairsArray m_rockets;
	public StringPrefabPairsArray m_engines;

	public static RocketList GetInstance()
	{
		return m_instance;
	}

	private void Awake()
	{
		m_instance = this;
	}

	public GameObject GetRocketPrefab(string name)
	{
		return m_rockets.GetGrefabByName(name);
	}

	public GameObject GetEnginePrefab(string name)
	{
		return m_engines.GetGrefabByName(name);
	}
	
	public int GetRocketsCount()
	{
		return m_rockets.Count();
	}

	public int GetEnginesCount()
	{
		return m_engines.Count();
	}
	
	public bool AddRocket(string name, GameObject rocketPrefab)
	{
		return m_rockets.Add(name, rocketPrefab);
	}

	public bool AddEngine(string name, GameObject enginePrefab)
	{
		return m_engines.Add(name, enginePrefab);
	}

	public void RemoveRocket(string name)
	{
		m_rockets.Remove(name);
	}

	public void RemoveEngine(string name)
	{
		m_engines.Remove(name);
	}
	
	public void ClearRocketList()
	{
		m_rockets.Clear();
		m_engines.Clear();
	}
	
	public bool ContainsRocket(string name)
	{
		return m_rockets.HasName(name);
	}

	public bool ContainsEngine(string name)
	{
		return m_engines.HasName(name);
	}
	
	public string[] GetRocketNamesList()
	{
		return m_rockets.GetNames();
	}

	public string[] GetEngineNamesList()
	{
		return m_engines.GetNames();
	}
}
