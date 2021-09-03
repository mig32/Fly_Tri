using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	private float m_zoom = 3.0f;
	public const float MAX_ZOOM = 6.0f;
	public const float MIN_ZOOM = 0.7f;

	private GameObject m_rocket;

	public void Start()
	{
		WorldControl.GetInstance().OnRocketCreated += OnRocketCreated;
	}

	public void OnRocketCreated(GameObject rocket)
	{
		m_rocket = rocket;
	}

	void LateUpdate () 
	{
		if (m_rocket != null)
		{
			transform.position = m_rocket.transform.position;
		}

		float zoom = Input.GetAxis("Mouse ScrollWheel");
		if (zoom != 0.0f)
		{
			Zooming(zoom);
		}
	}

	private void Zooming(float zoom)
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
}
