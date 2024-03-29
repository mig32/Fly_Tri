﻿using UnityEngine;
using System.Collections;
using System;

public class RocketControl : MonoBehaviour {
	[SerializeField] private string m_rocketName;
	[SerializeField] private float m_agility;
	[SerializeField] private float m_mass;
	[SerializeField] private float m_durability;
	[SerializeField] private Transform m_rotatingPosition;

	public AudioClip crashSound;
	public GameObject exlposionAnimation;

	public GameObject rocketObject;
	public GameObject destroyedRocketObject;
	public Collider2D rocketLanding;
	private GameObject m_exlposionAnimation;
	private EngineControl m_engineControl;
	private bool m_isAlive = true;
	private bool m_enableControl = true;

	public float HealthCurrent { get; private set; }
	public float HealthMax { get { return m_durability; } }

	private readonly int TeleportStart = Animator.StringToHash("StartTeleport");
	private readonly int TeleportFinish = Animator.StringToHash("FinishTeleport");

	public static RocketControl Create(RocketControl rocketPrefab, EngineControl enginePrefab, Transform targetTransform)
	{
		var rocket = Instantiate(rocketPrefab, targetTransform);
		rocket.SetEngine(enginePrefab);
		return rocket;
	}

	private void SetEngine(EngineControl engine)
	{
		m_engineControl = Instantiate(engine, transform);
		m_engineControl.transform.localPosition = new Vector3(0, 0, 0);
		m_engineControl.transform.localRotation = Quaternion.identity;
		m_engineControl.PhysicBody = GetComponent<Rigidbody2D>();
	}

	public void Reset()
	{
		m_isAlive = true;
		m_enableControl = true;
		HealthCurrent = m_durability;
		WorldControl.GetInstance().CallHpChanged(HealthCurrent);

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
		GetComponent<Rigidbody2D>().isKinematic = true;
	}

	private void Update() 
	{
		if (!m_enableControl || !WorldControl.GetInstance().IsInGame || WorldControl.GetInstance().IsPaused)
		{
			return;
		}

		var isEngine = Input.GetButton("Jump");
		m_engineControl.SetEngineActive(isEngine);

		float rotation = Input.GetAxis("Horizontal");
		if (rotation != 0) 
		{
			var sign = rotation > 0 ? -1 : 1;
			transform.Rotate(0, 0, sign * Time.deltaTime * (1 + m_agility / GetComponent<Rigidbody2D>().angularDrag), Space.Self);
		}
	}

	public EngineControl GetEngineControl()
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
					m_exlposionAnimation = Instantiate(exlposionAnimation, transform);
				}

				m_exlposionAnimation.transform.position = myCollision.GetContact(0).point;
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

	private void OnTriggerStay2D(Collider2D myCollision)
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

		trigger.OnTrigerStay(this);
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
		m_enableControl = false;
		HealthCurrent = 0.0f;
	}

	public void ApplyDamage(float damageAmount)
	{
		HealthCurrent -= damageAmount;
		WorldControl.GetInstance().CallHpChanged(HealthCurrent);

		if (HealthCurrent <= 0)
		{
			OnKilled();
		}
	}

	private void OnKilled()
	{
		m_isAlive = false;
		m_enableControl = false;
		HealthCurrent = 0.0f;

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

	public void StartTeleport()
	{
		m_enableControl = false;

		var rigidbody = GetComponent<Rigidbody2D>();
		rigidbody.isKinematic = true;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = 0;

		var animator = GetComponent<Animator>();
		if (animator)
		{
			animator.SetTrigger(TeleportStart);
		}
	}
	public void FinishTeleport()
	{
		var animator = GetComponent<Animator>();
		if (animator)
		{
			animator.SetTrigger(TeleportFinish);
		}
		else
		{
			m_enableControl = true;
			GetComponent<Rigidbody2D>().isKinematic = false;
		}
	}

	public void OnFinishTeleportAnimEnded()
	{
		m_enableControl = true;
		GetComponent<Rigidbody2D>().isKinematic = false;
	}
}
