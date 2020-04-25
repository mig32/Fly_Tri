using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private Transform targetTransform;		// Transform to follow
	private float m_zoom = 3.0f;
	public const float MAX_ZOOM = 6.0f;
	public const float MIN_ZOOM = 0.7f;
	
	// Update is called once per frame
	void LateUpdate () {
		if (targetTransform)
		{
			transform.position = targetTransform.position;
		}
		float zoom = Input.GetAxis("Mouse ScrollWheel");
		if (zoom != 0.0f)
		{
			zooming(zoom);
			//Debug.Log(zoom);
		}

	}

	private void zooming(float zoom)
	{
		m_zoom -= zoom;
		if (m_zoom > MAX_ZOOM)
		{
			m_zoom = MAX_ZOOM;
		}
		else if (m_zoom < MIN_ZOOM)
		{
			m_zoom = MIN_ZOOM;
		}
		GetComponent<Camera>().orthographicSize = m_zoom;

	}

	public void setTargetTransform(Transform target)
	{
		targetTransform = target;
	}
}
