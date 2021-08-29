using UnityEngine;
using System.Collections;
using System;

public class RocketControl : MonoBehaviour {
	[SerializeField] private string m_rocketName;
	[SerializeField] private float m_agility;
	[SerializeField] private float m_mass;
	[SerializeField] private float m_durability;

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
	public float HealthMax { get { return m_durability; } }

	public void Reset()
	{
		m_isAlive = true;
		m_currentHealth = m_durability;
		WorldControl.GetInstance().CallHpChanged(m_currentHealth);

		if (rocketObject)
		{
			rocketObject.SetActive(true);
		}

		if (destroyedRocketObject)
		{
			destroyedRocketObject.SetActive(false);
		}

		m_engineControl.SetEngineActive(false);
	}

	private void Start()
	{
		GetComponent<Rigidbody2D>().mass = m_mass;
	}

	private void Update () 
	{
		if (m_isAlive && WorldControl.GetInstance().IsInGame && !WorldControl.GetInstance().IsPaused)
		{
			var isEngine = Input.GetButton("Jump");
			m_engineControl.SetEngineActive(isEngine);

			float rotation = Input.GetAxis ("Horizontal");
			if (rotation != 0) 
			{
				if (rotation > 0)
				{
					transform.Rotate(0, 0, -1 * (m_agility / (GetComponent<Rigidbody2D>().angularDrag / 3 + 1)) * Time.deltaTime);
				}
				else
				{
					transform.Rotate(0, 0, (m_agility / (GetComponent<Rigidbody2D>().angularDrag / 3 + 1)) * Time.deltaTime);
				}
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

		//Debug.Log ("Set Engine new = " + m_engine);
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
			ApplyDamage(myCollision.relativeVelocity.magnitude);
		}
	}

	void OnTriggerEnter2D(Collider2D myCollision)
	{
		if (!m_isAlive)
		{
			return;
		}

		var trigger = myCollision.gameObject.GetComponent<TriggerBase>();
		if (trigger == null)
		{
			return;
		}

		trigger.OnTriggered(this);
	}

	void OnTriggerExit2D(Collider2D myCollision)
	{
		if (!m_isAlive)
		{
			return;
		}

		var trigger = myCollision.gameObject.GetComponent<TriggerBase>();
		if (trigger == null)
		{
			return;
		}

		trigger.OnTrigerExit(this);
	}

	public void KillRocketSilent()
	{
		m_isAlive = false;
		m_currentHealth = 0.0f;
	}

	public void ApplyDamage(float damageAmount)
	{
		m_currentHealth -= damageAmount;
		WorldControl.GetInstance().CallHpChanged(m_currentHealth);

		if (m_currentHealth <= 0)
		{
			OnKilled();
		}
	}

	private void OnKilled()
	{
		m_isAlive = false;
		m_currentHealth = 0.0f;

		if (crashSound)
		{
			WorldControl.GetInstance().soundController.PlayShortSound(crashSound);
		}

		m_engineControl.SetEngineActive(false);

		if (destroyedRocketObject != null)
		{
			destroyedRocketObject.SetActive(true);
		}

		if (rocketObject != null)
		{
			rocketObject.SetActive(false);
		}

		WorldControl.GetInstance().ShowMessage("You broke you rocket. Try again.");
		StartCoroutine(RestartCrt());
	}

	private IEnumerator RestartCrt()
	{
		yield return new WaitForSeconds(2);
		WorldControl.GetInstance().RestartFromCheckpoint();
	}
}
