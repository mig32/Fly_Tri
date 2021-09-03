using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	[SerializeField] private float m_zoom = 10.0f;
	[SerializeField] private float m_maxZoom = 20.0f;
	[SerializeField] private float m_minZoom = 2.0f;

	private RocketControl m_rocket;

	public void Start()
	{
		WorldControl.GetInstance().OnRocketCreated += OnRocketCreated;
	}

	public void OnRocketCreated(RocketControl rocket)
	{
		m_rocket = rocket;
		GetComponent<Camera>().orthographicSize = m_zoom;
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
			Zooming(zoom * 10);
		}
	}

	private void Zooming(float zoom)
	{
		m_zoom -= zoom;
		if (m_zoom > m_maxZoom)
		{
			m_zoom = m_maxZoom;
		}
		else if (m_zoom < m_minZoom)
		{
			m_zoom = m_minZoom;
		}
		GetComponent<Camera>().orthographicSize = m_zoom;
	}
}
