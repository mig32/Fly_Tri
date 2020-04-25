using UnityEngine;
using System.Collections;

[System.Serializable]
public class RocketInfo
{
	public float current_max_agility;
	public float agility;
	public float current_min_agility;
	public float current_max_mass;
	public float mass;
	public float current_min_mass;
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
	public Sprite destroyedRocketSprite;
	private GameObject m_engine;
	private EngineControl m_engineControl;

	public bool b_isAlive = true;

	// Update is called once per frame
	void Update () 
	{
		if (b_isAlive)
		{
			if (Input.GetButtonDown ("Jump")) 
			{
				m_engineControl.startEngine();
			}
			if (Input.GetButtonUp ("Jump")) 
				m_engineControl.stopEngine();

			float rotation = Input.GetAxis ("Horizontal");
			if (rotation != 0) {
				GetComponent<Rigidbody2D>().fixedAngle = true;
				if (rotation > 0)
					transform.Rotate (0, 0, -1 * (m_info.agility/(GetComponent<Rigidbody2D>().angularDrag/3 + 1)) * Time.deltaTime);
				else
					transform.Rotate (0, 0, (m_info.agility/(GetComponent<Rigidbody2D>().angularDrag/3 + 1)) * Time.deltaTime);
				GetComponent<Rigidbody2D>().fixedAngle = false;
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
		if (b_isAlive && myCollision.otherCollider.tag != WorldControl.ROCKET_LANDING_TAG) 
		{
			b_isAlive = false;
			WorldControl.playOneShotFX(crashSound);
			m_engineControl.stopEngine();
			GetComponent<Rigidbody2D>().fixedAngle = false;

			WorldControl.clearTempScore();
			GameProgress.score -= (int)((cost + m_engineControl.cost)/2);
			WorldControl.showScore();

			if (exlposionAnimation)
			{
				//GameObject explosion = GameObject.Instantiate(exlposionAnimation) as GameObject;
				//explosion.GetComponent<SpriteRenderer>().sortingLayerName = "Foreground";
				//explosion.transform.position = transform.position;
				//explosion.GetComponent<ParticleSystem>().Emit(100);

			}
			if (destroyedRocketSprite) 
			{
					gameObject.GetComponent<SpriteRenderer>().sprite = destroyedRocketSprite;
			}
			WorldControl.startTimer(2.0f, false);
		}
	}

	void OnTriggerEnter2D(Collider2D myCollision) 
	{   
		if (b_isAlive && myCollision.gameObject.name == WorldControl.END_LOCATION_NAME) 
		{
			//TODO не момнгьпбнпя победа а надо продержаться 2 секунды
			WorldControl.startTimer(WorldControl.WIN_TIMER, true);
		}
		else if (b_isAlive && myCollision.tag == WorldControl.COLLECTIBLES_TAG)
		{
			if (WorldControl.getMap().GetComponent<MapInfo>().collectSound)
			{
				WorldControl.playOneShotFX(WorldControl.getMap().GetComponent<MapInfo>().collectSound);
			}
			GameObject.Destroy(myCollision.gameObject);
			WorldControl.addScore(WorldControl.getMapInfo().coinCost);
		}
	}

	void OnTriggerExit2D(Collider2D myCollision) 
	{   
		if (b_isAlive && myCollision.gameObject.name == WorldControl.END_LOCATION_NAME) 
		{
			WorldControl.stopTimer();
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
		WorldControl.addScore (-agility_upgrade_cost);
	}

	public void upgradeHullMass()
	{
		m_info.current_min_mass -= mass_upgrade_step;
		if (m_info.current_min_mass < min_mass)
		{
			m_info.current_min_mass = min_mass;
		}
		WorldControl.addScore (-mass_upgrade_cost);
	}

	public void setMass(float rocketMass)
	{
		m_info.mass = rocketMass;
	}

	public float getMass()
	{
		return m_info.mass;
	}
}
