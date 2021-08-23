using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MapPaths
{
	public string Name;
	public string PrefabPath;
	public string IconPath;

	public Sprite GetMapIcon()
	{
		return Resources.Load<Sprite>(IconPath);
	}

	public GameObject GetMapPrefab()
	{
		return Resources.Load<GameObject>(PrefabPath);
	}
}

public class MapList : MonoBehaviour
{
	[SerializeField] private List<MapPaths> m_mapList = new List<MapPaths>();

	public int Count => m_mapList.Count;

	public MapPaths? GetMapPaths(int idx)
	{
		if (idx >= m_mapList.Count)
		{
			Debug.Assert(false, $"MapList out of range. Requested idx = {idx}, maps amount = {m_mapList.Count}");
			return null;
		}

		return m_mapList[idx];
	}
}
