using UnityEngine;
using System.Collections;

public class ImpulseEngineControl : EngineControl {

	override public void onEngineStart()
	{
		ParticleSystem particleSystem = GetComponent<ParticleSystem>();
		if (particleSystem != null)
		{
			particleSystem.Play();
		}
		transform.parent.GetComponent<Rigidbody2D>().AddForce ((Vector2)transform.parent.up * m_info.enginePower * Time.deltaTime);
		if (engineStartSound)
		{
			WorldControl wc = WorldControl.GetInstance();
			if (wc != null)
			{
				wc.playOneShotFX(engineStartSound);
			}
		}
	}
	
	override public void onEngineStop()
	{

	}
	
	override public void onEngineWorking()
	{

	}
}
