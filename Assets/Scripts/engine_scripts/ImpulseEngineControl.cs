using UnityEngine;
using System.Collections;

public class ImpulseEngineControl : EngineControl {

	override public void onEngineStart()
	{
		GetComponent<ParticleSystem>().Emit(100);
		transform.parent.GetComponent<Rigidbody2D>().AddForce ((Vector2)transform.parent.up * m_info.enginePower * Time.deltaTime);
		if (engineStartSound)
		{
			WorldControl.playOneShotFX (engineStartSound);
		}
	}
	
	override public void onEngineStop()
	{

	}
	
	override public void onEngineWorking()
	{

	}
}
