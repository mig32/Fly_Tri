using UnityEngine;
using System.Collections;

[System.Serializable]
public class EngineInfo
{
	public float current_max_power;
	public float enginePower;
	public float current_min_power;
}

public class EngineControl : MonoBehaviour
{
	public string engineName;
	public EngineInfo m_info;
	public float max_engine_power;
	public float min_engine_power;
	public float power_upgrade_step;
	public int power_upgrade_cost;
	public int cost;
	public Texture2D image;
	public AudioClip engineStartSound;
	public AudioClip engineWorkingSound;
	public AudioClip engineStopSound;



	private bool b_isEngineWorking = false;

	void Start()
	{
	}

	public virtual void upgradeEngine()
	{
		m_info.current_max_power += power_upgrade_step;
		if (m_info.current_max_power > max_engine_power)
		{
			m_info.current_max_power = max_engine_power;
		}
		m_info.current_min_power -= power_upgrade_step;
		if (m_info.current_min_power < min_engine_power)
		{
			m_info.current_min_power = min_engine_power;
		}
	}

	public void setEnginePower(float power)
	{
		m_info.enginePower = power;
	}

	public float getEnginePower()
	{
		return m_info.enginePower;
	}

	public void startEngine ()
	{
		b_isEngineWorking = true;
		onEngineStart ();
	}

	public void stopEngine ()
	{
		b_isEngineWorking = false;
		onEngineStop ();
	}

//=======================================================
	public virtual void onEngineStart()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem != null)
		{
			particleSystem.Play();
		}
		if (engineStartSound)
		{
			WorldControl wc = WorldControl.GetInstance();
			if (wc != null)
			{
				wc.PlayOneShotFX(engineStartSound);
			}
		}
	}

	public virtual void onEngineStop()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem != null)
		{
			particleSystem.Stop();
		}
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			wc.StopConstFX();
			if (engineStopSound)
			{
				wc.PlayOneShotFX(engineStopSound);
			}
		}
	}

	public virtual void onEngineWorking()
	{
		transform.parent.GetComponent<Rigidbody2D>().AddForce ((Vector2)transform.parent.up * m_info.enginePower * Time.deltaTime);
		WorldControl wc = WorldControl.GetInstance();
		if (engineWorkingSound && wc != null && !wc.IsFXSoundPlaying())
		{
			wc.PlayConstFX(engineWorkingSound);
		}
	}
//=======================================================

	void Update () 
	{
		if (b_isEngineWorking)
		{
			onEngineWorking();
		}
	}
}
