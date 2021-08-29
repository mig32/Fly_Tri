using UnityEngine;
using System.Collections;

public class ImpulseEngineControl : EngineControl {

	override public void OnEngineStart()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem != null)
		{
			particleSystem.Play();
		}
		transform.parent.GetComponent<Rigidbody2D>().AddForce ((Vector2)transform.parent.up * m_power * Time.deltaTime);
		if (m_engineStartSound)
		{
			WorldControl.GetInstance().soundController.PlayShortSound(m_engineStartSound);
		}
	}
	
	override public void OnEngineStop()
	{

	}
	
	override public void OnEngineWorking()
	{

	}
}
