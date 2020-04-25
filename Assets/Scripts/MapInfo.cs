using UnityEngine;
using System.Collections;

public class MapInfo : MonoBehaviour {
	public string mapName = "map";
	public float mapGravity = 1.0f;
	public float mapDrag = 2.0f;
	public float mapFriction = 1.0f;
	public float mapBounciness = 0.2f;
	public string mapDiscription = "Наслаждайся =)";
	public int coinCost = 100;
	public Texture2D mapLogo;
	public Texture2D miniImage;
	public AudioClip music;
	public AudioClip collectSound;

	void Start () 
	{
		if (!miniImage) 
		{
			miniImage = Resources.Load("Images/Default_mini_map") as Texture2D;
		}
		if (!mapLogo) 
		{
			mapLogo = Resources.Load("Images/Default_map_logo") as Texture2D;
		}
	}


}

