using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MapInfo : MonoBehaviour 
{
	public string m_mapName = "map";
	public float m_mapGravity = 1.0f;
	public float m_mapDrag = 2.0f;
	public float m_mapFriction = 1.0f;
	public float m_mapBounciness = 0.2f;
	public string m_mapDiscription = "Наслаждайся =)";
	public int m_coinCost = 100;
	public Sprite m_mapIcon;
	public Sprite m_miniImage;
	public AudioClip m_music;
	public AudioClip m_collectSound;
	[SerializeField] private List<TriggerCheckpoint> m_checkpoints;
	[SerializeField] private List<TriggerTargetZone> m_targetLocations;

	private void Awake() 
	{
		if (m_checkpoints != null && m_checkpoints.Any())
		{
			for (int i = 0; i < m_checkpoints.Count; ++i)
			{
				m_checkpoints[i].SetIdx(i);
			}
		}

		if (m_miniImage = null) 
		{
			m_miniImage = MapsHelper.GetDefaultMiniMap();
		}

		if (m_mapIcon = null) 
		{
			m_mapIcon = MapsHelper.GetDefaultIcon();
		}
	}

	public Transform GetCheckpointPosition(int idx)
	{
		if (idx >= m_checkpoints.Count || idx < 0)
		{
			return null;
		}

		return m_checkpoints[idx].transform;
	}

	public void MarkCheckedTargetZones(int zonesAmount)
	{
		if (zonesAmount <= 0)
		{
			return;
		}

		for (int i = 0; i < zonesAmount && i < m_targetLocations.Count - 1; ++i)
		{
			m_targetLocations[i].MarkAsChecked();
		}
	}
	
	public TriggerTargetZone GetNextTargetZone()
	{
		return m_targetLocations.FirstOrDefault(endZone => !endZone.IsChecked);
	}

	public void OnDrawGizmos()
	{
		if (m_checkpoints != null && m_checkpoints.Any())
		{
			foreach (var pstartPos in m_checkpoints)
			{
				var pos = pstartPos.transform.position;
				pos.y += 0.4f;
				Gizmos.DrawIcon(pos, "StartLocationGizmo.jpg", true);
			}
		}

		if (m_targetLocations != null && m_targetLocations.Any())
		{
			foreach (var target in m_targetLocations)
			{
				var targetCollider = target.GetComponent<Collider2D>();
				if (targetCollider != null)
				{
					Gizmos.DrawWireCube(targetCollider.bounds.center, targetCollider.bounds.size);
				}
			}
		}
	}
}

