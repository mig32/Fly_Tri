using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private Transform targetTransform;		// Transform to follow

	// Update is called once per frame
	void Update () {
		if (targetTransform)
			transform.position = targetTransform.position;
	}

	private void zooming(float scale)
	{

	}

	public void setTargetTransform(Transform target)
	{
		targetTransform = target;
	}


}
