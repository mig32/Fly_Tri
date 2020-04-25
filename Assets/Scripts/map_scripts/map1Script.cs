using UnityEngine;
using System.Collections;

public class map1Script : MonoBehaviour {

// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update()
	{
		foreach (GameObject coin in GameObject.FindGameObjectsWithTag(WorldControl.COLLECTIBLES_TAG))
		{
			coin.transform.Rotate (0, 0, -200 * Time.deltaTime);
		}
	}
}
