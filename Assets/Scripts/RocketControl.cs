using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class RocketInfo
{
	public float current_max_agility;
	public float agility;
	public float current_min_agility;
	public float current_max_mass;
	public float mass;
	public float current_min_mass;
	public float current_durability = 1.5f;
}

public class RocketControl : MonoBehaviour {
	public string rocketName;
	public RocketInfo m_info;
	public float max_agility;
	public float min_agility;
	public float agility_upgrade_step;
	public int agility_upgrade_cost;
	public float max_mass;
	public float min_mass;
	public float mass_upgrade_step;
	public int mass_upgrade_cost;
	public int cost;

	public AudioClip crashSound;
	public GameObject exlposionAnimation;

	public Sprite rocketSprite;
	public GameObject rocketObject;
	public GameObject destroyedRocketObject;
	public Collider2D rocketLanding;
	private GameObject m_engine;
	private GameObject m_exlposionAnimation;
	private EngineControl m_engineControl;

	private float m_currentHealth;
	private bool m_isAlive = true;

	public float HealthCurrent { get { return m_currentHealth; } }
	public float HealthMax { get { return m_info.current_durability; } }

	public void Reset()
	{
		m_isAlive = true;
		m_currentHealth = m_info.current_durability;
		WorldControl.GetInstance().CallHpChanged(m_currentHealth);

		if (rocketObject)
		{
			rocketObject.SetActive(true);
		}

		if (destroyedRocketObject)
		{
			destroyedRocketObject.SetActive(false);
		}

		if (m_engineControl)
		{
			m_engineControl.stopEngine();
		}
	}

	void Update () 
	{
		if (m_isAlive && WorldControl.GetInstance().IsInGame && !WorldControl.GetInstance().IsPaused)
		{
			if (Input.GetButtonDown ("Jump")) 
			{
				m_engineControl.startEngine();
			}
			if (Input.GetButtonUp("Jump"))
			{
				m_engineControl.stopEngine();
			}

			float rotation = Input.GetAxis ("Horizontal");
			if (rotation != 0) {
				GetComponent<Rigidbody2D>().freezeRotation = true;
				if (rotation > 0)
					transform.Rotate (0, 0, -1 * (m_info.agility/(GetComponent<Rigidbody2D>().angularDrag/3 + 1)) * Time.deltaTime);
				else
					transform.Rotate (0, 0, (m_info.agility/(GetComponent<Rigidbody2D>().angularDrag/3 + 1)) * Time.deltaTime);
				GetComponent<Rigidbody2D>().freezeRotation = false;
			}
		}
	}

	public void setEngine(GameObject engine)
	{
		//Debug.Log ("Set Engine old = " + m_engine);
		GameObject temp_engine = GameObject.Instantiate (engine) as GameObject;
		m_engine = temp_engine; 
		m_engine.transform.parent = transform;
		m_engine.transform.localPosition = new Vector3(0,0,0);
		m_engine.transform.localRotation = Quaternion.identity;
		m_engineControl = m_engine.GetComponent<EngineControl> ();

		LoadEngineParameners ();
		//Debug.Log ("Set Engine new = " + m_engine);
	}

	public void LoadEngineParameners()
	{
		if (GameProgress.buyedEngines.ContainsKey(m_engineControl.engineName))
		{
			m_engineControl.m_info = GameProgress.buyedEngines[m_engineControl.engineName];
		}
	}

	public GameObject getEngine()
	{
		return m_engine;
	}

	public EngineControl getEngineControl()
	{
		return m_engineControl;
	}

	void OnCollisionEnter2D(Collision2D myCollision) 
	{   
		if (m_isAlive && myCollision.otherCollider.name != rocketLanding.name) 
		{
			if (exlposionAnimation)
			{
				if (m_exlposionAnimation == null)
				{
					m_exlposionAnimation = GameObject.Instantiate(exlposionAnimation) as GameObject;
				}

				m_exlposionAnimation.transform.position = transform.position;
				m_exlposionAnimation.GetComponent<ParticleSystem>().Play();
			}
			m_currentHealth -= myCollision.relativeVelocity.magnitude;
			WorldControl.GetInstance().CallHpChanged(m_currentHealth);
			if (m_currentHealth <= 0)
			{
				m_isAlive = false;
				WorldControl wc = WorldControl.GetInstance();
				if (wc != null)
				{
					wc.PlayOneShotFX(crashSound);
					m_engineControl.stopEngine();
					GetComponent<Rigidbody2D>().freezeRotation = false;

					if (destroyedRocketObject && rocketObject)
					{
						rocketObject.SetActive(false);
						destroyedRocketObject.SetActive(true);
					}

					wc.StartTimer(2.0f, false);
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D myCollision)
	{
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			if (m_isAlive && myCollision.gameObject.name == WorldControl.END_LOCATION_NAME)
			{
				//TODO не момнгьпбнпя победа а надо продержаться 2 секунды
				wc.StartTimer(WorldControl.WIN_TIMER, true);
			}
			else if (m_isAlive && myCollision.tag == WorldControl.COLLECTIBLES_TAG)
			{
				if (wc.getMap().GetComponent<MapInfo>().m_collectSound)
				{
					wc.PlayOneShotFX(wc.getMap().GetComponent<MapInfo>().m_collectSound);
				}
				GameObject.Destroy(myCollision.gameObject);
			}
		}
	}

	void OnTriggerExit2D(Collider2D myCollision)
	{
		WorldControl wc = WorldControl.GetInstance();
		if (wc != null)
		{
			if (m_isAlive && myCollision.gameObject.name == WorldControl.END_LOCATION_NAME)
			{
				wc.StopTimer();
			}
		}
	}

	public void upgradeHullAgility()
	{
		m_info.current_max_agility += agility_upgrade_step;
		if (m_info.current_max_agility > max_agility)
		{
			m_info.current_max_agility = max_agility;
		}
		m_info.current_min_agility -= agility_upgrade_step;
		if (m_info.current_min_agility < min_agility)
		{
			m_info.current_min_agility = min_agility;
		}
	}

	public void upgradeHullMass()
	{
		m_info.current_min_mass -= mass_upgrade_step;
		if (m_info.current_min_mass < min_mass)
		{
			m_info.current_min_mass = min_mass;
		}
	}

	public void setMass(float rocketMass)
	{
		m_info.mass = rocketMass;
	}

	public float getMass()
	{
		return m_info.mass;
	}

	public void KillRocketSilent()
	{
		m_isAlive = false;
		m_currentHealth = 0.0f;
	}
}
