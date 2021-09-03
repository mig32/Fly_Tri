using UnityEngine;
using System.Collections.Generic;

public class RocketList : MonoBehaviour
{
	[SerializeField] private List<RocketControl> m_rockets;
	[SerializeField] private List<EngineControl> m_engines;

	public RocketControl GetRocketPrefab(int idx)
	{
		if (idx >= m_rockets.Count)
		{
			return null;
		}

		return m_rockets[idx];
	}

	public EngineControl GetEnginePrefab(int idx)
	{
		if (idx >= m_engines.Count)
		{
			return null;
		}

		return m_engines[idx];
	}
	
	public int GetRocketsCount()
	{
		return m_rockets.Count;
	}

	public int GetEnginesCount()
	{
		return m_engines.Count;
	}
}
