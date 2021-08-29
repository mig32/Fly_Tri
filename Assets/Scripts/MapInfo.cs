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
	public Transform m_startLocation;
	public List<TriggerEndZone> m_targetLocations;

	private void Start () 
	{
		if (m_miniImage = null) 
		{
			m_miniImage = MapsHelper.GetDefaultMiniMap();
		}

		if (m_mapIcon = null) 
		{
			m_mapIcon = MapsHelper.GetDefaultIcon();
		}
	}

	public TriggerEndZone GetNextCheckpoint()
	{
		return m_targetLocations.FirstOrDefault(endZone => !endZone.IsChecked);
	}

	public void OnDrawGizmos()
	{
		if (m_startLocation != null)
		{
			var pos = m_startLocation.position;
			pos.y += 0.4f;
			Gizmos.DrawIcon(pos, "StartLocationGizmo.jpg", true);
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

