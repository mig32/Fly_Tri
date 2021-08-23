using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	void Start () 
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


}

