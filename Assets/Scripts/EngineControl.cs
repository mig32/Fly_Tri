using UnityEngine;
using System.Collections;

public class EngineControl : MonoBehaviour
{
	[SerializeField] protected string m_name;
	[SerializeField] protected float m_power;
	[SerializeField] protected Texture2D m_image;
	[SerializeField] protected AudioClip m_engineStartSound;
	[SerializeField] protected AudioClip m_engineWorkingSound;
	[SerializeField] protected AudioClip m_engineStopSound;

	private bool m_isEngineWorking = false;

	void Start()
	{
		if (m_engineWorkingSound)
		{
			WorldControl.GetInstance().soundController.SetEngineAudioClip(m_engineWorkingSound);
		}
	}


	public void SetEngineActive(bool isActive)
	{
		if (m_isEngineWorking == isActive)
		{
			return;
		}

		m_isEngineWorking = isActive;
		if (isActive)
		{
			OnEngineStart();
		}
		else
		{
			OnEngineStop();
		}
	}

//=======================================================
	public virtual void OnEngineStart()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem)
		{
			particleSystem.Play();
		}

		if (m_engineStartSound)
		{
			WorldControl.GetInstance().soundController.PlayShortSound(m_engineStartSound);
		}

		if (m_engineWorkingSound)
		{
			WorldControl.GetInstance().soundController.PlayEngineSound();
		}
	}

	public virtual void OnEngineStop()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem)
		{
			particleSystem.Stop();
		}
		
		if (m_engineStopSound)
		{
			WorldControl.GetInstance().soundController.PlayShortSound(m_engineStopSound);
		}

		if (m_engineWorkingSound)
		{
			WorldControl.GetInstance().soundController.StopEngineSound();
		}
	}

	public virtual void OnEngineWorking()
	{
		transform.parent.GetComponent<Rigidbody2D>().AddForce ((Vector2)transform.parent.up * m_power * Time.deltaTime);
	}
//=======================================================

	void FixedUpdate() 
	{
		if (m_isEngineWorking)
		{
			OnEngineWorking();
		}
	}
}
